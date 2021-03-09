using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class LoginVM : BaseVM
    {
        [Display(Name = "_Admin.Account")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Login.Password")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "Login.RememberMe")]
        public bool RememberLogin { get; set; }

        private string _redirect;
        public string Redirect
        {
            get
            {
                var rv = _redirect;
                if (string.IsNullOrEmpty(rv) == false)
                {
                    if (rv.StartsWith("/#") == false)
                    {
                        rv = "/#" + rv;
                    }
                    if(rv.Split("#/").Length > 2)
                    {
                        int index = rv.LastIndexOf("#/");
                        rv = rv.Substring(0, index);
                    }
                }
                return rv;
            }
            set { _redirect = value; }
        }

        [Display(Name = "Login.InputValidation")]
        public string VerifyCode { get; set; }

        /// <summary>
        /// 进行登录
        /// </summary>
        /// <param name="ignorePris">外部传递的页面权限</param>
        /// <returns>登录用户的信息</returns>
        public async System.Threading.Tasks.Task<LoginUserInfo> DoLoginAsync(bool ignorePris = false)
        {
            //根据用户名和密码查询用户
            var rv = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == ITCode.ToLower() && x.Password == Utils.GetMD5String(Password)).Select(x => new { itcode = x.ITCode, id = x.ID }).SingleOrDefaultAsync();

            //如果没有找到则输出错误
            if (rv == null)
            {
                MSD.AddModelError("", Localizer["Sys.LoginFailed"]);
                return null;
            }
            else
            {
                LoginUserInfo user = new LoginUserInfo
                {
                    ITCode = rv.itcode,
                    UserId = rv.id.ToString()
                };
                //读取角色，用户组，页面权限，数据权限等框架配置信息
                await user.LoadBasicInfoAsync(Wtm);
                return user;
            }
        }
    }
}
