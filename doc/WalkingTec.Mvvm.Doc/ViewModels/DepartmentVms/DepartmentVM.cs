using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Doc.ViewModels.DepartmentVms
{
    public class DepartmentVM : BaseCRUDVM<Department>
    {
        public List<TreeSelectListItem> AllDeps { get; set; }
        public List<Guid?> SelectedIds { get; set; }
        protected override void InitVM()
        {
            
            AllDeps = new List<TreeSelectListItem>{
                new TreeSelectListItem
                {
                  Text = Localizer["Dep"]+ "1",
                  Id = Guid.NewGuid().ToString(),
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = Localizer["Dep"]+ "1-1",
                          Id = Guid.NewGuid().ToString(),
                          Children = new List<TreeSelectListItem>
                          {
                              new TreeSelectListItem
                              {
                                  Text = Localizer["Dep"]+ "1-1-1",
                                  Id = Guid.NewGuid().ToString(),
                              },
                              new TreeSelectListItem
                              {
                                  Text = Localizer["Dep"]+ "1-1-2",
                                  Id = Guid.NewGuid().ToString(),
                              }

                          }
                      },
                      new TreeSelectListItem
                      {
                          Text = Localizer["Dep"]+ "1-2",
                          Id = Guid.NewGuid().ToString()
                      },
                      new TreeSelectListItem
                      {
                          Text = Localizer["Dep"]+ "1-3",
                          Id = Guid.NewGuid().ToString()
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = Localizer["Dep"]+ "2",
                  Id = Guid.NewGuid().ToString(),
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = Localizer["Dep"]+ "2-1",
                          Id = Guid.NewGuid().ToString()
                      },
                      new TreeSelectListItem
                      {
                          Text = Localizer["Dep"]+ "2-2",
                          Id = Guid.NewGuid().ToString()
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = Localizer["Dep"]+ "3",
                  Id = Guid.NewGuid().ToString()
                }

            };
        }
    }
}
