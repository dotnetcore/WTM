using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs
{
    public partial class VirusVM : BaseCRUDVM<Virus>
    {
        [Display(Name = "患者")]
        public List<string> SelectedPatientsIDs { get; set; }

        public VirusVM()
        {
            SetInclude(x => x.Patients);
        }

        protected override void InitVM()
        {
            SelectedPatientsIDs = Entity.Patients?.Select(x => x.PatientId.ToString()).ToList();
        }

        public override void DoAdd()
        {
            Entity.Patients = new List<PatientVirus>();
            if (SelectedPatientsIDs != null)
            {
                foreach (var id in SelectedPatientsIDs)
                {
                     PatientVirus middle = new PatientVirus();
                    middle.SetPropertyValue("PatientId", id);
                    Entity.Patients.Add(middle);
                }
            }
           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            Entity.Patients = new List<PatientVirus>();
            if(SelectedPatientsIDs != null )
            {
                 foreach (var item in SelectedPatientsIDs)
                {
                    PatientVirus middle = new PatientVirus();
                    middle.SetPropertyValue("PatientId", item);
                    Entity.Patients.Add(middle);
                }
            }

            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
