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
        [Display(Name = "AllowedDp")]
        public List<string> SelectedItemsID { get; set; }
        [Display(Name ="Account")]
        public string UserItCode { get; set; }

        [Display(Name = "DpType")]
        public DpTypeEnum DpType { get; set; }

        public DpListVM DpList { get; set; }
        [Display(Name = "AllDp")]
        public bool? IsAll { get; set; }
        public DataPrivilegeVM()
        {
            DpList = new DpListVM();
            IsAll = false;
        }

        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
            if (ControllerName.Contains("WalkingTec.Mvvm.Mvc.Admin.Controllers"))
            {
                AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.GroupName);
                TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            }
            SelectedItemsID = new List<string>();
            List<string> rids = null;
            if (DpType == DpTypeEnum.User)
            {
                rids = DC.Set<DataPrivilege>().Where(x => x.TableName == Entity.TableName && x.UserId == Entity.UserId).Select(x => x.RelateId).ToList();
            }
            else
            {
                rids = DC.Set<DataPrivilege>().Where(x => x.TableName == Entity.TableName && x.GroupId == Entity.GroupId).Select(x => x.RelateId).ToList();
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
            TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
        }

        public override void Validate()
        {
            if (DpType == DpTypeEnum.User)
            {
                if (string.IsNullOrEmpty(UserItCode))
                {
                    MSD.AddModelError("UserItCode", Program._localizer["{0}required", Program._localizer["Account"]]);
                }
                else
                {
                    var user = DC.Set<FrameworkUserBase>().Where(x => x.ITCode == UserItCode).FirstOrDefault();
                    if (user == null)
                    {
                        MSD.AddModelError("UserItCode", Program._localizer["CannotFindUser", UserItCode]);
                    }
                    else
                    {
                        Entity.UserId = user.ID;
                    }
                }
            }
            else
            {
                if(Entity.GroupId == null)
                {
                    MSD.AddModelError("Entity.GroupId", Program._localizer["{0}required", Program._localizer["Group"]]);
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
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserId == Entity.UserId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupId == Entity.GroupId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
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
                    dp.UserId = Entity.UserId;
                    dp.TableName = this.Entity.TableName;
                    dp.DomainId = this.Entity.DomainId;
                    DC.Set<DataPrivilege>().Add(dp);

                }
                else
                {
                    foreach (var id in SelectedItemsID)
                    {
                        DataPrivilege dp = new DataPrivilege();
                        dp.RelateId = id;
                        dp.UserId = Entity.UserId;
                        dp.TableName = this.Entity.TableName;
                        dp.DomainId = this.Entity.DomainId;
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
                    dp.GroupId = Entity.GroupId;
                    dp.TableName = this.Entity.TableName;
                    dp.DomainId = this.Entity.DomainId;
                    DC.Set<DataPrivilege>().Add(dp);
                }
                else
                {
                    foreach (var id in SelectedItemsID)
                    {
                        DataPrivilege dp = new DataPrivilege();
                        dp.RelateId = id;
                        dp.GroupId = Entity.GroupId;
                        dp.TableName = this.Entity.TableName;
                        dp.DomainId = this.Entity.DomainId;
                        DC.Set<DataPrivilege>().Add(dp);
                    }
                }
            }
            await DC.SaveChangesAsync();
            if (DpType == DpTypeEnum.User)
            {
                await LoginUserInfo.RemoveUserCache(Entity.UserId.ToString());
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupId == Entity.GroupId).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
            }

        }

        public override async Task DoEditAsync(bool updateAllFields = false)
        {
            List<Guid> oldIDs = null;

            if (DpType == DpTypeEnum.User)
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserId == Entity.UserId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupId == Entity.GroupId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
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
                    dp.UserId = Entity.UserId;
                    dp.TableName = this.Entity.TableName;
                    dp.DomainId = this.Entity.DomainId;
                    DC.Set<DataPrivilege>().Add(dp);

                }
                else
                {
                    DataPrivilege dp = new DataPrivilege();
                    dp.RelateId = null;
                    dp.GroupId = Entity.GroupId;
                    dp.TableName = this.Entity.TableName;
                    dp.DomainId = this.Entity.DomainId;
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
                            dp.UserId = Entity.UserId;
                            dp.TableName = this.Entity.TableName;
                            dp.DomainId = this.Entity.DomainId;
                            DC.Set<DataPrivilege>().Add(dp);
                        }

                    }
                    else
                    {
                        foreach (var id in SelectedItemsID)
                        {
                            DataPrivilege dp = new DataPrivilege();
                            dp.RelateId = id;
                            dp.GroupId = Entity.GroupId;
                            dp.TableName = this.Entity.TableName;
                            dp.DomainId = this.Entity.DomainId;
                            DC.Set<DataPrivilege>().Add(dp);
                        }
                    }
                }
            }
            await DC.SaveChangesAsync();
            if (DpType == DpTypeEnum.User)
            {
                await LoginUserInfo.RemoveUserCache(Entity.UserId.ToString());
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupId == Entity.GroupId).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
            }
        }

        public override async Task  DoDeleteAsync()
        {
            List<Guid> oldIDs = null;

            if (DpType == DpTypeEnum.User)
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.UserId == Entity.UserId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
            }
            else
            {
                oldIDs = DC.Set<DataPrivilege>().Where(x => x.GroupId == Entity.GroupId && x.TableName == this.Entity.TableName).Select(x => x.ID).ToList();
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
                await LoginUserInfo.RemoveUserCache(Entity.UserId.ToString());
            }
            else
            {
                var userids = DC.Set<FrameworkUserGroup>().Where(x => x.GroupId == Entity.GroupId).Select(x => x.UserId.ToString()).ToArray();
                await LoginUserInfo.RemoveUserCache(userids);
            }
        }
    }
}
