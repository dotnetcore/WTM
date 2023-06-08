using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.Vue3Demo.testvue.ViewModels.SchoolVue3VMs
{
    public partial class SchoolVue3BatchVM : BaseBatchVM<SchoolVue3, SchoolVue3_BatchEdit>
    {
        public SchoolVue3BatchVM()
        {
            ListVM = new SchoolVue3ListVM();
            LinkedVM = new SchoolVue3_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class SchoolVue3_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
