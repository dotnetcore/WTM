using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class RegVM : BaseVM
    {
        [Display(Name = "账号")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string ITCode { get; set; }

        [Display(Name = "姓名")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "密码")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Password { get; set; }

        [Display(Name = "确认密码")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string NewPasswordComfirm { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "手机")]
        public string CellPhone { get; set; }

        /// <summary>
        /// 进行登录
        /// </summary>
        /// <returns>登录用户的信息</returns>
        public bool DoReg()
        {
            //检查两次新密码是否输入一致，如不一致则输出错误
            if (Password != NewPasswordComfirm)
            {
                MSD.AddModelError("NewPasswordComfirm", Localizer["PasswordNotSame"]);
                return false;
            }


            //检查itcode是否重复
            var exist = DC.Set<FrameworkUserBase>().Any(x => x.ITCode.ToLower() == ITCode.ToLower());

            if (exist == true)
            {
                MSD.AddModelError("ITCode", "账号重复");
                return false;
            }

            FrameworkUserBase user = new FrameworkUserBase
            {
                ITCode = ITCode,
                Name = Name,
                Password = Utils.GetMD5String(Password),
                IsValid = true,
                CellPhone = CellPhone,
                Email = Email
            };

            DC.Set<FrameworkUserBase>().Add(user);
            DC.SaveChanges();
            return true;
        }
    }
}
