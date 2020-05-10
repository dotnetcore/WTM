using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;

namespace WalkingTec.Mvvm.Doc.ViewModels.StudentVms
{
    public class StudentVm : BaseCRUDVM<Student>
    {
        public List<ComboSelectListItem> AllSchools { get; set; }
        public string SelectedSchool { get; set; }
        public string SelectedSchool2 { get; set; }
        public string SelectedSchool3 { get; set; }
        [Display(Name = "School")]
        public List<string> SelectedSchools { get; set; }
        public string SelectedMajor { get; set; }
        public string SelectedStudent { get; set; }

        [Display(Name = "School")]
        public List<string> TransferSelectedSchools { get; set; }

        [Display(Name = "Age1")]
        public int Age1 { get; set; }

        [Display(Name = "Age2")]
        public int Age2 { get; set; }

        [Display(Name = "StuCount1")]
        public int StuCount0 { get; set; }
        [Display(Name = "StuCount")]
        public int StuCount1 { get; set; }
        [Display(Name = "StuCount2")]
        public int StuCount2 { get; set; }
        [Display(Name = "StuCount")]
        public int StuCount3 { get; set; }

        public StudentVm()
        {
        }

        protected override void InitVM()
        {
            TransferSelectedSchools = new List<string>() { Localizer["U1"], Localizer["U3"] };
            AllSchools = new List<ComboSelectListItem>()
            {
                new ComboSelectListItem{ Text = Localizer["U1"], Value = Localizer["U1"].ToString().ToLower().Replace(" ","")},
                new ComboSelectListItem{ Text = Localizer["U2"], Value = Localizer["U2"].ToString().ToLower().Replace(" ","")},
                new ComboSelectListItem{ Text = Localizer["U3"], Value = Localizer["U3"].ToString().ToLower().Replace(" ",""), Disabled = true},
                new ComboSelectListItem{ Text = Localizer["U4"], Value = Localizer["U4"].ToString().ToLower().Replace(" ","")},
                new ComboSelectListItem{ Text = Localizer["U5"], Value =Localizer["U5"].ToString().ToLower().Replace(" ","")},
           };

            Entity.EnRollDateRange = DateRange.Week;
            Entity.EnYearRange = DateRange.Today;
            Entity.EnYearRange.SetStartTime(new DateTime(2016, 1, 1));
            Entity.EnMonthRange = new DateRange(new DateTime(2018, 4, 1), new DateTime(2018, 8, 8));
            Entity.EnTimeRange4 = DateRange.UtcDefault;
            Entity.EnTimeRange0 = new DateRange(new DateTime(2018, 4, 29), new DateTime(2018, 8, 8));

            StuCount0 = 40;
            StuCount1 = 60;
            StuCount2 = 40;
            StuCount3 = 60;
        }
    }
}
