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
        }
        protected override void InitVM()
        {
        }

        public override bool DoBatchDelete()
        {
            if(Ids != null)
            {
                foreach (var item in Ids)
                {
                    FrameworkMenu f = new FrameworkMenu { ID = item };
                    DC.CascadeDelete(f);
                }
            }
            DC.SaveChanges();
            return true;
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
