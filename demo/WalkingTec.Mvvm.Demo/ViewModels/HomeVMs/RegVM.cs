using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class RegVM : BaseVM
    {
        [Display(Name = "Account")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Password { get; set; }

        [Display(Name = "NewPasswordComfirm")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string NewPasswordComfirm { get; set; }

        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "{0}formaterror")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Email { get; set; }

        [Display(Name = "CellPhone")]
        [RegularExpression("^[1][3-9]\\d{9}$", ErrorMessage = "{0}formaterror")]
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
                MSD.AddModelError("ITCode", Localizer["Reg.ItcodeDuplicate"]);
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
