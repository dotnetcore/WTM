using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
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
    public partial class FrameworkContext : DbContext, IDataContext
    {
        //public DbSet<FrameworkModule> BaseFrameworkModules { get; set; }
        //public DbSet<FrameworkAction> BaseFrameworkActions { get; set; }
        public DbSet<FrameworkMenu> BaseFrameworkMenus { get; set; }
        public DbSet<FunctionPrivilege> BaseFunctionPrivileges { get; set; }
        public DbSet<DataPrivilege> BaseDataPrivileges { get; set; }
        public DbSet<FileAttachment> BaseFileAttachments { get; set; }
        public DbSet<FrameworkUserBase> BaseFrameworkUsers { get; set; }
        public DbSet<FrameworkRole> BaseFrameworkRoles { get; set; }
        public DbSet<FrameworkGroup> BaseFrameworkGroups { get; set; }
        public DbSet<ActionLog> BaseActionLogs { get; set; }
        public DbSet<FrameworkArea> BaseFrameworkAreas { get; set; }



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
        /// <summary>
        /// FrameworkContext
        /// </summary>
        public FrameworkContext()
        {
            CSName = "default";
        }

        /// <summary>
        /// FrameworkContext
        /// </summary>
        /// <param name="cs"></param>
        public FrameworkContext(string cs)
        {
            CSName = cs;
        }

        public FrameworkContext(string cs, DBTypeEnum dbtype)
        {
            CSName = cs;
            DBType = dbtype;
        }

        public IDataContext CreateNew()
        {
           return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum) }).Invoke(new object[] { CSName, DBType }); ;
        }

        public IDataContext ReCreate()
        {
            return (IDataContext)this.GetType().GetConstructor(new Type[] { typeof(string), typeof(DBTypeEnum) }).Invoke(new object[] { CSName, DBType }); ;
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
            if (set.Local.Where(x => x.ID == entity.ID).FirstOrDefault() == null)
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
            if (set.Local.Where(x => x.ID == entity.ID).FirstOrDefault() == null)
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
            if (set.Local.Where(x => x.ID == entity.ID).FirstOrDefault() == null)
            {
                set.Attach(entity);
            }
            set.Remove(entity);
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
            //菜单和菜单权限的级联删除
            modelBuilder.Entity<FunctionPrivilege>().HasOne(x => x.MenuItem).WithMany(x => x.Privileges).HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Cascade);
            //用户和用户搜索条件级联删除
            modelBuilder.Entity<SearchCondition>().HasOne(x => x.User).WithMany(x => x.SearchConditions).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

            var modelAsms = Utils.GetAllAssembly();

            var allTypes = new List<Type>();// 所有 DbSet<> 的泛型类型

            #region 获取所有 DbSet<T> 的泛型类型 T 及其 List<T> 类型属性对应的类型 T

            // 获取所有 DbSet<T> 的泛型类型 T
            foreach (var asm in modelAsms)
            {
                var dcModule = asm.GetExportedTypes().Where(x => typeof(DbContext).IsAssignableFrom(x)).ToList();
                if (dcModule != null && dcModule.Count > 0)
                {
                    foreach (var module in dcModule)
                    {
                        foreach (var pro in module.GetProperties())
                        {
                            if (pro.PropertyType.IsGeneric(typeof(DbSet<>)))
                            {
                                if (!allTypes.Contains(pro.PropertyType.GenericTypeArguments[0], new TypeComparer()))
                                {
                                    allTypes.Add(pro.PropertyType.GenericTypeArguments[0]);
                                }
                            }
                        }
                    }
                }
            }

            // 获取类型 T 下 List<S> 类型的属性对应的类型 S，且S 必须是 TopBasePoco 的子类，只有这些类会生成库
            for (int i = 0; i < allTypes.Count; i++) // 
            {
                var item = allTypes[i];
                var pros = item.GetProperties();
                foreach (var pro in pros)
                {
                    if (typeof(TopBasePoco).IsAssignableFrom(pro.PropertyType))
                    {
                        if (allTypes.Contains(pro.PropertyType) == false)
                        {
                            allTypes.Add(pro.PropertyType);
                        }
                    }
                    else
                    {
                        if (pro.PropertyType.IsGenericType && pro.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            var inner = pro.PropertyType.GetGenericArguments()[0];
                            if (typeof(TopBasePoco).IsAssignableFrom(inner))
                            {
                                if (allTypes.Contains(inner) == false)
                                {
                                    allTypes.Add(inner);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            foreach (var item in allTypes)
            {
                if (typeof(BasePoco).IsAssignableFrom(item))
                {
                    //给所有model的CreateTime字段加索引
                    //最终实现的是类似 modelBuilder.Entity<ActionLog>().HasIndex("CreateTime") 这种语句调用
                    //由于循环的多语言类是动态的，所以用反射和Lambda来实现
                    var entity = typeof(ModelBuilder).GetMethod("Entity", Type.EmptyTypes).MakeGenericMethod(item).Invoke(modelBuilder, null);
                    typeof(EntityTypeBuilder<>).MakeGenericType(item).GetMethod("HasIndex", new Type[] { typeof(string[]) }).Invoke(entity, new object[] { new string[] { "CreateTime" } });
                }
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
                    optionsBuilder.UseSqlServer(CSName,op=>op.UseRowNumberForPaging());
                    break;
                case DBTypeEnum.MySql:
                    optionsBuilder.UseMySql(CSName);
                    break;
                case DBTypeEnum.PgSql:
                    optionsBuilder.UseNpgsql(CSName);
                    break;
                case DBTypeEnum.Memory:
                    optionsBuilder.UseInMemoryDatabase(CSName);
                    break;
                default:
                    break;
            }
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true即数据新建完成，进入初始化操作，返回false即数据库已经存在</returns>
        public async virtual Task<bool> DataInit(object allModules, bool IsSpa)
        {
            if (await Database.EnsureCreatedAsync())
            {
                var AllModules = allModules as List<FrameworkModule>;
                //foreach (var module in AllModules)
                //{
                //    module.CreateTime = DateTime.Now;
                //    module.CreateBy = "admin";
                //    Set<FrameworkModule>().Add(module);
                //}

                var roles = new FrameworkRole[]
                {
                    new FrameworkRole{ RoleCode = "001", RoleName = "超级管理员"}
                };
                var users = new FrameworkUserBase[]
                {
                    new FrameworkUserBase{ITCode = "admin", Password = Utils.GetMD5String("000000"), IsValid = true, Name="超级管理员"}
                };
                var userroles = new FrameworkUserRole[]
                {
                    new FrameworkUserRole{ User = users[0], Role = roles[0]}
                };

                var adminRole = roles[0];
                if (Set<FrameworkMenu>().Any() == false)
                {
                    var systemManagement = GetFolderMenu("系统管理", new List<FrameworkRole> { adminRole }, null);
                    var logList = IsSpa ? GetMenu2(AllModules,"ActionLog", new List<FrameworkRole> { adminRole }, null, 1) : GetMenu(AllModules, "_Admin", "ActionLog", "Index", new List<FrameworkRole> { adminRole }, null, 1);
                    var userList = IsSpa ? GetMenu2(AllModules, "FrameworkUser", new List<FrameworkRole> { adminRole }, null, 2) : GetMenu(AllModules, "_Admin", "FrameworkUser", "Index", new List<FrameworkRole> { adminRole }, null, 2);
                    var roleList = IsSpa ? GetMenu2(AllModules, "FrameworkRole", new List<FrameworkRole> { adminRole }, null, 3) : GetMenu(AllModules, "_Admin", "FrameworkRole", "Index", new List<FrameworkRole> { adminRole }, null, 3);
                    var groupList = IsSpa ? GetMenu2(AllModules, "FrameworkGroup", new List<FrameworkRole> { adminRole }, null, 4) : GetMenu(AllModules, "_Admin", "FrameworkGroup", "Index", new List<FrameworkRole> { adminRole }, null, 4);
                    var menuList = IsSpa ? GetMenu2(AllModules, "FrameworkMenu", new List<FrameworkRole> { adminRole }, null, 5) : GetMenu(AllModules, "_Admin", "FrameworkMenu", "Index", new List<FrameworkRole> { adminRole }, null, 5);
                    var dpList = IsSpa ? GetMenu2(AllModules, "DataPrivilege", new List<FrameworkRole> { adminRole }, null, 6) : GetMenu(AllModules, "_Admin", "DataPrivilege", "Index", new List<FrameworkRole> { adminRole }, null, 6);
                    if (logList != null)
                    {
                        systemManagement.Children.AddRange(new FrameworkMenu[] { logList, userList, roleList, groupList, menuList, dpList });
                        Set<FrameworkMenu>().Add(systemManagement);
                    }
                }
                Set<FrameworkRole>().AddRange(roles);
                Set<FrameworkUserBase>().AddRange(users);
                Set<FrameworkUserRole>().AddRange(userroles);
                await SaveChangesAsync();
                return true;
            }
            return false;
        }

        private FrameworkMenu GetFolderMenu(string FolderText, List<FrameworkRole> allowedRoles, List<FrameworkUserBase> allowedUsers, bool isShowOnMenu = true, bool isInherite = false)
        {
            FrameworkMenu menu = new FrameworkMenu
            {
                PageName = FolderText,
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
            var acts = allModules.Where(x => x.ClassName == controllerName && x.IsApi == true).SelectMany(x => x.Actions).ToList();
            FrameworkMenu menu = GetMenuFromAction(acts[0], true, allowedRoles, allowedUsers, displayOrder);
            if (menu != null)
            {
                menu.Url = "/" + acts[0].Module.ClassName.ToLower();
                menu.ModuleName = acts[0].Module.ModuleName;
                menu.PageName = menu.ModuleName;
                menu.ActionName = "主页面";
                menu.ClassName = acts[0].Module.ClassName;
                menu.MethodName = null;
                for (int i = 0; i < acts.Count; i++)
                {
                    if (acts[i] != null)
                    {
                        menu.Children.Add(GetMenuFromAction(acts[i], false, allowedRoles, allowedUsers, (i + 1)));
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
                ClassName = act.Module.ClassName,
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
                menu.PageName = act.Module.ModuleName;
                menu.ModuleName = act.Module.ModuleName;
                menu.ActionName = act.ActionName;
            }
            else
            {
                menu.PageName = act.ActionName;
                menu.ModuleName = act.Module.ModuleName;
                menu.ActionName = act.ActionName;
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
            }
            return rv;
        }
    }
}
