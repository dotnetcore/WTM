using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs
{
    public partial class SchoolBatchVM : BaseBatchVM<School, School_BatchEdit>
    {
        public SchoolBatchVM()
        {
            ListVM = new SchoolListVM();
            LinkedVM = new School_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class School_BatchEdit : BaseVM
    {
        [Display(Name = "学校类型")]
        public SchoolTypeEnum? SchoolType { get; set; }
        [Display(Name = "备注")]
        public String Remark { get; set; }
        [Display(Name = "地点")]
        public Guid? PlaceId { get; set; }

        protected override Task InitVM()
        {
            return Task.CompletedTask;
        }

    }

}
