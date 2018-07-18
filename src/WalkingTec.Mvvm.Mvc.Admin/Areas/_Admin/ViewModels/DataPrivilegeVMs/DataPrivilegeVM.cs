using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public class DataPrivilegeVM : BaseCRUDVM<DataPrivilege>
    {
        public List<ComboSelectListItem> TableNames { get; set; }
        public List<ComboSelectListItem> AllItems { get; set; }
        public List<ComboSelectListItem> AllGroups { get; set; }
        [Display(Name = "允许访问")]
        public List<Guid?> SelectedItemsID { get; set; }
        [Display(Name ="用户")]
        public string UserItCode { get; set; }

        [Display(Name = "权限类别")]
        public DpTypeEnum DpType { get; set; }
        public DataPrivilegeVM()
        {
        }

        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
            AllGroups = DC.Set<FrameworkGroup>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, x => x.GroupName);
            AllItems = new List<ComboSelectListItem>();
            TableNames = ConfigInfo.DataPrivilegeSettings.ToListItems(x => x.PrivillegeName, x => x.ModelName);
            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == Entity.TableName).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(DC, null);
            }
            SelectedItemsID = new List<Guid?>();
            List<Guid?> rids = null;
            if (DpType == DpTypeEnum.User)
            {
                rids = DC.Set<DataPrivilege>().Join(DC.Set<FrameworkUserBase>(), ok => ok.UserId, ik => ik.ID, (dp, user) => new { dp = dp, userid = user.ID }).Where(x => x.dp.TableName == Entity.TableName && x.userid == Entity.UserId).Select(x => x.dp.RelateId).ToList();
            }
            else
            {
                rids = DC.Set<DataPrivilege>().Join(DC.Set<FrameworkGroup>(), ok => ok.GroupId, ik => ik.ID, (dp, group) => new { dp = dp, groupid = group.ID }).Where(x => x.dp.TableName == Entity.TableName && x.groupid == Entity.GroupId).Select(x => x.dp.RelateId).ToList();
            }
            if (rids.Contains(null))
            {
                SelectedItemsID.Add(null);
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
            var dps = ConfigInfo.DataPrivilegeSettings.Where(x => x.ModelName == Entity.TableName).SingleOrDefault();
            if (dps != null)
            {
                AllItems = dps.GetItemList(DC, null);
            }
        }

        public override void Validate()
        {
            if (DpType == DpTypeEnum.User)
            {
                if (string.IsNullOrEmpty(UserItCode))
                {
                    MSD.AddModelError("UserItCode", "用户为必填项");
                }
                else
                {
                    var user = DC.Set<FrameworkUserBase>().Where(x => x.ITCode == UserItCode).FirstOrDefault();
                    if (user == null)
                    {
                        MSD.AddModelError("UserItCode", "无法找到账号为" + UserItCode + "的用户");
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
                    MSD.AddModelError("Entity.GroupId", "用户组为必填项");
                }
            }

            base.Validate();
        }

        public override void DoAdd()
        {
            if (SelectedItemsID == null)
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
            DC.SaveChanges();
        }

        public override void DoEdit(bool updateAllFields = false)
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
            DC.SaveChanges();
        }

        public override void DoDelete()
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
            DC.SaveChanges();
        }
    }
}