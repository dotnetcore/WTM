using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public class FrameworkMenuBatchVM : BaseBatchVM<FrameworkMenu, FrameworkMenu_BatchEdit>
    {
        public FrameworkMenuBatchVM()
        {
            ListVM = new FrameworkMenuListVM();
            LinkedVM = new FrameworkMenu_BatchEdit();
        }
        protected override void InitVM()
        {
            var topMenu = DC.Set<FrameworkMenu>().Where(x => x.ParentId == null).ToList().FlatTree();
            List<Guid> pids = new List<Guid>();
            foreach (var item in this.Ids)
            {
                pids.AddRange(DC.Set<FrameworkMenu>().Where(x => x.ParentId == item).Select(x => x.ID).ToList());
            }

            LinkedVM.AllParents = topMenu.Where(x => !this.Ids.Contains(x.ID) && !pids.Contains(x.ID)).ToList().ToListItems(y => y.PageName, x=>x.ID);
        }
    }

    /// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class FrameworkMenu_BatchEdit : BaseVM
    {
        public List<Guid> IDs { get; set; }
        [Display(Name = "菜单显示")]
        public bool ShowOnMenu { get; set; }

        [Display(Name = "父级目录")]
        public Guid? ParentID { get; set; }
        public List<ComboSelectListItem> AllParents { get; set; }
        [Display(Name = "图标")]
        public Guid? IconID { get; set; }
    }
}
