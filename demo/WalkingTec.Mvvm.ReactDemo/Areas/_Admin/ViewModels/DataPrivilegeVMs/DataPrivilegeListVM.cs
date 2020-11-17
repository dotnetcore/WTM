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

        protected override IEnumerable<IGridColumn<DataPrivilege_ListView>> InitGridHeader()
        {
            return new List<GridColumn<DataPrivilege_ListView>>{
                this.MakeGridHeader(x => x.Name),
                this.MakeGridHeader(x => x.TableName).SetFormat((entity,val)=>GetPrivilegeName(entity)),
                this.MakeGridHeader(x => x.RelateIDs),
                this.MakeGridHeader(x=>x.Edit).SetHeader(Localizer["Operation"]),
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
                    .GroupBy(x => new { x.user.Name, x.user.ID, x.dp.TableName }, x => x.dp.RelateId)
                    .Select(x => new DataPrivilege_ListView
                    {
                        TargetId = x.Key.ID,
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
                    .Join(DC.Set<FrameworkGroup>(), ok => ok.GroupId, ik => ik.ID, (dp, group) => new { dp = dp, group = group })
                    .CheckContain(Searcher.Name, x => x.group.GroupName)
                    .CheckContain(Searcher.TableName, x => x.dp.TableName)
                       .GroupBy(x => new { x.group.GroupName, x.group.ID, x.dp.TableName }, x => x.dp.RelateId)
                       .Select(x => new DataPrivilege_ListView
                       {
                           TargetId = x.Key.ID,
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

    /// <summary>
    /// 如果需要显示树类型的列表需要继承ITreeData`T`接口，并实现Children,Parent,ParentID属性
    /// </summary>
    public class DataPrivilege_ListView : BasePoco
    {
        [Display(Name = "DpTargetName")]
        public string Name { get; set; }
        public Guid TargetId { get; set; }
        [Display(Name = "DataPrivilegeName")]
        public string TableName { get; set; }
        [Display(Name = "DataPrivilegeCount")]
        public int RelateIDs { get; set; }
        public int DpType { get; set; }
        public string DomainName { get; set; }

        public Guid? DomainID { get; set; }

        public string Edit { get; set; }
    }

}
