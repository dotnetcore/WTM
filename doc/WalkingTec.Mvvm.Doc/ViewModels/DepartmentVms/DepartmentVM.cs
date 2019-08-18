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
                  Text = "部门1",
                  Id = Guid.NewGuid().ToString(),
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = "部门1-1",
                          Id = Guid.NewGuid().ToString(),
                          Children = new List<TreeSelectListItem>
                          {
                              new TreeSelectListItem
                              {
                                  Text = "部门1-1-1",
                                  Id = Guid.NewGuid().ToString(),
                              },
                              new TreeSelectListItem
                              {
                                  Text = "部门1-1-2",
                                  Id = Guid.NewGuid().ToString(),
                              }

                          }
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门1-2",
                          Id = Guid.NewGuid().ToString()
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门1-3",
                          Id = Guid.NewGuid().ToString()
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = "部门2",
                  Id = Guid.NewGuid().ToString(),
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = "部门2-1",
                          Id = Guid.NewGuid().ToString()
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门2-2",
                          Id = Guid.NewGuid().ToString()
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = "部门3",
                  Id = Guid.NewGuid().ToString()
                }

            };
        }
    }
}
