using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkContext
    /// </summary>
    public partial class FrameworkContext : EmptyContext, IDataContext
    {
        public DbSet<FrameworkMenu> BaseFrameworkMenus { get; set; }
        public DbSet<FunctionPrivilege> BaseFunctionPrivileges { get; set; }
        public DbSet<DataPrivilege> BaseDataPrivileges { get; set; }
        public DbSet<FileAttachment> BaseFileAttachments { get; set; }
        public DbSet<FrameworkUserBase> BaseFrameworkUsers { get; set; }
        public DbSet<FrameworkRole> BaseFrameworkRoles { get; set; }
        public DbSet<FrameworkGroup> BaseFrameworkGroups { get; set; }
        public DbSet<ActionLog> BaseActionLogs { get; set; }
        //public DbSet<FrameworkArea> BaseFrameworkAreas { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        public FrameworkContext() : base()
        {
        }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        /// <param name="cs"></param>
        public FrameworkContext(string cs) : base(cs)
        {
        }

        public FrameworkContext(string cs, DBTypeEnum dbtype, string version = null) : base(cs, dbtype, version)
        {
        }

        public FrameworkContext(CS cs) : base(cs)
        {
        }
        public FrameworkContext(DbContextOptions<FrameworkContext> options) : base(options) { }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //菜单和菜单权限的级联删除
            modelBuilder.Entity<FunctionPrivilege>().HasOne(x => x.MenuItem).WithMany(x => x.Privileges).HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Cascade);
            //用户和用户搜索条件级联删除
            modelBuilder.Entity<SearchCondition>().HasOne(x => x.User).WithMany(x => x.SearchConditions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DataPrivilege>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DataPrivilege>().HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FrameworkUserBase>().HasIndex(x => x.ITCode).IsUnique();
            base.OnModelCreating(modelBuilder);
        }


        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async override Task<bool> DataInit(object allModules, bool IsSpa)
        {
            bool rv = await Database.EnsureCreatedAsync();
            //判断是否存在初始数据
            bool emptydb = false;

            try
            {
                emptydb = Set<FrameworkUserBase>().Count() == 0 && Set<FrameworkUserRole>().Count() == 0 && Set<FrameworkMenu>().Count() == 0;
            }
            catch { }

            if (emptydb == true)
            {
                var AllModules = allModules as List<FrameworkModule>;
                var roles = new FrameworkRole[]
                {
                    new FrameworkRole{ID=Guid.NewGuid(), RoleCode = "001", RoleName = Program._localizer["Admin"]}
                };
                var users = new FrameworkUserBase[]
                {
                    new FrameworkUserBase{ID=Guid.NewGuid(), ITCode = "admin", Password = Utils.GetMD5String("000000"), IsValid = true, Name=Program._localizer["Admin"]}
                };
                var userroles = new FrameworkUserRole[]
                {
                    new FrameworkUserRole{ User = users[0], Role = roles[0]}
                };

                var adminRole = roles[0];
                if (Set<FrameworkMenu>().Any() == false)
                {
                    var systemManagement = GetFolderMenu("SystemManagement", new List<FrameworkRole> { adminRole }, null);
                    var logList = IsSpa ? GetMenu2(AllModules, "ActionLog", new List<FrameworkRole> { adminRole }, null, 1) : GetMenu(AllModules, "_Admin", "ActionLog", "Index", new List<FrameworkRole> { adminRole }, null, 1);
                    var userList = IsSpa ? GetMenu2(AllModules, "FrameworkUser", new List<FrameworkRole> { adminRole }, null, 2) : GetMenu(AllModules, "_Admin", "FrameworkUser", "Index", new List<FrameworkRole> { adminRole }, null, 2);
                    var roleList = IsSpa ? GetMenu2(AllModules, "FrameworkRole", new List<FrameworkRole> { adminRole }, null, 3) : GetMenu(AllModules, "_Admin", "FrameworkRole", "Index", new List<FrameworkRole> { adminRole }, null, 3);
                    var groupList = IsSpa ? GetMenu2(AllModules, "FrameworkGroup", new List<FrameworkRole> { adminRole }, null, 4) : GetMenu(AllModules, "_Admin", "FrameworkGroup", "Index", new List<FrameworkRole> { adminRole }, null, 4);
                    var menuList = IsSpa ? GetMenu2(AllModules, "FrameworkMenu", new List<FrameworkRole> { adminRole }, null, 5) : GetMenu(AllModules, "_Admin", "FrameworkMenu", "Index", new List<FrameworkRole> { adminRole }, null, 5);
                    var dpList = IsSpa ? GetMenu2(AllModules, "DataPrivilege", new List<FrameworkRole> { adminRole }, null, 6) : GetMenu(AllModules, "_Admin", "DataPrivilege", "Index", new List<FrameworkRole> { adminRole }, null, 6);
                    if (logList != null)
                    {
                        var menus = new FrameworkMenu[] { logList, userList, roleList, groupList, menuList, dpList };
                        foreach (var item in menus)
                        {
                            if(item != null)
                            {
                                systemManagement.Children.Add(item);
                            }
                        }
                        Set<FrameworkMenu>().Add(systemManagement);

                        if (IsSpa == false)
                        {
                            systemManagement.ICon = "layui-icon layui-icon-set";
                            logList.ICon = "layui-icon layui-icon-form";
                            userList.ICon = "layui-icon layui-icon-friends";
                            roleList.ICon = "layui-icon layui-icon-user";
                            groupList.ICon = "layui-icon layui-icon-group";
                            menuList.ICon = "layui-icon layui-icon-menu-fill";
                            dpList.ICon = "layui-icon layui-icon-auz";

                            var apifolder = GetFolderMenu("Api", new List<FrameworkRole> { adminRole }, null);
                            apifolder.ShowOnMenu = false;
                            apifolder.DisplayOrder = 100;
                            var logList2 = GetMenu2(AllModules, "ActionLog", new List<FrameworkRole> { adminRole }, null, 1);
                            var userList2 = GetMenu2(AllModules, "FrameworkUser", new List<FrameworkRole> { adminRole }, null, 2);
                            var roleList2 = GetMenu2(AllModules, "FrameworkRole", new List<FrameworkRole> { adminRole }, null, 3);
                            var groupList2 = GetMenu2(AllModules, "FrameworkGroup", new List<FrameworkRole> { adminRole }, null, 4);
                            var menuList2 = GetMenu2(AllModules, "FrameworkMenu", new List<FrameworkRole> { adminRole }, null, 5);
                            var dpList2 = GetMenu2(AllModules, "DataPrivilege", new List<FrameworkRole> { adminRole }, null, 6);
                            var apis = new FrameworkMenu[] { logList2, userList2, roleList2, groupList2, menuList2, dpList2 };
                            //apis.ToList().ForEach(x => { x.ShowOnMenu = false;x.PageName += $"({Program._localizer["BuildinApi"]})"; });
                            foreach (var item in apis)
                            {
                                if(item != null)
                                {
                                    apifolder.Children.Add(item);

                                }
                            }
                            Set<FrameworkMenu>().Add(apifolder);
                        }
                        else
                        {
                            systemManagement.ICon = " _wtmicon _wtmicon-icon_shezhi";
                            logList.ICon = " _wtmicon _wtmicon-chaxun";
                            userList.ICon = " _wtmicon _wtmicon-zhanghaoquanxianguanli";
                            roleList.ICon = " _wtmicon _wtmicon-quanxianshenpi";
                            groupList.ICon = " _wtmicon _wtmicon-zuzhiqunzu";
                            menuList.ICon = " _wtmicon _wtmicon--lumingpai";
                            dpList.ICon = " _wtmicon _wtmicon-anquan";

                        }
                    }

                }
                Set<FrameworkRole>().AddRange(roles);
                Set<FrameworkUserBase>().AddRange(users);
                Set<FrameworkUserRole>().AddRange(userroles);
                await SaveChangesAsync();
            }
            return rv;
        }

        private FrameworkMenu GetFolderMenu(string FolderText, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, bool isShowOnMenu = true, bool isInherite = false)
        {
            FrameworkMenu menu = new FrameworkMenu
            {
                PageName = "MenuKey." + FolderText,
                Children = new List<FrameworkMenu>(),
                Privileges = new List<FunctionPrivilege>(),
                ShowOnMenu = isShowOnMenu,
                IsInside = true,
                FolderOnly = true,
                IsPublic = false,
                CreateTime = DateTime.Now,
                DisplayOrder = 1
            };

            if (allowedRoles != null)
            {
                foreach (var role in allowedRoles)
                {
                    menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });

                }
            }
            if (allowedUsers != null)
            {
                foreach (var user in allowedUsers)
                {
                    menu.Privileges.Add(new FunctionPrivilege { UserId = user.ID, Allowed = true });
                }
            }

            return menu;
        }

        private FrameworkMenu GetMenu(List<FrameworkModule> allModules, string areaName, string controllerName, string actionName, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, int displayOrder)
        {
            var acts = allModules.Where(x => x.ClassName == controllerName && (areaName == null || x.Area?.Prefix?.ToLower() == areaName.ToLower())).SelectMany(x => x.Actions).ToList();
            var act = acts.Where(x => x.MethodName == actionName).SingleOrDefault();
            var rest = acts.Where(x => x.MethodName != actionName && x.IgnorePrivillege == false).ToList();
            FrameworkMenu menu = GetMenuFromAction(act, true, allowedRoles, allowedUsers, displayOrder);
            if (menu != null)
            {
                for (int i = 0; i < rest.Count; i++)
                {
                    if (rest[i] != null)
                    {
                        menu.Children.Add(GetMenuFromAction(rest[i], false, allowedRoles, allowedUsers, (i + 1)));
                    }
                }
            }
            return menu;
        }

        private FrameworkMenu GetMenu2(List<FrameworkModule> allModules, string controllerName, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, int displayOrder)
        {
            var acts = allModules.Where(x => x.FullName == $"WalkingTec.Mvvm.Admin.Api,{controllerName}" && x.IsApi == true).SelectMany(x => x.Actions).ToList();
            var rest = acts.Where(x => x.IgnorePrivillege == false).ToList();
            FrameworkAction act = null;
            if(acts.Count > 0)
            {
                act = acts[0];
            }
            FrameworkMenu menu = GetMenuFromAction(act, true, allowedRoles, allowedUsers, displayOrder);
            if (menu != null)
            {
                menu.Url = "/" + acts[0].Module.ClassName.ToLower();
                menu.ModuleName = menu.ModuleName;
                menu.PageName = menu.ModuleName;
                menu.ActionName = "MainPage";
                menu.ClassName = acts[0].Module.FullName;
                menu.MethodName = null;
                for (int i = 0; i < rest.Count; i++)
                {
                    if (rest[i] != null)
                    {
                        menu.Children.Add(GetMenuFromAction(rest[i], false, allowedRoles, allowedUsers, (i + 1)));
                    }
                }
            }
            return menu;
        }

        private FrameworkMenu GetMenuFromAction(FrameworkAction act, bool isMainLink, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, int displayOrder = 1)
        {
            if (act == null)
            {
                return null;
            }
            FrameworkMenu menu = new FrameworkMenu
            {
                //ActionId = act.ID,
                //ModuleId = act.ModuleId,
                ClassName = act.Module.FullName,
                MethodName = act.MethodName,
                Url = act.Url,
                Privileges = new List<FunctionPrivilege>(),
                ShowOnMenu = isMainLink,
                FolderOnly = false,
                Children = new List<FrameworkMenu>(),
                IsPublic = false,
                IsInside = true,
                DisplayOrder = displayOrder,
                CreateTime = DateTime.Now
            };
            if (isMainLink)
            {
                menu.PageName = "MenuKey." + act.Module.ActionDes?.Description;
                menu.ModuleName = "MenuKey." + act.Module.ActionDes?.Description;
                menu.ActionName = act.ActionDes?.Description ?? act.ActionName;
                menu.MethodName = null;
            }
            else
            {
                menu.PageName = "MenuKey." + act.ActionDes?.Description;
                menu.ModuleName = "MenuKey." + act.ActionDes?.Description;
                menu.ActionName = act.ActionDes?.Description ?? act.ActionName;
            }
            if (allowedRoles != null)
            {
                foreach (var role in allowedRoles)
                {
                    menu.Privileges.Add(new FunctionPrivilege { RoleId = role.ID, Allowed = true });

                }
            }
            if (allowedUsers != null)
            {
                foreach (var user in allowedUsers)
                {
                    menu.Privileges.Add(new FunctionPrivilege { UserId = user.ID, Allowed = true });
                }
            }
            return menu;
        }

    }

    public partial class EmptyContext : DbContext, IDataContext
    {
        /// <summary>
        /// Commited
        /// </summary>
        public bool Commited { get; set; }

        /// <summary>
        /// IsFake
        /// </summary>
        public bool IsFake { get; set; }

        /// <summary>
        /// CSName
        /// </summary>
        public string CSName { get; set; }

        public DBTypeEnum DBType { get; set; }

        public string Version { get; set; }
        public CS ConnectionString { get; set; }
        /// <summary>
        /// FrameworkContext
        /// </summary>
        public EmptyContext()
        {
            CSName = "default";
            DBType = DBTypeEnum.SqlServer;
        }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        /// <param name="cs"></param>
        public EmptyContext(string cs)
        {
            CSName = cs;
        }

        public EmptyContext(string cs, DBTypeEnum dbtype, string version = null)
        {
            CSName = cs;
            DBType = dbtype;
            Version = version;
        }

        public EmptyContext(CS cs)
        {
            CSName = cs.Value;
            DBType = cs.DbType.Value;
            Version = cs.Version;
            ConnectionString = cs;
        }

        public EmptyContext(DbContextOptions<FrameworkContext> options) : base(options) { }

        public IDataContext CreateNew()
        {
            if (ConnectionString != null)
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(CS) }).Invoke(new object[] { ConnectionString }); ;
            }
            else
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum), typeof(string) }).Invoke(new object[] { CSName, DBType, Version });
            }
        }

        public IDataContext ReCreate()
        {
            if (ConnectionString != null)
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(CS) }).Invoke(new object[] { ConnectionString }); ;
            }
            else
            {
                return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum), typeof(string) }).Invoke(new object[] { CSName, DBType, Version });
            }
        }
        /// <summary>
        /// 将一个实体设为填加状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void AddEntity<T>(T entity) where T : TopBasePoco
        {
            this.Entry(entity).State = EntityState.Added;
        }

        /// <summary>
        /// 将一个实体设为修改状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void UpdateEntity<T>(T entity) where T : TopBasePoco
        {
            this.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 将一个实体的某个字段设为修改状态，用于只更新个别字段的情况
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <param name="fieldExp">要设定为修改状态的字段</param>
        public void UpdateProperty<T>(T entity, Expression<Func<T, object>> fieldExp)
            where T : TopBasePoco
        {
            var set = this.Set<T>();
            if (set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault() == null)
            {
                set.Attach(entity);
            }
            this.Entry(entity).Property(fieldExp).IsModified = true;
        }

        /// <summary>
        /// UpdateProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="fieldName"></param>
        public void UpdateProperty<T>(T entity, string fieldName)
            where T : TopBasePoco
        {
            var set = this.Set<T>();
            if (set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault() == null)
            {
                set.Attach(entity);
            }
            this.Entry(entity).Property(fieldName).IsModified = true;
        }

        /// <summary>
        /// 将一个实体设定为删除状态
        /// </summary>
        /// <param name="entity">实体</param>
        public void DeleteEntity<T>(T entity) where T : TopBasePoco
        {
            var set = this.Set<T>();
            var exist = set.Local.AsQueryable().CheckID(entity.GetID()).FirstOrDefault();
            if (exist == null)
            {
                set.Attach(entity);
                set.Remove(entity);
            }
            else
            {
                set.Remove(exist);

            }
        }

        /// <summary>
        /// CascadeDelete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void CascadeDelete<T>(T entity) where T : TopBasePoco, ITreeData<T>
        {
            if (entity != null && entity.ID != Guid.Empty)
            {
                var set = this.Set<T>();
                var entities = set.Where(x => x.ParentId == entity.ID).ToList();
                if (entities.Count > 0)
                {
                    foreach (var item in entities)
                    {
                        CascadeDelete(item);
                    }
                }
                DeleteEntity(entity);
            }
        }

        /// <summary>
        /// GetCoreType
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Type GetCoreType(Type t)
        {
            if (t != null && t.IsNullable())
            {
                if (!t.GetTypeInfo().IsValueType)
                {
                    return t;
                }
                else
                {
                    if ("DateTime".Equals(t.GenericTypeArguments[0].Name))
                    {
                        return typeof(string);
                    }
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                if ("DateTime".Equals(t.Name))
                {
                    return typeof(string);
                }
                return t;
            }
        }

        /// <summary>
        /// OnModelCreating
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if(DBType == DBTypeEnum.Oracle)
            {
                modelBuilder.Model.Relational().MaxIdentifierLength = 30;
            }
        }

        /// <summary>
        /// OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (DBType)
            {
                case DBTypeEnum.SqlServer:
                    try
                    {
                        var Configs = GlobalServices.GetRequiredService<Configs>();
                        if (Configs.IsOldSqlServer == true)
                        {
                            optionsBuilder.UseSqlServer(CSName, op => op.UseRowNumberForPaging());
                        }
                        else
                        {
                            optionsBuilder.UseSqlServer(CSName);
                        }
                    }
                    catch
                    {
                        optionsBuilder.UseSqlServer(CSName, op => op.UseRowNumberForPaging());
                    }
                    break;
                case DBTypeEnum.MySql:
                    optionsBuilder.UseMySql(CSName, mySqlOptions =>
                    {
                        if (string.IsNullOrEmpty(Version) == false)
                        {
                            mySqlOptions.ServerVersion(Version);
                        }
                    });
                    break;
                case DBTypeEnum.PgSql:
                    optionsBuilder.UseNpgsql(CSName);
                    break;
                case DBTypeEnum.Memory:
                    optionsBuilder.UseInMemoryDatabase(CSName);
                    break;
                case DBTypeEnum.SQLite:
                    optionsBuilder.UseSqlite(CSName);
                    break;
                case DBTypeEnum.Oracle:
                    
                    optionsBuilder.UseOracle(CSName, option=> {
                        if (string.IsNullOrEmpty(Version) == false)
                        {
                            option.UseOracleSQLCompatibility(Version);
                        }
                        else
                        {
                            option.UseOracleSQLCompatibility("11");
                        }
                    });
                    break;
                default:
                    break;
            }
            try
            {
                var Configs = GlobalServices.GetRequiredService<Configs>();//如果是debug模式,将EF生成的sql语句输出到debug输出
                if (Configs.IsQuickDebug)
                {
                    optionsBuilder.EnableDetailedErrors();
                    optionsBuilder.EnableSensitiveDataLogging();
                    optionsBuilder.UseLoggerFactory(LoggerFactory);
                }
            }
            catch { }
            base.OnConfiguring(optionsBuilder);
        }

        public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new ILoggerProvider[] {
            new DebugLoggerProvider(),
            new ConsoleLoggerProvider(GlobalServices.GetRequiredService<IOptionsMonitor<ConsoleLoggerOptions>>())
        }, GlobalServices.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>());

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true表示需要进行初始化数据操作，返回false即数据库已经存在或不需要初始化数据</returns>
        public async virtual Task<bool> DataInit(object allModules, bool IsSpa)
        {
            bool rv = await Database.EnsureCreatedAsync();
            return rv;
        }

        #region 执行存储过程返回datatable
        /// <summary>
        /// 执行存储过程，返回datatable结果集
        /// </summary>
        /// <param name="command">存储过程名称</param>
        /// <param name="paras">存储过程参数</param>
        /// <returns></returns>
        public DataTable RunSP(string command, params object[] paras)
        {
            return Run(command, CommandType.StoredProcedure, paras);
        }
        #endregion

        public IEnumerable<TElement> RunSP<TElement>(string command, params object[] paras)
        {
            return Run<TElement>(command, CommandType.StoredProcedure, paras);
        }

        #region 执行Sql语句，返回datatable
        public DataTable RunSQL(string sql, params object[] paras)
        {
            return Run(sql, CommandType.Text, paras);
        }
        #endregion

        public IEnumerable<TElement> RunSQL<TElement>(string sql, params object[] paras)
        {
            return Run<TElement>(sql, CommandType.Text, paras);
        }


        #region 执行存储过程或Sql语句返回DataTable
        /// <summary>
        /// 执行存储过程或Sql语句返回DataTable
        /// </summary>
        /// <param name="sql">存储过程名称或Sql语句</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public DataTable Run(string sql, CommandType commandType, params object[] paras)
        {
            DataTable table = new DataTable();
            switch (this.DBType)
            {
                case DBTypeEnum.SqlServer:
                    SqlConnection con = this.Database.GetDbConnection() as SqlConnection;
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        adapter.SelectCommand = cmd;
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        adapter.Fill(table);
                        adapter.SelectCommand.Parameters.Clear();
                    }
                    break;
                case DBTypeEnum.MySql:
                    MySqlConnection mySqlCon = this.Database.GetDbConnection() as MySqlConnection;
                    using (MySqlCommand cmd = new MySqlCommand(sql, mySqlCon))
                    {
                        if (mySqlCon.State == ConnectionState.Closed)
                        {
                            mySqlCon.Open();
                        }
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        MySqlDataReader dr = cmd.ExecuteReader();
                        table.Load(dr);
                        dr.Close();
                        mySqlCon.Close();
                    }
                    break;
                case DBTypeEnum.PgSql:
                    Npgsql.NpgsqlConnection npgCon = this.Database.GetDbConnection() as Npgsql.NpgsqlConnection;
                    using (Npgsql.NpgsqlCommand cmd = new Npgsql.NpgsqlCommand(sql, npgCon))
                    {
                        if (npgCon.State == ConnectionState.Closed)
                        {
                            npgCon.Open();
                        }
                        cmd.CommandTimeout = 2400;
                        cmd.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                cmd.Parameters.Add(param);
                        }
                        Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                        table.Load(dr);
                        dr.Close();
                        npgCon.Close();
                    }
                    break;
                case DBTypeEnum.SQLite:
                case DBTypeEnum.Oracle:
                    var connection = this.Database.GetDbConnection();
                    var isClosed = connection.State == ConnectionState.Closed;
                    if (isClosed)
                    {
                        connection.Open();
                    }
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.CommandTimeout = 2400;
                        command.CommandType = commandType;
                        if (paras != null)
                        {
                            foreach (var param in paras)
                                command.Parameters.Add(param);
                        }
                        using (var reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                    if (isClosed)
                    {
                        connection.Close();
                    }
                    break;
            }
            return table;
        }
        #endregion


        public IEnumerable<TElement> Run<TElement>(string sql, CommandType commandType, params object[] paras)
        {
            IEnumerable<TElement> entityList = new List<TElement>();
            DataTable dt = Run(sql, commandType, paras);
            entityList = EntityHelper.GetEntityList<TElement>(dt);
            return entityList;
        }


        public object CreateCommandParameter(string name, object value, ParameterDirection dir)
        {
            object rv = null;
            switch (this.DBType)
            {
                case DBTypeEnum.SqlServer:
                    rv = new SqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.MySql:
                    rv = new MySqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.PgSql:
                    rv = new NpgsqlParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.SQLite:
                    rv = new SqliteParameter(name, value) { Direction = dir };
                    break;
                case DBTypeEnum.Oracle:
                    rv = new OracleParameter(name, value) { Direction = dir };
                    break;
            }
            return rv;
        }
    }

}
