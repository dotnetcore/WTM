// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupMDVM : BaseVM
    {

        public string GroupCode { get; set; }
        public List<GroupDp> DpLists { get; set; }

        public FrameworkGroupMDVM()
        {
        }

        protected override void InitVM()
        {
            DpLists = new List<GroupDp>();
            foreach (var item in Wtm.DataPrivilegeSettings)
            {
                DpListVM list = new DpListVM();
                list.Searcher = new DpSearcher();
                list.Searcher.TableName = item.ModelName;
                DpLists.Add(new GroupDp { DpName = item.PrivillegeName, List = list, SelectedIds = new List<string>() });
            }
            var alldp = DC.Set<DataPrivilege>().Where(x => x.GroupCode == GroupCode).ToList();
            foreach (var item in DpLists)
            {
                var select = alldp.Where(x => x.TableName == item.List.Searcher.TableName).Select(x => x.RelateId).ToList();
                if(select.Count == 0)
                {
                    item.IsAll = null;
                }
                else if (select.Contains(null))
                {
                    item.IsAll = true;
                }
                else
                {
                    item.IsAll = false;
                    item.SelectedIds = select;
                }
            }
        }

        public bool DoChange()
        {
            List<Guid> oldIDs =  DC.Set<DataPrivilege>().Where(x => x.GroupCode == GroupCode).Select(x => x.ID).ToList();
            
            foreach (var oldid in oldIDs)
            {
                DataPrivilege dp = new DataPrivilege { ID = oldid };
                DC.Set<DataPrivilege>().Attach(dp);
                DC.DeleteEntity(dp);
            }
            foreach (var item in DpLists)
            {
                if(item.IsAll == true)
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.GroupCode = GroupCode;
                    dp.TableName = item.List.Searcher.TableName;
                    DC.Set<DataPrivilege>().Add(dp);
                }
                if (item.IsAll == false && item.SelectedIds != null)
                {
                    foreach (var id in item.SelectedIds)
                    {
                        DataPrivilege dp = new DataPrivilege();
                        dp.RelateId = id;
                        dp.GroupCode = GroupCode;
                        dp.TableName = item.List.Searcher.TableName;
                        DC.Set<DataPrivilege>().Add(dp);
                    }

                }
            }
            DC.SaveChanges();
            return true;
        }

    }

    public class GroupDp
    {
        public DpListVM List { get; set; }
        public string DpName { get; set; }

        public List<string> SelectedIds { get; set; }

        public bool? IsAll { get; set; }
    }
}
