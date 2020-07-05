using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public partial class StudentSearcher : BaseSearcher
    {
        [Display(Name = "姓名")]
        public String Name { get; set; }
        [Display(Name = "性别")]
        public SexEnum? Sex { get; set; }
        [Display(Name = "邮编")]
        public String ZipCode { get; set; }
        [Display(Name = "是否有效")]
        public Boolean? IsValid { get; set; }
        [Display(Name = "日期")]
        public DateRange EnRollDate { get; set; }
        public List<ComboSelectListItem> AllStudentMajors { get; set; }
        [Display(Name = "专业")]
        public List<Guid> SelectedStudentMajorIDs { get; set; }

        protected override void InitVM()
        {
            AllStudentMajors = DC.Set<Major>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.MajorName);
        }

    }
}
