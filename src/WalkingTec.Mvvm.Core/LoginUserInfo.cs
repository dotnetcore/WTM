using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NPOI.SS.Formula.Functions;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// 用户登录信息，需要保存在Session中，所以使用Serializable标记
    /// </summary>
    public class LoginUserInfo
    {
        public string UserId { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        public string TenantCode { get; set; }

        private string _currentTenant;
        public string CurrentTenant {
            get { return _currentTenant ?? TenantCode; }
            set { _currentTenant = value; }
        }

        public string RemoteToken { get; set; }

        public string Name { get; set; }

        public string Memo { get; set; }

        public Guid? PhotoId { get; set; }

        public List<SimpleRole> Roles { get; set; }

        public List<SimpleGroup> Groups { get; set; }

        public Dictionary<string, object> Attributes { get; set; }
        /// <summary>
        /// 用户的页面权限列表
        /// </summary>
        public List<SimpleFunctionPri> FunctionPrivileges { get; set; }
        /// <summary>
        /// 用户的数据权限列表
        /// </summary>
        public List<SimpleDataPri> DataPrivileges { get; set; }
        public long TimeTick { get; set; } = DateTime.Now.Ticks;
        public async System.Threading.Tasks.Task LoadBasicInfoAsync(WTMContext context)
        {
            if (string.IsNullOrEmpty(this.ITCode) || context?.DC == null || context.BaseUserQuery == null)
            {
                return;
            }
            var DC = context.DC;
            List<SimpleGroup> allgroups = context.GetTenantGroups(this.TenantCode);
            List<SimpleRole> allroles = context.GetTenantRoles(this.TenantCode);

            if (this.Groups == null || this.Roles == null)
            {
                var userInfo = await context.BaseUserQuery
                                            .Where(x => x.ITCode == ITCode)
                                            .Select(x => new
                                            {
                                                user = x,
                                                UserRoles = DC.Set<FrameworkUserRole>().Where(y => y.UserCode == x.ITCode).Select(x=>x.RoleCode).ToList(),
                                                UserGroups = DC.Set<FrameworkUserGroup>().Where(y => y.UserCode == x.ITCode).Select(x=>x.GroupCode).ToList(),
                                            })
                                            .AsSingleQuery()
                                            .FirstOrDefaultAsync();

                if (userInfo != null)
                {
                    // 初始化用户信息
                    var roleIDs = userInfo.UserRoles.ToList();
                    var groupIDs = userInfo.UserGroups.ToList();
                    List<SimpleGroup> groups = allgroups.Where(x => groupIDs.Contains(x.GroupCode)).ToList();
                    List<SimpleRole>roles = allroles.Where(x => roleIDs.Contains(x.RoleCode)).ToList();
                    this.UserId = userInfo.user.ID.ToString();
                    this.ITCode = userInfo.user.ITCode;
                    if (string.IsNullOrEmpty(this.Name))
                    {
                        this.Name = userInfo.user.Name;
                    }
                    if (this.PhotoId == null)
                    {
                        this.PhotoId = userInfo.user.PhotoId;
                    }
                    if (string.IsNullOrEmpty(this.TenantCode))
                    {
                        this.TenantCode = userInfo.user.TenantCode;
                    }
                    if (Attributes == null)
                    {
                        Attributes = new Dictionary<string, object>();
                    }
                    foreach (var item in context.GlobaInfo.CustomUserProperties)
                    {
                        if (Attributes.ContainsKey(item.Name) == false)
                        {
                            Attributes.Add(item.Name, item.GetValue(userInfo.user));
                        }
                    }
                    this.Roles = roles;
                    this.Groups = groups;
                }
            }

            var moregroups = this.Groups.ToList();
            for (int i = 0; i < moregroups.Count; i++)
            {
                var group = moregroups[i];
                var children = allgroups.Where(x => x.ParentId == group.ID).ToList();
                foreach (var child in children)
                {
                    if (moregroups.Any(x => x.ID == child.ID) == false)
                    {
                        moregroups.Add(child);
                    }
                }
            }
            var gc = moregroups.Select(x => x.GroupCode).ToList();
            var rc = this.Roles.Select(x=>x.RoleCode).ToList();

            //查找登录用户的页面权限
            var funcPrivileges = await DC.Set<FunctionPrivilege>().AsNoTracking()
                .Where(x => x.RoleCode != null && rc.Contains(x.RoleCode))
                .Distinct()
                .ToListAsync();
            var dataPris = await DC.Set<DataPrivilege>().AsNoTracking()
                .Where(x => x.UserCode == this.ITCode || (x.GroupCode != null && gc.Contains(x.GroupCode)))
                .Distinct()
                .ToListAsync();
            ProcessTreeDp(dataPris,context);
            this.DataPrivileges = dataPris.Select(x => new SimpleDataPri { ID = x.ID, RelateId = x.RelateId, TableName = x.TableName, UserCode = x.UserCode, GroupCode = x.GroupCode }).ToList();
            this.FunctionPrivileges = funcPrivileges.Select(x => new SimpleFunctionPri { ID = x.ID, RoleCode = x.RoleCode, Allowed = x.Allowed, MenuItemId = x.MenuItemId }).ToList();
        }

        public void ProcessTreeDp(List<DataPrivilege> dps,WTMContext context)
        {
            var dpsSetting = context.DataPrivilegeSettings;
            foreach (var dp in dpsSetting)
            {
                if (typeof(TreePoco).IsAssignableFrom(dp.ModelType))
                {
                    var ids = dps.Where(x => x.TableName == dp.ModelName).Select(x => x.RelateId).ToList();
                    if (ids.Count > 0 && ids.Contains(null) == false)
                    {
                        var skipids = dp.GetTreeParentIds(context, dps);
                        List<string> subids = new List<string>();
                        subids.AddRange(GetSubIds(dp, ids, dp.ModelType, skipids,context));
                        subids = subids.Distinct().ToList();
                        subids.ForEach(x => dps.Add(new DataPrivilege
                        {
                            TableName = dp.ModelName,
                            RelateId = x.ToString()
                        }));
                    }
                }
            }
        }

        private IEnumerable<string> GetSubIds(IDataPrivilege dp, List<string> p_id, Type modelType, List<string> skipids, WTMContext context)
        {
            var ids = p_id.Where(x => skipids.Contains(x) == false).ToList();
            var subids = dp.GetTreeSubIds(context, ids);
            if (subids.Count > 0)
            {
                return subids.Concat(GetSubIds(dp, subids, modelType, skipids,context));
            }
            else
            {
                return new List<string>();
            }
        }

        public void SetAttributesForApi(WTMContext context)
        {
            var ms = new List<SimpleMenuApi>();
            List<string> urls = new List<string>();
            List<SimpleMenu> menudata = null;

            if (context.ConfigInfo.IsQuickDebug == false)
            {
                menudata = context.GlobaInfo.AllMenus;
            }
            else
            {
                using (var dc = context.CreateDC(false,"default"))
                {
                    menudata = dc?.Set<FrameworkMenu>()
                            .OrderBy(x => x.DisplayOrder)
                            .Select(x => new SimpleMenu
                            {
                                ID = x.ID,
                                ParentId = x.ParentId,
                                PageName = x.PageName,
                                Url = x.Url,
                                DisplayOrder = x.DisplayOrder,
                                ShowOnMenu = x.ShowOnMenu,
                                Icon = x.Icon,
                                IsPublic = x.IsPublic,
                                FolderOnly = x.FolderOnly,
                                MethodName = x.MethodName,
                                IsInside = x.IsInside,
                                TenantAllowed = x.TenantAllowed
                            })
                            .ToList();
                }
            }
            var topdata = context.GlobaInfo.AllMenus.Where(x =>x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName)).ToList();
            var allowedids = context.LoginUserInfo?.FunctionPrivileges?.Select(x => x.MenuItemId).ToList();
            foreach (var item in topdata)
            {
                if (allowedids?.Contains(item.ID) == true && item.IsParentShowOnMenu(topdata))
                {
                    ms.Add(new SimpleMenuApi
                    {
                        Id = item.ID.ToString().ToLower(),
                        ParentId = item.ParentId?.ToString()?.ToLower(),
                        Text = item.PageName,
                        Url = item.Url,
                        Icon = item.Icon,
                        ShowOnMenu = item.ShowOnMenu
                    });
                }
            }

            LocalizeMenu(ms);

            urls.AddRange(context.GlobaInfo.AllMenus.Where(x => allowedids.Contains(x.ID) && x.Url != null).Select(x => x.Url).Distinct());
            urls.AddRange(context.GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));

            if (this.Attributes == null)
            {
                this.Attributes = new Dictionary<string, object>();
            }
            if (this.Attributes.ContainsKey("Menus"))
            {
                this.Attributes.Remove("Menus");
            }
            if (this.Attributes.ContainsKey("Actions"))
            {
                this.Attributes.Remove("Actions");
            }
            this.Attributes.Add("Menus", ms);
            this.Attributes.Add("Actions", urls);
        }

        private void LocalizeMenu(List<SimpleMenuApi> menus)
        {
            if (menus == null)
            {
                return;
            }
            foreach (var menu in menus)
            {
                if (menu.Text?.StartsWith("MenuKey.") == true)
                {
                    menu.Text = CoreProgram._localizer[menu.Text];
                }
            }
        }

        public IDataContext GetUserDC(WTMContext context)
        {
            if (context?.LoginUserInfo?.CurrentTenant == null)
            {
                return context.CreateDC(cskey: "default");
            }
            else
            {
                var item = context.GlobaInfo.AllTenant.Where(x => x.TCode == context?.LoginUserInfo?.CurrentTenant).FirstOrDefault();
                if (item != null)
                {
                    return item.CreateDC(context);
                }
                else
                {
                    return context.CreateDC(cskey: "default");

                }
            }
        }
    }
}
