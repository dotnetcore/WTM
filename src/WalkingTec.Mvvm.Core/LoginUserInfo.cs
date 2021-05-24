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
            if (string.IsNullOrEmpty(this.ITCode) || context?.DC == null || context.BaseUserQuery == null)
            {
                return;
            }
            var DC = context.DC;
            var userInfo = await context.BaseUserQuery
                                        .Where(x => x.ITCode.ToLower() == this.ITCode.ToLower() && x.IsValid)
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
                ProcessTreeDp(dataPris,context);

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
                this.Roles = roles.Select(x => new SimpleRole { ID = x.ID, RoleCode = x.RoleCode, RoleName = x.RoleName }).ToList();
                this.Groups = groups.Select(x => new SimpleGroup { ID = x.ID, GroupCode = x.GroupCode, GroupName = x.GroupName }).ToList();
                this.DataPrivileges = dataPris.Select(x => new SimpleDataPri { ID = x.ID, RelateId = x.RelateId, TableName = x.TableName, UserCode = x.UserCode, GroupCode = x.GroupCode }).ToList();
                this.FunctionPrivileges = funcPrivileges.Select(x => new SimpleFunctionPri { ID = x.ID, RoleCode = x.RoleCode, Allowed = x.Allowed, MenuItemId = x.MenuItemId }).ToList();
            }
        }

        private void ProcessTreeDp(List<DataPrivilege> dps, WTMContext context)
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
        private IEnumerable<string> GetSubIds(IDataPrivilege dp, List<string> p_id, Type modelType, List<string> skipids,WTMContext context)
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

    }
}
