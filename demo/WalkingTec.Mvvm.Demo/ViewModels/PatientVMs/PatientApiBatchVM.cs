using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.Demo.ViewModels.PatientVMs
{
    public partial class PatientApiBatchVM : BaseBatchVM<Patient, PatientApi_BatchEdit>
    {
        public PatientApiBatchVM()
        {
            ListVM = new PatientApiListVM();
            LinkedVM = new PatientApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class PatientApi_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
