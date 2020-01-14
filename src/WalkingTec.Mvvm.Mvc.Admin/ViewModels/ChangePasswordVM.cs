using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Admin.ViewModels
{
    public class ChangePasswordVM : BaseVM
    {
        [Display(Name = "Account")]
        public Guid UserId { get; set; }

        [Display(Name = "OldPassword")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string OldPassword { get; set; }

        [Display(Name = "NewPassword")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string NewPassword { get; set; }

        [Display(Name = "NewPasswordComfirm")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string NewPasswordComfirm { get; set; }

        /// <summary>
        /// 自定义验证函数，验证原密码是否正确，并验证两次新密码是否输入一致
        /// </summary>
        /// <returns>验证结果</returns>
        public override void Validate()
        {
            List<ValidationResult> rv = new List<ValidationResult>();
            //检查原密码是否正确，如不正确则输出错误
            if (DC.Set<FrameworkUserBase>().Where(x => x.ITCode == LoginUserInfo.ITCode && x.Password == Utils.GetMD5String(OldPassword)).SingleOrDefault() == null)
            {
                MSD.AddModelError("OldPassword",Program._localizer["OldPasswrodWrong"]);
            }
            //检查两次新密码是否输入一致，如不一致则输出错误
            if (NewPassword != NewPasswordComfirm)
            {
                MSD.AddModelError("NewPasswordComfirm", Program._localizer["PasswordNotSame"]);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public void DoChange()
        {
            var user = DC.Set<FrameworkUserBase>().Where(x => x.ITCode == LoginUserInfo.ITCode).SingleOrDefault();
            if (user != null)
            {
                user.Password = Utils.GetMD5String(NewPassword);
            }
            DC.SaveChanges();
        }
    }
}
