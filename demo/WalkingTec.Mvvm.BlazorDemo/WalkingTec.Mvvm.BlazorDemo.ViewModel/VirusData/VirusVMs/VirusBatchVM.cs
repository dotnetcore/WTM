using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs
{
    public partial class VirusBatchVM : BaseBatchVM<Virus, Virus_BatchEdit>
    {
        public VirusBatchVM()
        {
            ListVM = new VirusListVM();
            LinkedVM = new Virus_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class Virus_BatchEdit : BaseVM
    {

        protected override Task InitVM()
        {
            return Task.CompletedTask;
        }

    }

}
