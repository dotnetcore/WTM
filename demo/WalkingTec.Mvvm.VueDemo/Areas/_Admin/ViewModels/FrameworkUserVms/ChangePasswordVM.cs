// WTM默认页面 Wtm buidin page
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkUserVms
{
    public class ChangePasswordVM : BaseVM
    {
        [Display(Name = "_Admin.Account")]
        public string ITCode { get; set; }

        [Display(Name = "Login.OldPassword")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string OldPassword { get; set; }

        [Display(Name = "Login.NewPassword")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string NewPassword { get; set; }

        [Display(Name = "Login.NewPasswordComfirm")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string NewPasswordComfirm { get; set; }

        public override async Task Validate()
        {
            List<ValidationResult> rv = new List<ValidationResult>();
            var _itcode = (await GetLoginUserInfo ()).ITCode;
            if (DC.Set<FrameworkUser>().Where(x => x.ITCode == _itcode && x.Password == Utils.GetMD5String(OldPassword)).SingleOrDefault() == null)
            {
                MSD.AddModelError("OldPassword", Localizer["Login.OldPasswrodWrong"]);
            }
            if (NewPassword != NewPasswordComfirm)
            {
                MSD.AddModelError("NewPasswordComfirm", Localizer["Login.PasswordNotSame"]);
            }
        }

        public async Task DoChange()
        {
            var _itcode = (await GetLoginUserInfo ()).ITCode;
            var user = await DC.Set<FrameworkUser>().Where(x => x.ITCode == _itcode).SingleOrDefaultAsync();
            if (user != null)
            {
                user.Password = Utils.GetMD5String(NewPassword);
            }
            await DC.SaveChangesAsync();
        }
    }
}
