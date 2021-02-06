// WTM默认页面 Wtm buidin page
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
            string tp = "";
            if (Searcher.DpType == DpTypeEnum.User)
            {
                tp = "User";
            }
            if (Searcher.DpType == DpTypeEnum.UserGroup)
            {
                tp = "UserGroup";
            }

                return new List<GridAction>
            {
                this.MakeStandardAction("DataPrivilege", GridActionStandardTypesEnum.Create, "","_Admin", dialogWidth: 800).SetQueryString($"Type={tp}"),
                this.MakeStandardAction("DataPrivilege", GridActionStandardTypesEnum.ExportExcel, "","_Admin"),
            };
        }

        protected override IEnumerable<IGridColumn<DataPrivilege_ListView>> InitGridHeader()
        {
            return new List<GridColumn<DataPrivilege_ListView>>{
                this.MakeGridHeader(x => x.Name, 200),
                this.MakeGridHeader(x => x.TableName).SetFormat((entity,val)=>GetPrivilegeName(entity)),
                this.MakeGridHeader(x => x.RelateIDs),
                this.MakeGridHeader(x=>x.Edit,200).SetFormat((entity,val)=>GetOperation(entity)).SetHeader(Localizer["Sys.Operation"]).SetDisableExport(),
                this.MakeGridHeader(x => x.DpType).SetHide(true),
                this.MakeGridHeader(x => x.TargetId).SetHide(true)
           };
        }


        public string GetPrivilegeName(DataPrivilege_ListView item)
        {
            var temp = Wtm.DataPrivilegeSettings.Where(x => x.ModelName == item.TableName).SingleOrDefault();
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
                ColumnFormatInfo.MakeDialogButton(ButtonTypesEnum.Button,editurl,Localizer["Sys.Edit"],800,null,Localizer["Sys.Edit"]),
                ColumnFormatInfo.MakeDialogButton(ButtonTypesEnum.Button,delurl,Localizer["Sys.Delete"],null,null,showDialog:false)
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
                    .Join(DC.Set<FrameworkUser>(), ok => ok.UserCode, ik => ik.ITCode, (dp, user) => new { dp = dp, user = user })
                    .CheckContain(Searcher.Name, x => x.user.Name)
                    .CheckContain(Searcher.TableName, x => x.dp.TableName)
                    .GroupBy(x => new { x.user.Name, x.user.ITCode, x.dp.TableName }, x => x.dp.RelateId)
                    .Select(x => new DataPrivilege_ListView
                    {
                        TargetId = x.Key.ITCode,
                        Name = x.Key.Name,
                        TableName = x.Key.TableName,
                        RelateIDs = x.Count(),
                        DpType = (int)Searcher.DpType
                    })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);
            }
            else
            {
                query = DC.Set<DataPrivilege>()
                    .Join(DC.Set<FrameworkGroup>(), ok => ok.GroupCode, ik => ik.GroupCode, (dp, group) => new { dp = dp, group = group })
                    .CheckContain(Searcher.Name, x => x.group.GroupName)
                    .CheckContain(Searcher.TableName, x => x.dp.TableName)
                       .GroupBy(x => new { x.group.GroupName, x.group.GroupCode, x.dp.TableName }, x => x.dp.RelateId)
                       .Select(x => new DataPrivilege_ListView
                       {
                           TargetId = x.Key.GroupCode,
                           Name = x.Key.GroupName,
                           TableName = x.Key.TableName,
                           RelateIDs = x.Count(),
                           DpType = (int)Searcher.DpType
                       })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);

            }
            return query;
        }
    }

    public class DataPrivilege_ListView : BasePoco
    {
        [Display(Name = "_Admin.DpTargetName")]
        public string Name { get; set; }
        public string TargetId { get; set; }
        [Display(Name = "_Admin.DataPrivilegeName")]
        public string TableName { get; set; }
        [Display(Name = "_Admin.DataPrivilegeCount")]
        public int RelateIDs { get; set; }
        public int DpType { get; set; }
        public string DomainName { get; set; }

        public Guid? DomainID { get; set; }

        public string Edit { get; set; }
    }

}
