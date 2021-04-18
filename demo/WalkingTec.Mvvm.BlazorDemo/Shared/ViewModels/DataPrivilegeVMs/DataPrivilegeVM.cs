// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs;
using WalkingTec.Mvvm.Core.Extensions;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public class DataPrivilegeVM : BaseCRUDVM<DataPrivilege>
    {
        public List<ComboSelectListItem> TableNames { get; set; }
        public List<ComboSelectListItem> AllItems { get; set; }
        public List<ComboSelectListItem> AllGroups { get; set; }
        [Display(Name = "_Admin.AllowedDp")]
        public List<string> SelectedItemsID { get; set; }

        [Display(Name = "_Admin.DpType")]
        public DpTypeEnum DpType { get; set; }

        public DpListVM DpList { get; set; }
        [Display(Name = "_Admin.AllDp")]
        public bool? IsAll { get; set; }
        public DataPrivilegeVM()
        {
            DpList = new DpListVM();
            IsAll = false;
        }

        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
            if (ControllerName.Contains("/api") == false)
            {
                AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(Wtm, x => x.GroupName, x=>x.GroupCode);
                TableNames = Wtm.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            }
            SelectedItemsID = new List<string>();
            List<string> rids = null;
            if (DpType == DpTypeEnum.User)
            {
                rids = DC.Set<DataPrivilege>().Where(x => x.TableName == Entity.TableName && x.UserCode == Entity.UserCode).Select(x => x.RelateId).ToList();
            }
            else
            {
                rids = DC.Set<DataPrivilege>().Where(x => x.TableName == Entity.TableName && x.GroupCode == Entity.GroupCode).Select(x => x.RelateId).ToList();
            }
            if (rids.Contains(null))
            {
                IsAll = true;
            }
            else
            {
                SelectedItemsID.AddRange(rids.Select(x => x));
            }

        }

        protected override void ReInitVM()
        {
            TableNames = new List<ComboSelectListItem>();
            AllItems = new List<ComboSelectListItem>();
            TableNames = Wtm.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
        }

        public override void Validate()
        {
            if (DpType == DpTypeEnum.User)
            {
                if (string.IsNullOrEmpty(Entity.UserCode))
                {
                    MSD.AddModelError("Entity.UserCode", Localizer["Validate.{0}required", Localizer["_Admin.Account"]]);
                }
                else
                {
                    var user = DC.Set<FrameworkUser>().Where(x => x.ITCode == Entity.UserCode).FirstOrDefault();
                    if (user == null)
                    {
                        MSD.AddModelError("Entity.UserCode", Localizer["Sys.CannotFindUser", Entity.UserCode]);
                    }
                }
            }
            else
            {
                if(string.IsNullOrEmpty(Entity.GroupCode))
                {
                    MSD.AddModelError("Entity.GroupId", Localizer["Validate.{0}required", Localizer["_Admin.Group"]]);
                }
            }

            base.Validate();
        }

        public override async Task DoAddAsync()
        {
            if (SelectedItemsID == null && IsAll == false)
            {
                return;
            }
            List<Guid> oldIDs = null;

            if (DpType == DpTypeEnum.User)
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserCode == Entity.UserCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupCode == Entity.GroupCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            foreach (var oldid in oldIDs)
            {
                DataPrivilege dp = new DataPrivilege { ID = oldid };
                DC.Set<DataPrivilege>().Attach(dp);
                DC.DeleteEntity(dp);
            }
            if (DpType == DpTypeEnum.User)
            {
                if (IsAll == true)
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.UserCode = Entity.UserCode;
                    dp.TableName = this.Entity.TableName;
                    DC.Set<DataPrivilege>().Add(dp);

                }
                else
                {
                    foreach (var id in SelectedItemsID)
                    {
                        DataPrivilege dp = new DataPrivilege();
                        dp.RelateId = id;
                        dp.UserCode = Entity.UserCode;
                        dp.TableName = this.Entity.TableName;
                        DC.Set<DataPrivilege>().Add(dp);
                    }
                }
            }
            else
            {
                if (IsAll == true)
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.GroupCode = Entity.GroupCode;
                    dp.TableName = this.Entity.TableName;
                    DC.Set<DataPrivilege>().Add(dp);
                }
                else
                {
                    foreach (var id in SelectedItemsID)
                    {
                        DataPrivilege dp = new DataPrivilege();
                        dp.RelateId = id;
                        dp.GroupCode = Entity.GroupCode;
                        dp.TableName = this.Entity.TableName;
                        DC.Set<DataPrivilege>().Add(dp);
                    }
                }
            }
            await DC.SaveChangesAsync();
            if (DpType == DpTypeEnum.User)
            {
                await Wtm.RemoveUserCache(Entity.UserCode);
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode).Select(x => x.UserCode).ToArray();
                await Wtm.RemoveUserCache(userids);
            }

        }

        public override async Task DoEditAsync(bool updateAllFields = false)
        {
            List<Guid> oldIDs = null;

            if (DpType == DpTypeEnum.User)
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserCode == Entity.UserCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupCode == Entity.GroupCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            foreach (var oldid in oldIDs)
            {
                DataPrivilege dp = new DataPrivilege { ID = oldid };
                DC.Set<DataPrivilege>().Attach(dp);
                DC.DeleteEntity(dp);
            }
            if(IsAll == true)
            {
                if (DpType == DpTypeEnum.User)
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.UserCode = Entity.UserCode;
                    dp.TableName = this.Entity.TableName;
                    DC.Set<DataPrivilege>().Add(dp);

                }
                else
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.GroupCode = Entity.GroupCode;
                    dp.TableName = this.Entity.TableName;
                    DC.Set<DataPrivilege>().Add(dp);
                }
            }
            else {
                if (SelectedItemsID != null)
                {
                    if (DpType == DpTypeEnum.User)
                    {
                        foreach (var id in SelectedItemsID)
                        {
                            DataPrivilege dp = new DataPrivilege();
                            dp.RelateId = id;
                            dp.UserCode = Entity.UserCode;
                            dp.TableName = this.Entity.TableName;
                            DC.Set<DataPrivilege>().Add(dp);
                        }

                    }
                    else
                    {
                        foreach (var id in SelectedItemsID)
                        {
                            DataPrivilege dp = new DataPrivilege();
                            dp.RelateId = id;
                            dp.GroupCode = Entity.GroupCode;
                            dp.TableName = this.Entity.TableName;
                            DC.Set<DataPrivilege>().Add(dp);
                        }
                    }
                }
            }
            await DC.SaveChangesAsync();
            if (DpType == DpTypeEnum.User)
            {
                await Wtm.RemoveUserCache(Entity.UserCode);
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode).Select(x => x.UserCode).ToArray();
                await Wtm.RemoveUserCache(userids);
            }
        }

        public override async Task  DoDeleteAsync()
        {
            List<Guid> oldIDs = null;

            if (DpType == DpTypeEnum.User)
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserCode == Entity.UserCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupCode == Entity.GroupCode && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            foreach (var oldid in oldIDs)
            {
                DataPrivilege dp = new DataPrivilege { ID = oldid };
                DC.Set<DataPrivilege>().Attach(dp);
                DC.DeleteEntity(dp);
            }
            await DC.SaveChangesAsync();
            if (DpType == DpTypeEnum.User)
            {
                await Wtm.RemoveUserCache(Entity.UserCode.ToString());
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupCode == Entity.GroupCode).Select(x => x.UserCode).ToArray();
                await Wtm.RemoveUserCache(userids);
            }
        }
    }
}
