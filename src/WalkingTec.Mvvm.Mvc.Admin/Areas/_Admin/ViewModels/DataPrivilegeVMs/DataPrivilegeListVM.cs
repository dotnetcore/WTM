using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public class DataPrivilegeListVM : BasePagedListVM<DataPrivilege_ListView, DataPrivilegeSearcher>
    {

        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("DataPrivilege", GridActionStandardTypesEnum.Create, "新建","_Admin", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<DataPrivilege_ListView>> InitGridHeader()
        {
            return new List<GridColumn<DataPrivilege_ListView>>{
                this.MakeGridHeader(x => x.Name, 200),
                this.MakeGridHeader(x => x.TableName).SetFormat((entity,val)=>GetPrivilegeName(entity)),
                this.MakeGridHeader(x => x.RelateIDs),
                this.MakeGridHeader(x=>x.Edit,200).SetFormat((entity,val)=>GetOperation(entity)).SetHeader("操作"),
                this.MakeGridHeader(x => x.DpType).SetHide(true),
                this.MakeGridHeader(x => x.TargetId).SetHide(true)
           };
        }


        public string GetPrivilegeName(DataPrivilege_ListView item)
        {
            var temp = GlobalServices.GetService<Configs>().DataPrivilegeSettings.Where(x => x.ModelName == item.TableName).SingleOrDefault();
            if (temp == null)
            {
                return "";
            }
            else
            {
                return temp.PrivillegeName;
            }
        }

        public List<ColumnFormatInfo> GetOperation(DataPrivilege_ListView item)
        {
            string editurl = "";
            string delurl = "";
            if(Searcher.DpType == DpTypeEnum.User)
            {
                editurl = "/_Admin/DataPrivilege/Edit?ModelName=" + item.TableName + "&Type=User&Id=" + item.TargetId;
                delurl = "/_Admin/DataPrivilege/Delete?ModelName=" + item.TableName + "&Type=User&Id=" + item.TargetId;
            }
            else
            {
                editurl = "/_Admin/DataPrivilege/Edit?ModelName=" + item.TableName + "&Type=UserGroup&Id=" + item.TargetId;
                delurl = "/_Admin/DataPrivilege/Delete?ModelName=" + item.TableName + "&Type=UserGroup&Id=" + item.TargetId;
            }
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDialogButton(ButtonTypesEnum.Button,editurl,"修改",800,null,"修改"),
                ColumnFormatInfo.MakeDialogButton(ButtonTypesEnum.Button,delurl,"删除",null,null,showDialog:false)
            };
        }

        /// <summary>
        /// 查询结果
        /// </summary>
        public override IOrderedQueryable<DataPrivilege_ListView> GetSearchQuery()
        {
            IOrderedQueryable<DataPrivilege_ListView> query = null;
            if (Searcher.DpType == DpTypeEnum.User)
            {
                query = DC.Set<DataPrivilege>()
                    .Join(DC.Set<FrameworkUserBase>(), ok => ok.UserId, ik => ik.ID, (dp, user) => new { dp = dp, user = user })
                    .CheckContain(Searcher.Name, x => x.user.Name)
                    .CheckContain(Searcher.TableName, x => x.dp.TableName)
                    .GroupBy(x => new { x.user, x.dp.Domain, x.dp.TableName }, x => x.dp.RelateId)
                    .Select(x => new DataPrivilege_ListView
                    {
                        TargetId = x.Key.user.ID,
                        Name = x.Key.user.Name,
                        TableName = x.Key.TableName,
                        RelateIDs = x.Count(),
                        DpType = (int)Searcher.DpType
                    })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);
            }
            else
            {
                query = DC.Set<DataPrivilege>()
                    .Join(DC.Set<FrameworkGroup>(), ok => ok.GroupId, ik => ik.ID, (dp, group) => new { dp = dp, group = group })
                    .CheckContain(Searcher.Name, x => x.group.GroupName)
                    .CheckContain(Searcher.TableName, x => x.dp.TableName)
                    .GroupBy(x => new { x.group, x.dp.Domain, x.dp.TableName }, x => x.dp.RelateId)
                    .Select(x => new DataPrivilege_ListView
                    {
                        TargetId = x.Key.group.ID,
                        Name = x.Key.group.GroupName,
                        TableName = x.Key.TableName,
                        RelateIDs = x.Count(),
                        DpType = (int)Searcher.DpType
                    })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);

            }
            return query;
        }
    }

    /// <summary>
    /// 如果需要显示树类型的列表需要继承ITreeData`T`接口，并实现Children,Parent,ParentID属性
    /// </summary>
    public class DataPrivilege_ListView : BasePoco
    {
        [Display(Name = "授权对象")]
        public string Name { get; set; }
        public Guid TargetId { get; set; }
        [Display(Name = "权限名称")]
        public string TableName { get; set; }
        [Display(Name = "权限数量")]
        public int RelateIDs { get; set; }
        public int DpType { get; set; }
        [Display(Name = "域名")]
        public string DomainName { get; set; }

        public Guid? DomainID { get; set; }

        public string Edit { get; set; }
    }

}