using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.ISOTypeVMs
{
    public partial class ISOTypeVM : BaseCRUDVM<ISOType>
    {
        public List<ComboSelectListItem> AlliSOTypess { get; set; }
        [Display(Name = "EXE版本")]
        public List<Guid> SelectediSOTypesIDs { get; set; }

        public ISOTypeVM()
        {
            SetInclude(x => x.iSOTypes);
        }

        protected override async Task InitVM()
        {
            AlliSOTypess = await DC.Set<SoftFacInfo>().GetSelectListItems(Wtm, y => y.IsoName);
            SelectediSOTypesIDs = Entity.iSOTypes?.Select(x => x.softFacInfoID).ToList();
        }

        public override async Task DoAdd()
        {
            Entity.iSOTypes = new List<ISOEXE>();
            if (SelectediSOTypesIDs != null)
            {
                foreach (var id in SelectediSOTypesIDs)
                {
                    Entity.iSOTypes.Add(new ISOEXE { softFacInfoID = id });
                }
            }
           
            await base.DoAdd();
        }

        public override async Task DoEdit(bool updateAllFields = false)
        {
            Entity.iSOTypes = new List<ISOEXE>();
            if(SelectediSOTypesIDs != null )
            {
                SelectediSOTypesIDs.ForEach(x => Entity.iSOTypes.Add(new ISOEXE { ID = Guid.NewGuid(), softFacInfoID = x }));
            }

            await base.DoEdit(updateAllFields);
        }

        public override async Task DoDelete()
        {
            await base.DoDelete();
        }
    }
}
