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
                this.MakeGridHeader(x => x.PName).SetFormat((entity,val)=>GetPrivilegeName(entity)),
                this.MakeGridHeader(x => x.TableName),
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
                query = DC.Set<DataPrivilege>().Where(x=>x.UserCode != null)
                    .CheckContain(Searcher.Name, x => x.UserCode)
                    .CheckContain(Searcher.TableName, x => x.TableName)
                    .GroupBy(x => new { x.UserCode, x.TableName }, x => x.RelateId)
                    .Select(x => new DataPrivilege_ListView
                    {
                        TargetId = x.Key.UserCode,
                        Name = x.Key.UserCode,
                        TableName = x.Key.TableName,
                        RelateIDs = x.Count(),
                        DpType = (int)Searcher.DpType
                    })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);
            }
            else
            {
                query = DC.Set<DataPrivilege>().Where(x=>x.GroupCode != null)
                    .CheckContain(Searcher.Name, x => x.GroupCode)
                    .CheckContain(Searcher.TableName, x => x.TableName)
                       .GroupBy(x => new { x.GroupCode, x.TableName }, x => x.RelateId)
                       .Select(x => new DataPrivilege_ListView
                       {
                           TargetId = x.Key.GroupCode,
                           Name = "",
                           TableName = x.Key.TableName,
                           RelateIDs = x.Count(),
                           DpType = (int)Searcher.DpType
                       })
                    .OrderByDescending(x => x.Name).OrderByDescending(x => x.TableName);

            }
            return query;
        }

        public override void AfterDoSearcher()
        {
            if (Searcher.DpType == DpTypeEnum.User)
            {
                return;
            }
            var groupIDs = EntityList.Select(x=>x.TargetId).ToList();
            Dictionary<string, string> groupdata = new Dictionary<string, string>();
            if (ConfigInfo.HasMainHost && Wtm.LoginUserInfo?.CurrentTenant == null)
            {
                var dd = Wtm.CallAPI<List<ComboSelectListItem>>("mainhost", "/api/_account/GetFrameworkGroups").Result;
                if(dd.Data != null)
                {
                    foreach (var item in dd.Data)
                    {
                        groupdata.TryAdd(item.Value.ToString(), item.Text);
                    }
                }
            }
            else
            {
                var ag = Wtm.GetTenantGroups(Wtm.LoginUserInfo?.CurrentTenant);
                foreach (var item in ag)
                {
                    groupdata.TryAdd(item.GroupCode, item.GroupName);
                }
            }
            foreach (var item in EntityList)
            {
                item.Name = groupdata.ContainsKey(item.TargetId) ? groupdata[item.TargetId] : "";
            }
            base.AfterDoSearcher();
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

        [Display(Name = "_Admin.DataPrivilegeName")]
        public string PName { get; set; }
    }

}
