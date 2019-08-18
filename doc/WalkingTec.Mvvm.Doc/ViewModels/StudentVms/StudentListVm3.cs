using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;

namespace WalkingTec.Mvvm.Doc.ViewModels.StudentVms
{
    public class StudentListVm3 : BasePagedListVM<Student,StudentSearcher>
    {
        public List<TreeSelectListItem> AllDeps { get; set; }
        public List<TreeSelectListItem> UrlTree { get; set; }

        [Display(Name = "测试文本")]
        public string Test { get; set; }

        public StudentListVm3()
        {
            AllDeps = new List<TreeSelectListItem>{
                new TreeSelectListItem
                {
                  Text = "部门1",
                          Id = "00000000-0000-0000-0000-000000000001",
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = "部门1-1",
                          Id = "00000000-0000-0000-0000-000000000002",
                          Children = new List<TreeSelectListItem>
                          {
                              new TreeSelectListItem
                              {
                                  Text = "部门1-1-1",
                          Id = "00000000-0000-0000-0000-000000000003",
                              },
                              new TreeSelectListItem
                              {
                                  Text = "部门1-1-2",
                          Id = "00000000-0000-0000-0000-000000000004",
                              }

                          }
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门1-2",
                          Id = "00000000-0000-0000-0000-000000000005",
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门1-3",
                          Id = "00000000-0000-0000-0000-000000000006",
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = "部门2",
                          Id = "00000000-0000-0000-0000-000000000007",
                  Children = new List<TreeSelectListItem>()
                  {
                      new TreeSelectListItem
                      {
                          Text = "部门2-1",
                          Id = "00000000-0000-0000-0000-000000000008",
                      },
                      new TreeSelectListItem
                      {
                          Text = "部门2-2",
                          Id = "00000000-0000-0000-0000-000000000009",
                      }
                  }
                },
                new TreeSelectListItem
                {
                  Text = "部门3",
                          Id = "00000000-0000-0000-0000-000000000010",
                }

            };
            UrlTree = new List<TreeSelectListItem>
            {
                new TreeSelectListItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "内部地址",
                    Children = new List<TreeSelectListItem>
                    {
                        new TreeSelectListItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = "简介",
                            Url = "/QuickStart/Intro"
                        },
                        new TreeSelectListItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = "FAQ",
                            Url = "/QuickStart/faq"
                        },
                    }
                },
                new TreeSelectListItem
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "外部地址",
                    Children = new List<TreeSelectListItem>
                    {
                        new TreeSelectListItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = "必应",
                            Url = "https://www.bing.com"
                        },
                        new TreeSelectListItem
                        {
                            Id = Guid.NewGuid().ToString(),
                            Text = "百度",
                            Url = "https://www.baidu.com"
                        }

                    }
                },
            };
        }


        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {

            };
        }

        protected override IEnumerable<IGridColumn<Student>> InitGridHeader()
        {
            return new List<GridColumn<Student>>{
                this.MakeGridHeader(x => x.LoginName),
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.Sex),
                this.MakeGridHeader(x=>x.Department.DepName),
                this.MakeGridHeader(x => x.CellPhone),
                this.MakeGridHeader(x=>x.IsValid)
            };
        }

        public override IOrderedQueryable<Student> GetSearchQuery()
        {            
            List<Student> data = new List<Student>
            {
                 new Student{ LoginName = "dingyi", Name="丁一", Sex= Models.SexEnum.Male, ExcelIndex = 0, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Id), DepName = AllDeps[0].Text} },
               new Student{ LoginName = "liuer", Name="刘二", Sex= Models.SexEnum.Male,  ExcelIndex = 1, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Children[0].Id), DepName = AllDeps[0].Children[0].Text} },
                new Student{ LoginName = "zhangsan", Name="张三", Sex= Models.SexEnum.Male,  ExcelIndex = 2, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Children[0].Children[0].Id), DepName = AllDeps[0].Children[0].Children[0].Text} },
                new Student{ LoginName = "lisi", Name="李四", Sex= Models.SexEnum.Male,  ExcelIndex = 3, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Children[0].Children[1].Id), DepName = AllDeps[1].Text} },
                new Student{ LoginName = "wangwu", Name="王五", Sex= Models.SexEnum.Male,  ExcelIndex = 4, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Children[1].Id), DepName = AllDeps[0].Children[1].Text} },
                new Student{ LoginName = "zhaoliu", Name="赵六", Sex= Models.SexEnum.Female,  ExcelIndex = 5, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[0].Children[2].Id), DepName = AllDeps[0].Children[2].Text} },
                new Student{ LoginName = "qianqi", Name="钱七", Sex= Models.SexEnum.Female, ExcelIndex = 6, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[1].Id), DepName = AllDeps[1].Text} },
                new Student{ LoginName = "qianqi", Name="冯八", Sex= Models.SexEnum.Female, ExcelIndex = 7, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[1].Children[0].Id), DepName = AllDeps[1].Children[0].Text} },
                new Student{ LoginName = "zhoujiu", Name="周九", Sex= Models.SexEnum.Female,  ExcelIndex = 8, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[1].Children[1].Id), DepName = AllDeps[1].Children[1].Text} },
                new Student{ LoginName = "wushi", Name="吴十", Sex= Models.SexEnum.Female,  ExcelIndex = 9, IsValid = true, ID = Guid.NewGuid(),Department = new Department{ ID = new Guid(AllDeps[2].Id), DepName = AllDeps[2].Text} },
            };

            var query = data.AsQueryable().Where(x=>
                    (string.IsNullOrEmpty(Searcher.LoginName) || x.LoginName.Contains(Searcher.LoginName)) &&
                    (string.IsNullOrEmpty(Searcher.Name) || x.Name.Contains(Searcher.Name)) &&
                    (x.Department.ID == Searcher.DepId) 
                )
                .OrderBy(x => x.ExcelIndex);
            return query;
        }

    }

}
