using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModel.HomeVMs
{
    public class LoginVM : BaseVM
    {
        [Display(Name = "_Admin.Account")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50)]
        public string ITCode { get; set; }

        [Display(Name = "_Admin.Password")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        /// <summary>
        /// 进行登录
        /// </summary>
        /// <param name="ignorePris">外部传递的页面权限</param>
        /// <returns>登录用户的信息</returns>
        public async System.Threading.Tasks.Task<LoginUserInfo> DoLoginAsync(bool ignorePris = false)
        {
            //根据用户名和密码查询用户
            string code = await DC.Set<FrameworkUser>().Where(x => x.ITCode.ToLower() == ITCode.ToLower() && x.Password == Utils.GetMD5String(Password)).Select(x => x.ITCode).SingleOrDefaultAsync();

            //如果没有找到则输出错误
            if (string.IsNullOrEmpty(code))
            {
                MSD.AddModelError("", Localizer["Sys.LoginFailed"]);
                return null;
            }
            else
            {
                LoginUserInfo user = new LoginUserInfo
                {
                    ITCode = code
                };
                //读取角色，用户组，页面权限，数据权限等框架配置信息
                await user.LoadBasicInfoAsync(Wtm);
                return user;
            }
        }


    }
}
