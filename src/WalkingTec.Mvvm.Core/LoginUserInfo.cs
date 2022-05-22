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

        public async System.Threading.Tasks.Task LoadBasicInfoAsync(WTMContext context)
        {
            if (string.IsNullOrEmpty(this.UserId) || context?.DC == null || context.BaseUserQuery == null)
            {
                return;
            }
            var DC = context.DC;
            Guid userid = Guid.Empty;
            Guid.TryParse(this.UserId, out userid);
            var userInfo = await context.BaseUserQuery
                                        .Where(x => x.ID == userid && x.IsValid)
                                        .Select(x => new {
                                            user = x,
                                            UserRoles = DC.Set<FrameworkUserRole>().Where(y => y.UserCode == x.ITCode).ToList(),
                                            UserGroups = DC.Set<FrameworkUserGroup>().Where(y => y.UserCode == x.ITCode).ToList(),
                                        })
                                        .FirstOrDefaultAsync();

            if (userInfo != null)
            {
                // 初始化用户信息
                var roleIDs = userInfo.UserRoles.Select(x => x.RoleCode).ToList();
                var groupIDs = userInfo.UserGroups.Select(x => x.GroupCode).ToList();


                var dataPris = await DC.Set<DataPrivilege>().AsNoTracking()
                                .Where(x => x.UserCode == userInfo.user.ITCode || (x.GroupCode != null && groupIDs.Contains(x.GroupCode)))
                                .Distinct()
                                .ToListAsync();
               context.ProcessTreeDp(dataPris);

                //查找登录用户的页面权限
                var funcPrivileges = await DC.Set<FunctionPrivilege>().AsNoTracking()
                    .Where(x => x.RoleCode != null && roleIDs.Contains(x.RoleCode))
                    .Distinct()
                    .ToListAsync();

                var roles = DC.Set<FrameworkRole>().AsNoTracking().Where(x => roleIDs.Contains(x.RoleCode)).ToList();
                var groups = DC.Set<FrameworkGroup>().AsNoTracking().Where(x => groupIDs.Contains(x.GroupCode)).ToList();
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
                if(Attributes == null)
                {
                    Attributes = new Dictionary<string, object>();
                }
                this.Roles = roles.Select(x => new SimpleRole { ID = x.ID, RoleCode = x.RoleCode, RoleName = x.RoleName }).ToList();
                this.Groups = groups.Select(x => new SimpleGroup { ID = x.ID, GroupCode = x.GroupCode, GroupName = x.GroupName }).ToList();
                this.DataPrivileges = dataPris.Select(x => new SimpleDataPri { ID = x.ID, RelateId = x.RelateId, TableName = x.TableName, UserCode = x.UserCode, GroupCode = x.GroupCode }).ToList();
                this.FunctionPrivileges = funcPrivileges.Select(x => new SimpleFunctionPri { ID = x.ID, RoleCode = x.RoleCode, Allowed = x.Allowed, MenuItemId = x.MenuItemId }).ToList();
            }
        }

        public void SetAttributesForApi(WTMContext context)
        {
            var ms = new List<SimpleMenuApi>();
            List<string> urls = new List<string>();
            if (context.ConfigInfo.IsQuickDebug == false)
            {
                var topdata = context.GlobaInfo.AllMenus.Where(x => x.ShowOnMenu && (x.IsInside == false || x.FolderOnly == true || string.IsNullOrEmpty(x.MethodName))).ToList();
                var allowedids = context.LoginUserInfo.FunctionPrivileges.Select(x => x.MenuItemId).ToList();
                foreach (var item in topdata)
                {
                    if (allowedids.Contains(item.ID) && item.IsParentShowOnMenu(topdata))
                    {
                        ms.Add(new SimpleMenuApi
                        {
                            Id = item.ID.ToString().ToLower(),
                            ParentId = item.ParentId?.ToString()?.ToLower(),
                            Text = item.PageName,
                            Url = item.Url,
                            Icon = item.Icon
                        });
                    }
                }

                LocalizeMenu(ms);

                urls.AddRange(context.GlobaInfo.AllMenus.Where(x => allowedids.Contains(x.ID) && x.Url != null).Select(x => x.Url).Distinct());
                urls.AddRange(context.GlobaInfo.AllModule.Where(x => x.IsApi == true).SelectMany(x => x.Actions).Where(x => (x.IgnorePrivillege == true || x.Module.IgnorePrivillege == true) && x.Url != null).Select(x => x.Url));
            }
            if(this.Attributes == null)
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
            if (context?.LoginUserInfo?.TenantCode == null)
            {
                return context.CreateDC(cskey: "default");
            }
            else
            {
                var item = context.GlobaInfo.AllTenant.Where(x => x.TCode == context?.LoginUserInfo?.TenantCode).FirstOrDefault();
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
