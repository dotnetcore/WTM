using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 用户登录信息，需要保存在Session中，所以使用Serializable标记
    /// </summary>
    public class LoginUserInfo
    {
        /// <summary>
        /// 当前租户编码的私有字段，为空时回退使用TenantCode
        /// </summary>
        private string _currentTenant;

        /// <summary>
        /// 用户ID（框架用户表主键的字符串形式）
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 登录用户
        /// </summary>
        public string ITCode { get; set; }

        /// <summary>
        /// 用户所属租户的编码
        /// </summary>
        public string TenantCode { get; set; }

        /// <summary>
        /// 当前租户编码；未单独指定时返回TenantCode
        /// </summary>
        public string CurrentTenant
        {
            get
            {
                return _currentTenant ?? TenantCode;
            }
            set
            {
                _currentTenant = value;
            }
        }

        /// <summary>
        /// 跨系统单点登录使用的远程令牌
        /// </summary>
        public string RemoteToken { get; set; }

        /// <summary>
        /// 用户显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 用户头像附件ID
        /// </summary>
        public Guid? PhotoId { get; set; }

        /// <summary>
        /// 用户拥有的角色列表
        /// </summary>
        public List<SimpleRole> Roles { get; set; }

        /// <summary>
        /// 用户所属的组列表
        /// </summary>
        public List<SimpleGroup> Groups { get; set; }

        /// <summary>
        /// 用户的扩展属性字典（存放自定义用户字段的值，以及Menus/Actions等框架注入的数据）
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// 用户的页面权限列表
        /// </summary>
        public List<SimpleFunctionPri> FunctionPrivileges { get; set; }

        /// <summary>
        /// 用户的数据权限列表
        /// </summary>
        public List<SimpleDataPri> DataPrivileges { get; set; }

        /// <summary>
        /// 登录时间戳，默认取当前时间
        /// </summary>
        public long TimeTick { get; set; } = DateTime.Now.Ticks;

        /// <summary>
        /// 异步加载用户的基本信息，包括角色、组、页面权限和数据权限
        /// </summary>
        /// <param name="context">WTM上下文</param>
        /// <returns>异步任务</returns>
        public async Task LoadBasicInfoAsync(WTMContext context)
        {
            //用户名、DataContext或用户查询为空时无法加载，直接返回
            if (string.IsNullOrEmpty(ITCode) || context?.DC == null || context.BaseUserQuery == null)
            {
                return;
            }
            IDataContext DC = context.DC;
            //获取当前租户下全部组和角色，供后续按用户过滤
            List<SimpleGroup> allgroups = context.GetTenantGroups(TenantCode);
            List<SimpleRole> allroles = context.GetTenantRoles(TenantCode);
            //初始化用户信息
            if (Groups == null || Roles == null)
            {
                //一次性查询用户本身、其角色关联和组关联
                var userInfo = await (from x in context.BaseUserQuery
                                      where x.ITCode == ITCode
                                      select new
                                      {
                                          user = x,
                                          UserRoles = (from frameworkUserRole in DC.Set<FrameworkUserRole>()
                                                       where frameworkUserRole.UserCode == x.ITCode
                                                       select frameworkUserRole.RoleCode).ToList(),
                                          UserGroups = (from frameworkUserGroup in DC.Set<FrameworkUserGroup>()
                                                        where frameworkUserGroup.UserCode == x.ITCode
                                                        select frameworkUserGroup.GroupCode).ToList()
                                      }).AsSingleQuery().FirstOrDefaultAsync();
                if (userInfo != null)
                {
                    List<string> roleIDs = userInfo.UserRoles.ToList();
                    List<string> groupIDs = userInfo.UserGroups.ToList();
                    List<SimpleGroup> groups = allgroups.Where((SimpleGroup x) => groupIDs.Contains(x.GroupCode)).ToList();
                    List<SimpleRole> roles = allroles.Where((SimpleRole x) => roleIDs.Contains(x.RoleCode)).ToList();
                    UserId = userInfo.user.ID.ToString();
                    ITCode = userInfo.user.ITCode;
                    if (string.IsNullOrEmpty(Name))
                    {
                        Name = userInfo.user.Name;
                    }
                    if (!PhotoId.HasValue)
                    {
                        PhotoId = userInfo.user.PhotoId;
                    }
                    if (string.IsNullOrEmpty(TenantCode))
                    {
                        TenantCode = userInfo.user.TenantCode;
                    }
                    if (Attributes == null)
                    {
                        Attributes = new Dictionary<string, object>();
                    }
                    //将自定义用户属性的值写入Attributes字典
                    foreach (PropertyInfo item in context.GlobaInfo.CustomUserProperties)
                    {
                        if (!Attributes.ContainsKey(item.Name))
                        {
                            Attributes.Add(item.Name, item.GetValue(userInfo.user));
                        }
                    }
                    Roles = roles;
                    Groups = groups;
                }
            }
            //展开用户所在组的全部子组（组可能有层级）
            List<SimpleGroup> moregroups = Groups.ToList();
            for (int i = 0; i < moregroups.Count; i++)
            {
                SimpleGroup group = moregroups[i];
                List<SimpleGroup> children = allgroups.Where((SimpleGroup x) => x.ParentId == group.ID).ToList();
                foreach (SimpleGroup child in children)
                {
                    if (!moregroups.Any((SimpleGroup x) => x.ID == child.ID))
                    {
                        moregroups.Add(child);
                    }
                }
            }
            List<string> gc = moregroups.Select((SimpleGroup x) => x.GroupCode).ToList();
            List<string> rc = Roles.Select((SimpleRole x) => x.RoleCode).ToList();
            //查找登录用户的页面权限和数据权限
            List<FunctionPrivilege> funcPrivileges = await (from x in DC.Set<FunctionPrivilege>().AsNoTracking()
                                                            where x.RoleCode != null && rc.Contains(x.RoleCode)
                                                            select x).Distinct().ToListAsync();
            List<DataPrivilege> dataPris = await (from x in DC.Set<DataPrivilege>().AsNoTracking()
                                                  where x.UserCode == ITCode || (x.GroupCode != null && gc.Contains(x.GroupCode))
                                                  select x).Distinct().ToListAsync();
            //对树形结构的数据权限进行展开处理
            ProcessTreeDp(dataPris, context);
            //转换为精简的传输对象后保存到用户信息中
            DataPrivileges = dataPris.Select((DataPrivilege x) => new SimpleDataPri
            {
                ID = x.ID,
                RelateId = x.RelateId,
                TableName = x.TableName,
                UserCode = x.UserCode,
                GroupCode = x.GroupCode
            }).ToList();
            FunctionPrivileges = funcPrivileges.Select((FunctionPrivilege x) => new SimpleFunctionPri
            {
                ID = x.ID,
                RoleCode = x.RoleCode,
                Allowed = x.Allowed,
                MenuItemId = x.MenuItemId
            }).ToList();
        }

        /// <summary>
        /// 对树形结构的数据权限进行展开：若用户对某些树形节点有权限，则其子节点自动获得权限
        /// </summary>
        /// <param name="dps">数据库中查询到的数据权限列表</param>
        /// <param name="context">WTM上下文</param>
        public void ProcessTreeDp(List<DataPrivilege> dps, WTMContext context)
        {
            List<IDataPrivilege> dataPrivilegeSettings = context.DataPrivilegeSettings;
            foreach (IDataPrivilege dp in dataPrivilegeSettings)
            {
                //只处理模型为树形结构（继承TreePoco）的数据权限设置
                if (!typeof(TreePoco).IsAssignableFrom(dp.ModelType))
                {
                    continue;
                }
                List<string> list = (from x in dps
                                     where x.TableName == dp.ModelName
                                     select x.RelateId).ToList();
                if (list.Count > 0 && !list.Contains(null))
                {
                    //获取已处理过的父节点Id，避免重复展开
                    List<string> treeParentIds = dp.GetTreeParentIds(context, dps);
                    List<string> list2 = new List<string>();
                    list2.AddRange(GetSubIds(dp, list, dp.ModelType, treeParentIds, context));
                    list2 = list2.Distinct().ToList();
                    //将展开得到的子节点权限补充进数据权限列表
                    list2.ForEach(delegate (string x)
                    {
                        dps.Add(new DataPrivilege
                        {
                            TableName = dp.ModelName,
                            RelateId = x.ToString()
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 递归获取指定节点的全部子节点Id
        /// </summary>
        /// <param name="dp">数据权限设置</param>
        /// <param name="p_id">需要展开的父节点Id列表</param>
        /// <param name="modelType">数据权限对应的模型类型</param>
        /// <param name="skipids">需要跳过的节点Id列表（已处理过的父节点）</param>
        /// <param name="context">WTM上下文</param>
        /// <returns>全部子节点Id的可枚举集合</returns>
        private IEnumerable<string> GetSubIds(IDataPrivilege dp, List<string> p_id, Type modelType, List<string> skipids, WTMContext context)
        {
            //过滤掉已处理过的节点后再查询其子节点
            List<string> pids = p_id.Where((string x) => !skipids.Contains(x)).ToList();
            List<string> treeSubIds = dp.GetTreeSubIds(context, pids);
            if (treeSubIds.Count > 0)
            {
                //递归展开下一层子节点
                return treeSubIds.Concat(GetSubIds(dp, treeSubIds, modelType, skipids, context));
            }
            return new List<string>();
        }

        /// <summary>
        /// 为API调用准备用户属性：将用户可见的菜单和可访问的Url整理后写入Attributes字典
        /// </summary>
        /// <param name="context">WTM上下文</param>
        public void SetAttributesForApi(WTMContext context)
        {
            List<SimpleMenuApi> list = new List<SimpleMenuApi>();
            List<string> list2 = new List<string>();
            List<SimpleMenu> list3 = null;
            if (!context.ConfigInfo.IsQuickDebug)
            {
                //非快速调试模式直接使用内存中缓存的菜单
                list3 = context.GlobaInfo.AllMenus;
            }
            else
            {
                //快速调试模式下每次从数据库读取菜单
                using IDataContext dataContext = context.CreateDC(isLog: false, "default");
                list3 = dataContext?.Set<FrameworkMenu>().OrderBy((FrameworkMenu x) => x.DisplayOrder).Select((FrameworkMenu x) => new SimpleMenu
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
            //筛选内置页面、纯目录节点和没有对应方法的菜单项
            List<SimpleMenu> list4 = context.GlobaInfo.AllMenus.Where((SimpleMenu x) => x.IsInside == false || x.FolderOnly || string.IsNullOrEmpty(x.MethodName)).ToList();
            //用户有权限的菜单项Id列表
            List<Guid> allowedids = context.LoginUserInfo?.FunctionPrivileges?.Select((SimpleFunctionPri x) => x.MenuItemId).ToList();
            foreach (SimpleMenu item in list4)
            {
                List<Guid> list5 = allowedids;
                //菜单有权限且其父级菜单均设置为显示时，加入用户的菜单列表
                if (list5 != null && list5.Contains(item.ID) && item.IsParentShowOnMenu(list4))
                {
                    list.Add(new SimpleMenuApi
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
            //对菜单文本做多语言处理
            LocalizeMenu(list);
            //收集用户可访问的普通菜单Url
            list2.AddRange((from x in context.GlobaInfo.AllMenus
                            where allowedids.Contains(x.ID) && x.Url != null
                            select x.Url).Distinct());
            //收集标记为忽略权限验证的API模块的Url
            list2.AddRange(from x in context.GlobaInfo.AllModule.Where((SimpleModule x) => x.IsApi).SelectMany((SimpleModule x) => x.Actions)
                           where (x.IgnorePrivillege || x.Module.IgnorePrivillege) && x.Url != null
                           select x.Url);
            if (Attributes == null)
            {
                Attributes = new Dictionary<string, object>();
            }
            //Menus和Actions属性每次重新生成，先移除旧值再写入新值
            if (Attributes.ContainsKey("Menus"))
            {
                Attributes.Remove("Menus");
            }
            if (Attributes.ContainsKey("Actions"))
            {
                Attributes.Remove("Actions");
            }
            Attributes.Add("Menus", list);
            Attributes.Add("Actions", list2);
        }

        /// <summary>
        /// 对菜单文本做多语言处理，以MenuKey.开头的文本替换为本地化资源中的翻译
        /// </summary>
        /// <param name="menus">待处理的菜单列表</param>
        private void LocalizeMenu(List<SimpleMenuApi> menus)
        {
            if (menus == null)
            {
                return;
            }
            foreach (SimpleMenuApi menu in menus)
            {
                string text = menu.Text;
                if (text != null && text.StartsWith("MenuKey."))
                {
                    menu.Text = CoreProgram._localizer[menu.Text];
                }
            }
        }

        /// <summary>
        /// 获取当前用户所在的租户对应的DataContext；无租户时返回默认连接的DataContext
        /// </summary>
        /// <param name="context">WTM上下文</param>
        /// <returns>租户或默认连接的DataContext</returns>
        public IDataContext GetUserDC(WTMContext context)
        {
            if (context?.LoginUserInfo?.CurrentTenant == null)
            {
                return context.CreateDC(isLog: false, "default");
            }
            //按租户编码查找租户信息，使用租户专属的连接串创建DataContext
            FrameworkTenant frameworkTenant = context.GlobaInfo.AllTenant.Where((FrameworkTenant x) => x.TCode == context?.LoginUserInfo?.CurrentTenant).FirstOrDefault();
            if (frameworkTenant != null)
            {
                return frameworkTenant.CreateDC(context);
            }
            return context.CreateDC(isLog: false, "default");
        }
    }
}
