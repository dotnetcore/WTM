using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.PatientVMs
{
    public partial class PatientVM : BaseCRUDVM<Patient>
    {
        public List<ComboSelectListItem> AllLocations { get; set; }
        public List<ComboSelectListItem> AllHospitals { get; set; }
        public List<ComboSelectListItem> AllVirusess { get; set; }
        [Display(Name = "病毒")]
        public List<string> SelectedVirusesIDs { get; set; }

        public PatientVM()
        {
            SetInclude(x => x.Location);
            SetInclude(x => x.Hospital);
            SetInclude(x => x.Viruses);
        }

        protected override async Task InitVM()
        {
            AllLocations = await DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
            AllHospitals = await DC.Set<Hospital>().GetSelectListItems(Wtm, y => y.Name);
            AllVirusess = await DC.Set<Virus>().GetSelectListItems(Wtm, y => y.VirtusName);
            SelectedVirusesIDs = Entity.Viruses?.Select(x => x.VirusId.ToString()).ToList();
        }

        public override async Task DoAdd()
        {
            Entity.Viruses = new List<PatientVirus>();
            if (SelectedVirusesIDs != null)
            {
                foreach (var id in SelectedVirusesIDs)
                {
                     PatientVirus middle = new PatientVirus();
                    middle.SetPropertyValue("VirusId", id);
                    Entity.Viruses.Add(middle);
                }
            }
           
            await base.DoAdd();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            Entity.Viruses = new List<PatientVirus>();
            if(SelectedVirusesIDs != null )
            {
                 foreach (var item in SelectedVirusesIDs)
                {
                    PatientVirus middle = new PatientVirus();
                    middle.SetPropertyValue("VirusId", item);
                    Entity.Viruses.Add(middle);
                }
            }

            await base.DoEdit(updateAllFields);
        }

        public override async Task DoDelete()
        {
            await base.DoDelete();
        }
    }
}
