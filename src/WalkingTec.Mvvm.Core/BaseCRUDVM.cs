using Elsa.Models;
using Elsa.Persistence.Specifications;
using Elsa.Persistence;
using Elsa.Services;
using Elsa.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core.Models;
using WalkingTec.Mvvm.Core.Support.FileHandlers;
using WalkingTec.Mvvm.Core.WorkFlow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Npgsql.Logging;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 单表增删改查VM的接口
    /// </summary>
    /// <typeparam name="T">继承TopBasePoco的类</typeparam>
    public interface IBaseCRUDVM<out T> where T : TopBasePoco, new()
    {
        T Entity { get; }
        /// <summary>
        /// 根据主键Id获取Entity
        /// </summary>
        /// <param name="id">主键Id</param>
        void SetEntityById(object id);

        /// <summary>
        /// 设置Entity
        /// </summary>
        /// <param name="entity">要设定的TopBasePoco</param>
        void SetEntity(object entity);

        /// <summary>
        /// 添加
        /// </summary>
        void DoAdd();

        Task DoAddAsync();

        /// <summary>
        /// 修改
        /// </summary>
        void DoEdit(bool updateAllFields);
        Task DoEditAsync(bool updateAllFields);

        /// <summary>
        /// 删除，对于TopBasePoco进行物理删除，对于PersistPoco把IsValid修改为false
        /// </summary>
        void DoDelete();
        Task DoDeleteAsync();

        Task<RunWorkflowResult> StartWorkflowAsync(string flowName=null);
        Task<RunWorkflowResult> ContinueWorkflowAsync(string actionName, string remark, string flowName=null, string tag = null);
        Task<List<ApproveTimeLine>> GetWorkflowTimeLineAsync(string flowName = null);
        Task<WorkflowInstance> GetWorkflowInstanceAsync(string flowName = null);
        /// <summary>
        /// 彻底删除，对PersistPoco进行物理删除
        /// </summary>
        void DoRealDelete();
        Task DoRealDeleteAsync();

        /// <summary>
        /// 将源VM的上数据库上下文，Session，登录用户信息，模型状态信息，缓存信息等内容复制到本VM中
        /// </summary>
        /// <param name="vm">复制的源</param>
        void CopyContext(BaseVM vm);

        /// <summary>
        /// 是否跳过基类的唯一性验证，批量导入的时候唯一性验证会由存储过程完成，不需要单独调用本类的验证方法
        /// </summary>
        bool ByPassBaseValidation { get; set; }

        void Validate();
        IModelStateService MSD { get; }
    }

    /// <summary>
    /// 单表增删改查基类，所有单表操作的VM应该继承这个基类
    /// </summary>
    /// <typeparam name="TModel">继承TopBasePoco的类</typeparam>
    public class BaseCRUDVM<TModel> : BaseVM, IBaseCRUDVM<TModel> where TModel : TopBasePoco, new()
    {
        internal static readonly MethodInfo IncludeMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("Include").Single((MethodInfo mi) => mi.GetGenericArguments().Count() == 2 && mi.GetParameters().Any((ParameterInfo pi) => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));
        internal static readonly MethodInfo ThenIncludeAfterEnumerableMethodInfo = (from mi in typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("ThenInclude")
                                                                                    where mi.GetGenericArguments().Count() == 3
                                                                                    select mi).Single(delegate (MethodInfo mi)
                                                                                    {
                                                                                        Type type = mi.GetParameters()[0].ParameterType.GenericTypeArguments[1];
                                                                                        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
                                                                                    });

        internal static readonly MethodInfo ThenIncludeAfterReferenceMethodInfo = typeof(EntityFrameworkQueryableExtensions).GetTypeInfo().GetDeclaredMethods("ThenInclude").Single((MethodInfo mi) => mi.GetGenericArguments().Count() == 3 && mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter);
        public TModel Entity { get; set; }
        [JsonIgnore]
        public bool ByPassBaseValidation { get; set; }

        //保存读取时Include的内容
        private List<Expression<Func<TModel, object>>> _toInclude { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseCRUDVM()
        {
            //初始化Entity
            var ctor = typeof(TModel).GetConstructor(Type.EmptyTypes);
            Entity = ctor.Invoke(null) as TModel;
            //初始化VM中所有List<>的类
            //var lists = typeof(TModel).GetAllProperties().Where(x => x.PropertyType.IsGeneric(typeof(List<>)));
            //foreach (var li in lists)
            //{
            //    var gs = li.PropertyType.GetGenericArguments();
            //    var newObj = Activator.CreateInstance(typeof(List<>).MakeGenericType(gs[0]));
            //    li.SetValue(Entity, newObj, null);
            //}
        }

        public IQueryable<TModel> GetBaseQuery()
        {
            return DC.Set<TModel>();
        }
        /// <summary>
        /// 设定添加和修改时对于重复数据的判断，子类进行相关操作时应重载这个函数
        /// </summary>
        /// <returns>唯一性属性</returns>
        public virtual DuplicatedInfo<TModel> SetDuplicatedCheck()
        {
            return null;
        }

        /// <summary>
        /// 设定读取是Include的内容
        /// </summary>
        /// <param name="exps">需要关联的类</param>
        public void SetInclude(params Expression<Func<TModel, object>>[] exps)
        {
            _toInclude = _toInclude ?? new List<Expression<Func<TModel, object>>>();
            _toInclude.AddRange(exps);
        }

        /// <summary>
        /// 根据主键Id设定Entity
        /// </summary>
        /// <param name="id">主键Id</param>
        public void SetEntityById(object id)
        {
            this.Entity = GetById(id);
        }

        /// <summary>
        /// 设置Entity
        /// </summary>
        /// <param name="entity">要设定的TopBasePoco</param>
        public void SetEntity(object entity)
        {
            this.Entity = entity as TModel;
        }

        /// <summary>
        /// 根据主键获取Entity
        /// </summary>
        /// <param name="Id">主键Id</param>
        /// <returns>Entity</returns>
        protected virtual TModel GetById(object Id)
        {
            TModel rv = null;
            var ModelType = typeof(TModel);
            //建立基础查询
            var query = DC.Set<TModel>().AsQueryable();
            List<IncludeInfo> includeInfo = new List<IncludeInfo>();
            //循环添加其他设定的Include
            if (_toInclude != null)
            {
                foreach (var item in _toInclude)
                {
                    List<IncludeInfo> exps = new List<IncludeInfo>();
                    Expression current = item.Body;
                    while (current != null && current.NodeType != ExpressionType.Parameter)
                    {
                        if (current.NodeType == ExpressionType.MemberAccess)
                        {
                            MemberExpression me = current as MemberExpression;
                            Type mt = me.Member.GetMemberType();
                            Type testTypt = mt;
                            if (testTypt.IsList())
                            {
                                testTypt = testTypt.GetGenericArguments()[0];
                            }
                            if (typeof(TopBasePoco).IsAssignableFrom(testTypt))
                            {
                                IncludeInfo newinfo = new IncludeInfo
                                {
                                    mi = me.Member,
                                    t = mt
                                };
                                var top = exps.FirstOrDefault();
                                if (top != null)
                                {
                                    newinfo.Next = top;
                                    top.Pre = newinfo;
                                }
                                exps.Insert(0, newinfo);
                            }
                            current = me.Expression;
                        }
                        else if (current.NodeType == ExpressionType.Call)
                        {
                            MethodCallExpression mc = current as MethodCallExpression;
                            current = mc.Object;
                        }
                        else if (current.NodeType == ExpressionType.Convert)
                        {
                            UnaryExpression ue = current as UnaryExpression;
                            current = ue.Operand;
                        }
                    }
                    if (exps.Count == 0)
                    {
                        continue;
                    }
                    includeInfo.Add(exps[0]);
                    Expression includeExpression = query.Expression;
                    ParameterExpression para = Expression.Parameter(ModelType, "x");
                    MemberExpression newme = Expression.MakeMemberAccess(para, exps[0].mi);

                    if (exps[0].IsNotMapped)
                    {
                        continue;
                    }
                    includeExpression = Expression.Call(
                             null,
                             IncludeMethodInfo.MakeGenericMethod(ModelType, exps[0].t),
                             includeExpression,
                             Expression.Lambda(newme, new ParameterExpression[] { para }));

                    for (int i = 1; i < exps.Count; i++)
                    {
                        if (exps[i].IsNotMapped)
                        {
                            break; ;
                        }
                        Type stype = exps[i - 1].t;
                        if (stype.IsList())
                        {
                            stype = stype.GetGenericArguments()[0];
                        }
                        para = Expression.Parameter(stype, "s");
                        newme = Expression.MakeMemberAccess(para, exps[i].mi);
                        if (exps[i - 1].t.IsList())
                        {
                            includeExpression = Expression.Call(
                             null,
                             ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(ModelType, stype, exps[i].t),
                             includeExpression,
                             Expression.Lambda(newme, new ParameterExpression[] { para }));
                        }
                        else
                        {
                            includeExpression = Expression.Call(
                             null,
                             ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(ModelType, stype, exps[i].t),
                             includeExpression,
                             Expression.Lambda(newme, new ParameterExpression[] { para }));
                        }
                    }
                    query = query.Provider.CreateQuery<TModel>(includeExpression);
                }
            }

            List<IncludeInfo> softincludes = includeInfo.Where(x => x.HasNotMapped == true).ToList();
            if (softincludes.Count > 0)
            {
                ParameterExpression pe = Expression.Parameter(ModelType);
                NewExpression newItem = Expression.New(ModelType);

                var pp = ModelType.GetAllProperties();
                List<MemberBinding> bindExps = new List<MemberBinding>();
                List<string> existname = new List<string>();
                foreach (var pro in pp)
                {
                    if (existname.Contains(pro.Name))
                    {
                        continue;
                    }
                    if (pro.GetCustomAttribute<NotMappedAttribute>() == null)
                    {
                        if ((pro.PropertyType.IsList() == false && typeof(TopBasePoco).IsAssignableFrom(pro.PropertyType) == false) || includeInfo.Any(x=>x.t == pro.PropertyType))
                        {
                            var right = Expression.MakeMemberAccess(pe, pro);
                            if (right != null)
                            {
                                MemberBinding bind = Expression.Bind(pro, right);
                                bindExps.Add(bind);
                                existname.Add(pro.Name);
                            }
                        }
                    }
                    else
                    {
                        var soft = softincludes.Where(x => x.mi.Name == pro.Name).FirstOrDefault();
                        if (soft != null)
                        {
                            var right = soft.SoftSelect(pe, DC);
                            if (right != null)
                            {
                                MemberBinding bind = Expression.Bind(pro, right);
                                bindExps.Add(bind);
                            }
                        }
                    }
                }
                MemberInitExpression init = Expression.MemberInit(newItem, bindExps);
                var lambda = Expression.Lambda<Func<TModel, TModel>>(init, pe);
                query = query.Select(lambda);
            }
            //获取数据
            rv = query.CheckID(Id).AsNoTracking().FirstOrDefault();
            if (rv == null)
            {
                throw new Exception("数据不存在");
            }
            //如果TopBasePoco有关联的附件，则自动Include 附件名称
            var pros = typeof(TModel).GetAllProperties();
            var fa = pros.Where(x => x.PropertyType == typeof(FileAttachment)).ToList();
            foreach (var f in fa)
            {
                var fname = DC.GetFKName2<TModel>(f.Name);
                var fid = typeof(TModel).GetSingleProperty(fname).GetValue(rv);
                if (fid != null && Wtm.ServiceProvider != null)
                {
                    var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                    var file = fp.GetFile(fid?.ToString(), false, DC);
                    rv.SetPropertyValue(f.Name, file);
                }
            }
            return rv;
        }

        /// <summary>
        /// 添加，进行默认的添加操作。子类如有自定义操作应重载本函数
        /// </summary>
        public virtual void DoAdd()
        {
            DoAddPrepare();
            //删除不需要的附件
            if (DeletedFileIds != null && DeletedFileIds.Count > 0 && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();

                foreach (var item in DeletedFileIds)
                {
                    fp.DeleteFile(item.ToString(), DC);
                }
            }
            DC.SaveChanges();
        }

        public virtual async Task DoAddAsync()
        {
            DoAddPrepare();
            //删除不需要的附件
            if (DeletedFileIds != null && DeletedFileIds.Count > 0 && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();

                foreach (var item in DeletedFileIds)
                {
                    fp.DeleteFile(item.ToString(), DC.ReCreate());
                }
            }
            await DC.SaveChangesAsync();
        }

        private void DoAddPrepare()
        {
            var pros = typeof(TModel).GetAllProperties();
            //将所有TopBasePoco的属性赋空值，防止添加关联的重复内容
            if (typeof(TModel) != typeof(FileAttachment))
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                    {
                        pro.SetValue(Entity, null);
                        string fkname = DC.GetFKName2<TModel>(pro.Name);
                        var fkpro = pros.Where(x => x.Name == fkname).FirstOrDefault();
                        if (fkpro != null)
                        {
                            if (fkpro.PropertyType == typeof(string) && fkpro.GetValue(Entity)?.ToString() == "")
                            {
                                fkpro.SetValue(Entity, null);
                            }
                        }
                    }
                }
            }
            //自动设定添加日期和添加人
            if (typeof(IBasePoco).IsAssignableFrom(typeof(TModel)))
            {
                IBasePoco ent = Entity as IBasePoco;
                if (ent.CreateTime == null)
                {
                    ent.CreateTime = DateTime.Now;
                }
                if (string.IsNullOrEmpty(ent.CreateBy))
                {
                    ent.CreateBy = LoginUserInfo?.ITCode;
                }
            }
            if (typeof(ITenant).IsAssignableFrom(typeof(TModel)))
            {
                ITenant ent = Entity as ITenant;
                ent.TenantCode = LoginUserInfo?.CurrentTenant;
            }
            if (typeof(IPersistPoco).IsAssignableFrom(typeof(TModel)))
            {
                (Entity as IPersistPoco).IsValid = true;
            }

            #region 更新子表
            foreach (var pro in pros)
            {
                //找到类型为List<xxx>的字段
                if (pro.PropertyType.GenericTypeArguments.Count() > 0)
                {
                    //获取xxx的类型
                    var ftype = pro.PropertyType.GenericTypeArguments.First();
                    //如果xxx继承自TopBasePoco
                    if (ftype.IsSubclassOf(typeof(TopBasePoco)))
                    {
                        string softkey = "";
                        //界面传过来的子表数据
                        IEnumerable<TopBasePoco> list = pro.GetValue(Entity) as IEnumerable<TopBasePoco>;
                        if (list != null && list.Count() > 0)
                        {
                            string fkname = DC.GetFKName<TModel>(pro.Name);
                            if (string.IsNullOrEmpty(fkname))
                            {
                                if (pro.GetCustomAttribute<NotMappedAttribute>() != null)
                                {
                                    fkname = pro.GetCustomAttribute<SoftFKAttribute>()?.PropertyName;
                                    softkey = typeof(TModel).GetCustomAttribute<SoftKeyAttribute>()?.PropertyName;
                                }
                            }
                            var itemPros = ftype.GetAllProperties();

                            bool found = false;
                            foreach (var newitem in list)
                            {
                                foreach (var itempro in itemPros)
                                {
                                    if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                                    {
                                        itempro.SetValue(newitem, null);
                                    }
                                    if (!string.IsNullOrEmpty(fkname))
                                    {
                                        if (itempro.Name.ToLower() == fkname.ToLower())
                                        {
                                            try
                                            {
                                                itempro.SetValue(newitem, string.IsNullOrEmpty(softkey) ? Entity.GetID() : Entity.GetPropertyValue(softkey));
                                            }
                                            catch { }
                                            found = true;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(softkey) == false)
                                {
                                    DC.AddEntity(newitem);
                                }
                            }
                            //如果没有找到相应的外建字段，则可能是多对多的关系，或者做了特殊的设定，这种情况框架无法支持，直接退出本次循环
                            if (found == false)
                            {
                                continue;
                            }
                            //循环页面传过来的子表数据,自动设定添加日期和添加人
                            foreach (var newitem in list)
                            {
                                var subtype = newitem.GetType();
                                if (typeof(IBasePoco).IsAssignableFrom(subtype))
                                {
                                    IBasePoco ent = newitem as IBasePoco;
                                    if (ent.CreateTime == null)
                                    {
                                        ent.CreateTime = DateTime.Now;
                                    }
                                    if (string.IsNullOrEmpty(ent.CreateBy))
                                    {
                                        ent.CreateBy = LoginUserInfo?.ITCode;
                                    }
                                }
                                if (typeof(ITenant).IsAssignableFrom(subtype))
                                {
                                    ITenant ent = newitem as ITenant;
                                    ent.TenantCode = LoginUserInfo?.CurrentTenant;
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            //添加数据
            DC.Set<TModel>().Add(Entity);

        }

        /// <summary>
        /// 修改，进行默认的修改操作。子类如有自定义操作应重载本函数
        /// </summary>
        /// <param name="updateAllFields">为true时，框架会更新当前Entity的全部值，为false时，框架会检查Request.Form里的key，只更新表单提交的字段</param>
        public virtual void DoEdit(bool updateAllFields = false)
        {
            DoEditPrepare(updateAllFields);

            try
            {
                DC.SaveChanges();
            }
            catch
            {
                MSD.AddModelError(" ", Localizer["Sys.EditFailed"]);
            }
            //删除不需要的附件
            if (DeletedFileIds != null && DeletedFileIds.Count > 0 && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();

                foreach (var item in DeletedFileIds)
                {
                    fp.DeleteFile(item.ToString(), DC.ReCreate());
                }
            }

        }

        public virtual async Task DoEditAsync(bool updateAllFields = false)
        {
            DoEditPrepare(updateAllFields);

            await DC.SaveChangesAsync();
            //删除不需要的附件
            if (DeletedFileIds != null && DeletedFileIds.Count > 0 && Wtm.ServiceProvider != null)
            {
                var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();

                foreach (var item in DeletedFileIds)
                {
                    fp.DeleteFile(item.ToString(), DC);
                }
            }
        }

        private void DoEditPrepare(bool updateAllFields)
        {
            if (typeof(IBasePoco).IsAssignableFrom(typeof(TModel)))
            {
                IBasePoco ent = Entity as IBasePoco;
                //if (ent.UpdateTime == null)
                //{
                ent.UpdateTime = DateTime.Now;
                //}
                //if (string.IsNullOrEmpty(ent.UpdateBy))
                //{
                ent.UpdateBy = LoginUserInfo?.ITCode;
                //}
            }
            var pros = typeof(TModel).GetAllProperties();
            //pros = pros.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)) == false).ToList();
            if (typeof(TModel) != typeof(FileAttachment))
            {
                foreach (var pro in pros)
                {
                    if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                    {
                        pro.SetValue(Entity, null);
                        string fkname = DC.GetFKName2<TModel>(pro.Name);
                        var fkpro = pros.Where(x => x.Name == fkname).FirstOrDefault();
                        if (fkpro != null)
                        {
                            if (fkpro.PropertyType == typeof(string) && fkpro.GetValue(Entity)?.ToString() == "")
                            {
                                fkpro.SetValue(Entity, null);
                            }
                        }
                    }
                }
            }
            #region 更新子表
            foreach (var pro in pros)
            {
                //找到类型为List<xxx>的字段
                if (pro.PropertyType.GenericTypeArguments.Count() > 0)
                {
                    //获取xxx的类型
                    var ftype = pro.PropertyType.GenericTypeArguments.First();
                    //如果xxx继承自TopBasePoco
                    if (ftype.IsSubclassOf(typeof(TopBasePoco)))
                    {
                        //界面传过来的子表数据
                        //获取外键字段名称
                        string fkname = DC.GetFKName<TModel>(pro.Name);
                        string softkey = "";
                        if (string.IsNullOrEmpty(fkname))
                        {
                            if (pro.GetCustomAttribute<NotMappedAttribute>() != null)
                            {
                                fkname = pro.GetCustomAttribute<SoftFKAttribute>()?.PropertyName;
                                softkey = typeof(TModel).GetCustomAttribute<SoftKeyAttribute>()?.PropertyName;
                            }
                        }
                        if (pro.GetValue(Entity) is IEnumerable<TopBasePoco> list && list.Count() > 0)
                        {
                            var itemPros = ftype.GetAllProperties();
                            bool found = false;
                            foreach (var newitem in list)
                            {
                                var subtype = newitem.GetType();
                                if (typeof(IBasePoco).IsAssignableFrom(subtype))
                                {
                                    IBasePoco ent = newitem as IBasePoco;
                                    if (ent.UpdateTime == null)
                                    {
                                        ent.UpdateTime = DateTime.Now;
                                    }
                                    if (string.IsNullOrEmpty(ent.UpdateBy))
                                    {
                                        ent.UpdateBy = LoginUserInfo?.ITCode;
                                    }
                                }
                                //循环页面传过来的子表数据,将关联到TopBasePoco的字段设为null,并且把外键字段的值设定为主表ID
                                foreach (var itempro in itemPros)
                                {
                                    if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                                    {
                                        itempro.SetValue(newitem, null);
                                    }
                                    if (!string.IsNullOrEmpty(fkname))
                                    {
                                        if (itempro.Name.ToLower() == fkname.ToLower())
                                        {
                                            try
                                            {
                                                itempro.SetValue(newitem, string.IsNullOrEmpty(softkey) ? Entity.GetID() : Entity.GetPropertyValue(softkey));
                                            }
                                            catch { }
                                            found = true;
                                        }
                                    }
                                }
                            }
                            //如果没有找到相应的外建字段，则可能是多对多的关系，或者做了特殊的设定，这种情况框架无法支持，直接退出本次循环
                            if (found == false)
                            {
                                continue;
                            }

                            var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(ftype);
                            var dataquery = set.Invoke(DC, null) as IQueryable<TopBasePoco>;
                            ParameterExpression pe = Expression.Parameter(ftype);
                            Expression member = Expression.MakeMemberAccess(pe, ftype.GetSingleProperty(fkname));
                            //member = Expression.Call(member, "ToString", new Type[] { });
                            Expression right = Expression.Constant(string.IsNullOrEmpty(softkey) ? Entity.GetID() : Entity.GetPropertyValue(softkey), member.Type);
                            Expression condition = Expression.Equal(member, right);
                            var exp = Expression.Call(
                                  typeof(Queryable),
                                  "Where",
                                  new Type[] { ftype },
                                  dataquery.Expression,
                                  Expression.Lambda(condition, new ParameterExpression[] { pe }));
                            var q = dataquery.Provider.CreateQuery(exp) as IQueryable<TopBasePoco>;
                            IEnumerable<TopBasePoco> data = q.AsNoTracking().ToList();
                            //比较子表原数据和新数据的区别
                            IEnumerable<TopBasePoco> toadd = null;
                            IEnumerable<TopBasePoco> toremove = null;
                            Utils.CheckDifference(data, list, out toremove, out toadd);
                            //设定子表应该更新的字段
                            List<string> setnames = new List<string>();
                            foreach (var field in FC.Keys)
                            {
                                var f = field.ToLower();

                                if (f.StartsWith($"{this.GetParentStr().ToLower()}entity." + pro.Name.ToLower() + "[0]."))
                                {
                                    string name = f.Replace($"{this.GetParentStr().ToLower()}entity." + pro.Name.ToLower() + "[0].", "");
                                    setnames.Add(name);
                                }
                            }

                            //前台传过来的数据
                            foreach (var newitem in list)
                            {
                                //数据库中的数据
                                foreach (var item in data)
                                {
                                    //需要更新的数据
                                    if (newitem.GetID().ToString() == item.GetID().ToString())
                                    {
                                        dynamic i = newitem;
                                        var newitemType = item.GetType();
                                        foreach (var itempro in itemPros)
                                        {
                                            if (!itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)) && (updateAllFields == true || setnames.Contains(itempro.Name.ToLower())))
                                            {
                                                var notmapped = itempro.GetCustomAttribute<NotMappedAttribute>();
                                                var cannotedit = itempro.GetCustomAttribute<CanNotEditAttribute>();
                                                if (itempro.Name != "ID" && notmapped == null && itempro.PropertyType.IsList() == false && cannotedit == null)
                                                {
                                                    DC.UpdateProperty(i, itempro.Name);
                                                }
                                            }
                                        }
                                        if (typeof(IBasePoco).IsAssignableFrom(item.GetType()))
                                        {
                                            DC.UpdateProperty(i, "UpdateTime");
                                            DC.UpdateProperty(i, "UpdateBy");
                                        }
                                    }
                                }
                            }
                            //需要删除的数据
                            foreach (var item in toremove)
                            {
                                //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
                                if (typeof(IPersistPoco).IsAssignableFrom(ftype))
                                {
                                    (item as IPersistPoco).IsValid = false;
                                    if (typeof(IBasePoco).IsAssignableFrom(ftype))
                                    {
                                        (item as IBasePoco).UpdateTime = DateTime.Now;
                                        (item as IBasePoco).UpdateBy = LoginUserInfo?.ITCode;
                                    }
                                    dynamic i = item;
                                    DC.UpdateEntity(i);
                                }
                                else
                                {
                                    foreach (var itempro in itemPros)
                                    {
                                        if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                                        {
                                            itempro.SetValue(item, null);
                                        }
                                    }
                                    dynamic i = item;
                                    DC.DeleteEntity(i);
                                }
                            }
                            //需要添加的数据
                            foreach (var item in toadd)
                            {
                                if (typeof(IBasePoco).IsAssignableFrom(item.GetType()))
                                {
                                    IBasePoco ent = item as IBasePoco;
                                    if (ent.CreateTime == null)
                                    {
                                        ent.CreateTime = DateTime.Now;
                                    }
                                    if (string.IsNullOrEmpty(ent.CreateBy))
                                    {
                                        ent.CreateBy = LoginUserInfo?.ITCode;
                                    }
                                }
                                if (typeof(ITenant).IsAssignableFrom(item.GetType()))
                                {
                                    ITenant ent = item as ITenant;
                                    ent.TenantCode = LoginUserInfo?.CurrentTenant;
                                }
                                DC.AddEntity(item);
                            }
                        }
                        else if ((pro.GetValue(Entity) is IEnumerable<TopBasePoco> list2 && list2?.Count() == 0))
                        {
                            if (string.IsNullOrEmpty(fkname))
                            {
                                continue;
                            }
                            var itemPros = ftype.GetAllProperties();
                            var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(ftype);
                            var dataquery = set.Invoke(DC, null) as IQueryable<TopBasePoco>;
                            ParameterExpression pe = Expression.Parameter(ftype);
                            Expression member = Expression.MakeMemberAccess(pe, ftype.GetSingleProperty(fkname));
                            //member = Expression.Call(member, "ToString", new Type[] { });
                            Expression right = Expression.Constant(string.IsNullOrEmpty(softkey) ? Entity.GetID() : Entity.GetPropertyValue(softkey), member.Type);
                            Expression condition = Expression.Equal(member, right);
                            var exp = Expression.Call(
                                  typeof(Queryable),
                                  "Where",
                                  new Type[] { ftype },
                                  dataquery.Expression,
                                  Expression.Lambda(condition, new ParameterExpression[] { pe }));
                            var q = dataquery.Provider.CreateQuery(exp) as IQueryable<TopBasePoco>;
                            IEnumerable<TopBasePoco> removeData = q.AsNoTracking().ToList();

                            foreach (var item in removeData)
                            {
                                //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
                                if (typeof(IPersistPoco).IsAssignableFrom(ftype))
                                {
                                    (item as IPersistPoco).IsValid = false;
                                    if (typeof(IBasePoco).IsAssignableFrom(ftype))
                                    {
                                        (item as IBasePoco).UpdateTime = DateTime.Now;
                                        (item as IBasePoco).UpdateBy = LoginUserInfo?.ITCode;
                                    }
                                    dynamic i = item;
                                    DC.UpdateEntity(i);
                                }
                                else
                                {
                                    foreach (var itempro in itemPros)
                                    {
                                        if (itempro.PropertyType.IsSubclassOf(typeof(TopBasePoco)))
                                        {
                                            itempro.SetValue(item, null);
                                        }
                                    }
                                    dynamic i = item;
                                    DC.DeleteEntity(i);
                                }
                            }
                        }
                    }
                }
            }
            #endregion


            if (updateAllFields == false)
            {
                if (typeof(TreePoco).IsAssignableFrom(typeof(TModel)))
                {
                    var cid = Entity.GetID();
                    var pid = Entity.GetParentID();
                    if (cid != null && pid != null && cid.ToString() == pid.ToString())
                    {
                        var pkey = FC.Keys.Where(x => x.ToLower() == "entity.parentid").FirstOrDefault();
                        if (string.IsNullOrEmpty(pkey) == false)
                        {
                            FC.Remove(pkey);
                        }
                    }
                }
                foreach (var field in FC.Keys)
                {
                    var f = field.ToLower();
                    if (f.StartsWith($"{this.GetParentStr().ToLower()}entity.") && !f.Contains("["))
                    {
                        string name = f.Replace($"{this.GetParentStr().ToLower()}entity.", "");
                        try
                        {
                            var itempro = pros.Where(x => x.Name.ToLower() == name).FirstOrDefault();
                            var notmapped = itempro.GetCustomAttribute<NotMappedAttribute>();
                            var cannotedit = itempro.GetCustomAttribute<CanNotEditAttribute>();
                            if (itempro.Name != "ID" && notmapped == null && itempro.PropertyType.IsList() == false && cannotedit == null)
                            {
                                DC.UpdateProperty(Entity, itempro.Name);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                if (typeof(IBasePoco).IsAssignableFrom(typeof(TModel)))
                {
                    try
                    {
                        DC.UpdateProperty(Entity, "UpdateTime");
                        DC.UpdateProperty(Entity, "UpdateBy");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                if (typeof(TreePoco).IsAssignableFrom(typeof(TModel)))
                {
                    var cid = Entity.GetID();
                    var pid = Entity.GetParentID();
                    if (cid != null && pid != null && cid.ToString() == pid.ToString())
                    {
                        var parentid = Entity.GetType().GetSingleProperty("ParentId");
                        if (parentid != null)
                        {
                            try
                            {
                                parentid.SetValue(Entity, null);
                            }
                            catch { }
                        }
                    }
                }
                DC.UpdateEntity(Entity);
            }
        }

        /// <summary>
        /// 删除，进行默认的删除操作。子类如有自定义操作应重载本函数
        /// </summary>
        public virtual void DoDelete()
        {
            //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
            if (typeof(IPersistPoco).IsAssignableFrom(typeof(TModel)))
            {
                FC.Add("Entity.IsValid", 0);
                (Entity as IPersistPoco).IsValid = false;

                var pros = typeof(TModel).GetAllProperties();
                //如果包含List<PersistPoco>，将子表IsValid也设置为false
                var fas = pros.Where(x => typeof(IEnumerable<IPersistPoco>).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fas)
                {
                    f.SetValue(Entity, f.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null));
                }

                DoEditPrepare(false);
                DC.SaveChanges();
            }
            //如果是普通的TopBasePoco，则进行物理删除
            else if (typeof(TModel).GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
            {
                DoRealDelete();
            }
        }

        public virtual async Task DoDeleteAsync()
        {
            //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
            if (typeof(IPersistPoco).IsAssignableFrom(typeof(TModel)))
            {
                FC.Add("Entity.IsValid", 0);
                (Entity as IPersistPoco).IsValid = false;
                var pros = typeof(TModel).GetAllProperties();
                //如果包含List<PersistPoco>，将子表IsValid也设置为false
                var fas = pros.Where(x => typeof(IEnumerable<IPersistPoco>).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fas)
                {
                    f.SetValue(Entity, f.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null));
                }
                fas = pros.Where(x => typeof(TopBasePoco).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fas)
                {
                    f.SetValue(Entity, null);
                }
                DoEditPrepare(false);
                try
                {
                    await DC.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    MSD.AddModelError("", CoreProgram._localizer?["Sys.DeleteFailed"]);
                }
            }
            //如果是普通的TopBasePoco，则进行物理删除
            else if (typeof(TModel).GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
            {
                DoRealDelete();
            }
        }

        /// <summary>
        /// 物理删除，对于普通的TopBasePoco和Delete操作相同，对于PersistPoco则进行真正的删除。子类如有自定义操作应重载本函数
        /// </summary>
        public virtual void DoRealDelete()
        {
            try
            {
                List<Guid> fileids = new List<Guid>();
                var pros = typeof(TModel).GetAllProperties();

                //如果包含附件，则先删除附件
                var fa = pros.Where(x => x.PropertyType == typeof(FileAttachment) || typeof(TopBasePoco).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fa)
                {
                    if (f.GetValue(Entity) is FileAttachment file)
                    {
                        fileids.Add(file.ID);
                    }
                    f.SetValue(Entity, null);
                }

                var fas = pros.Where(x => typeof(IEnumerable<ISubFile>).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fas)
                {
                    var subs = f.GetValue(Entity) as IEnumerable<ISubFile>;
                    if (subs == null)
                    {
                        var fullEntity = DC.Set<TModel>().AsQueryable().Include(f.Name).AsNoTracking().CheckID(Entity.ID).FirstOrDefault();
                        subs = f.GetValue(fullEntity) as IEnumerable<ISubFile>;
                    }
                    if (subs != null)
                    {
                        foreach (var sub in subs)
                        {
                            fileids.Add(sub.FileId);
                        }
                        f.SetValue(Entity, null);
                    }
                }
                if (typeof(TModel) != typeof(FileAttachment))
                {
                    foreach (var pro in pros)
                    {
                        if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                        {
                            pro.SetValue(Entity, null);
                        }
                    }
                }
                DC.DeleteEntity(Entity);
                DC.SaveChanges();
                if (typeof(IWorkflow).IsAssignableFrom(typeof(TModel)))
                {
                    var wi = DC.Set<Elsa_WorkflowInstance>().CheckEqual(typeof(TModel).FullName, x => x.ContextType).CheckEqual(Entity.GetID().ToString(), x => x.ContextId).ToList();
                    if (wi.Count > 0)
                    {
                        DC.Set<Elsa_WorkflowInstance>().RemoveRange(wi);
                        var wl = DC.Set<Elsa_WorkflowExecutionLogRecord>().CheckContain(wi.Select(x => x.ID).ToList(), x => x.WorkflowInstanceId).ToList();
                        DC.Set<Elsa_WorkflowExecutionLogRecord>().RemoveRange(wl);
                        var ww = DC.Set<FrameworkWorkflow>().CheckContain(wi.Select(x => x.ID).ToList(), x => x.WorkflowId).ToList();
                        DC.Set<FrameworkWorkflow>().RemoveRange(ww);
                        DC.SaveChanges();
                    }
                }
                if (Wtm.ServiceProvider != null)
                {
                    var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                    foreach (var item in fileids)
                    {
                        fp.DeleteFile(item.ToString(), DC.ReCreate());
                    }
                }
            }
            catch (Exception)
            {
                MSD.AddModelError("", CoreProgram._localizer?["Sys.DeleteFailed"]);
            }
        }


        public virtual async Task DoRealDeleteAsync()
        {
            try
            {
                List<Guid> fileids = new List<Guid>();
                var pros = typeof(TModel).GetAllProperties();

                //如果包含附件，则先删除附件
                var fa = pros.Where(x => x.PropertyType == typeof(FileAttachment) || typeof(TopBasePoco).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fa)
                {
                    if (f.GetValue(Entity) is FileAttachment file)
                    {
                        fileids.Add(file.ID);
                    }
                    f.SetValue(Entity, null);
                }

                var fas = pros.Where(x => typeof(IEnumerable<ISubFile>).IsAssignableFrom(x.PropertyType)).ToList();
                foreach (var f in fas)
                {
                    var subs = f.GetValue(Entity) as IEnumerable<ISubFile>;
                    foreach (var sub in subs)
                    {
                        fileids.Add(sub.FileId);
                    }
                    f.SetValue(Entity, null);
                }
                if (typeof(TModel) != typeof(FileAttachment))
                {
                    foreach (var pro in pros)
                    {
                        if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)))
                        {
                            pro.SetValue(Entity, null);
                        }
                    }
                }
                DC.DeleteEntity(Entity);
                await DC.SaveChangesAsync();
                if (typeof(IWorkflow).IsAssignableFrom(typeof(TModel)))
                {
                    var wi = DC.Set<Elsa_WorkflowInstance>().CheckEqual(typeof(TModel).FullName, x => x.ContextType).CheckEqual(Entity.GetID().ToString(), x => x.ContextId).ToList();
                    if (wi.Count > 0)
                    {
                        DC.Set<Elsa_WorkflowInstance>().RemoveRange(wi);
                        var wl = DC.Set<Elsa_WorkflowExecutionLogRecord>().CheckContain(wi.Select(x => x.ID).ToList(), x => x.WorkflowInstanceId).ToList();
                        DC.Set<Elsa_WorkflowExecutionLogRecord>().RemoveRange(wl);
                        var ww = DC.Set<FrameworkWorkflow>().CheckContain(wi.Select(x => x.ID).ToList(), x => x.WorkflowId).ToList();
                        DC.Set<FrameworkWorkflow>().RemoveRange(ww);
                        await DC.SaveChangesAsync();
                    }
                }
                if (Wtm.ServiceProvider != null)
                {
                    var fp = Wtm.ServiceProvider.GetRequiredService<WtmFileProvider>();
                    foreach (var item in fileids)
                    {
                        fp.DeleteFile(item.ToString(), DC.ReCreate());
                    }
                }
            }
            catch (Exception)
            {
                MSD.AddModelError("", CoreProgram._localizer?["Sys.DeleteFailed"]);
            }
        }

        /// <summary>
        /// 创建重复数据信息
        /// </summary>
        /// <param name="FieldExps">重复数据信息</param>
        /// <returns>重复数据信息</returns>
        protected DuplicatedInfo<TModel> CreateFieldsInfo(params DuplicatedField<TModel>[] FieldExps)
        {
            DuplicatedInfo<TModel> d = new DuplicatedInfo<TModel>();
            d.AddGroup(FieldExps);
            return d;
        }

        /// <summary>
        /// 创建一个简单重复数据信息
        /// </summary>
        /// <param name="FieldExp">重复数据的字段</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<TModel> SimpleField(Expression<Func<TModel, object>> FieldExp)
        {
            return new DuplicatedField<TModel>(FieldExp);
        }

        /// <summary>
        /// 创建一个关联到其他表数组中数据的重复信息
        /// </summary>
        /// <typeparam name="V">关联表类</typeparam>
        /// <param name="MiddleExp">指向关联表类数组的Lambda</param>
        /// <param name="FieldExps">指向最终字段的Lambda</param>
        /// <returns>重复数据信息</returns>
        public static DuplicatedField<TModel> SubField<V>(Expression<Func<TModel, List<V>>> MiddleExp, params Expression<Func<V, object>>[] FieldExps)
        {
            return new ComplexDuplicatedField<TModel, V>(MiddleExp, FieldExps);
        }

        /// <summary>
        /// 验证数据，默认验证重复数据。子类如需要其他自定义验证，则重载这个函数
        /// </summary>
        /// <returns>验证结果</returns>
        public override void Validate()
        {
            if (ByPassBaseValidation == false)
            {
                base.Validate();
                ////如果msd是BasicMSD，则认为他是手动创建的，也就是说并没有走asp.net core默认的模型验证
                ////那么手动验证模型
                //if (Wtm?.MSD is BasicMSD)
                //{
                //    var valContext = new ValidationContext(this.Entity);
                //    List<ValidationResult> error = new List<ValidationResult>();
                //    if (!Validator.TryValidateObject(Entity, valContext, error, true))
                //    {
                //        foreach (var item in error)
                //        {
                //            string key = item.MemberNames.FirstOrDefault();
                //            if (MSD.Keys.Contains(key) == false)
                //            {
                //                MSD.AddModelError($"Entity.{key}", item.ErrorMessage);
                //            }
                //        }
                //    }
                //    var list = typeof(TModel).GetAllProperties().Where(x => x.PropertyType.IsListOf<TopBasePoco>());
                //    foreach (var item in list)
                //    {
                //        var it = item.GetValue(Entity) as IEnumerable;
                //        if(it == null)
                //        {
                //            continue;
                //        }
                //        var contextset = false;
                //        foreach (var e in it)
                //        {
                //            if(contextset == false)
                //            {
                //                valContext = new ValidationContext(e);
                //                contextset = true;
                //            }

                //            if (!Validator.TryValidateObject(e, valContext, error, true))
                //            {
                //                foreach (var err in error)
                //                {
                //                    string key = err.MemberNames.FirstOrDefault();
                //                    if (MSD.Keys.Contains(key) == false)
                //                    {
                //                        MSD.AddModelError($"Entity.{item.Name}.{key}", err.ErrorMessage);
                //                    }
                //                }
                //            }

                //        }
                //    }
                //}

                //验证重复数据
                ValidateDuplicateData();
            }
        }

        /// <summary>
        /// 验证重复数据
        /// 如果存在重复的数据，则返回已存在数据的id列表
        /// 如果不存在重复数据，则返回一个空列表
        /// </summary>
        protected List<object> ValidateDuplicateData()
        {
            //定义一个对象列表用于存放重复数据的id
            var count = new List<object>();
            //获取设定的重复字段信息
            var checkCondition = SetDuplicatedCheck();
            if (checkCondition != null && checkCondition.Groups.Count > 0)
            {
                //生成基础Query
                var baseExp = DC.Set<TModel>().IgnoreQueryFilters().AsQueryable();
                var modelType = typeof(TModel);
                ParameterExpression para = Expression.Parameter(modelType, "tm");
                //循环所有重复字段组
                foreach (var group in checkCondition.Groups)
                {
                    var innercount = new List<object>();
                    List<Expression> conditions = new List<Expression>();
                    //生成一个表达式，类似于 x=>x.Id != id，这是为了当修改数据时验证重复性的时候，排除当前正在修改的数据
                    var idproperty = typeof(TModel).GetSingleProperty("ID");
                    MemberExpression idLeft = Expression.Property(para, idproperty);
                    ConstantExpression idRight = Expression.Constant(Entity.GetID());
                    BinaryExpression idNotEqual = Expression.NotEqual(idLeft, idRight);
                    conditions.Add(idNotEqual);
                    List<PropertyInfo> props = new List<PropertyInfo>();
                    //在每个组中循环所有字段
                    foreach (var field in group.Fields)
                    {
                        Expression exp = field.GetExpression(Entity, para);
                        if (exp != null)
                        {
                            conditions.Add(exp);
                        }
                        //将字段名保存，为后面生成错误信息作准备
                        props.AddRange(field.GetProperties());
                    }
                    if (typeof(ITenant).IsAssignableFrom(typeof(TModel)) && props.Any(x => x.Name.ToLower() == "tenantcode") == false && Wtm?.ConfigInfo.EnableTenant == true && group.UseTenant == true)
                    {
                        ITenant ent = Entity as ITenant;
                        ent.TenantCode = LoginUserInfo.CurrentTenant;
                        var f = new DuplicatedField<TModel>(x => (x as ITenant).TenantCode);
                        Expression exp = f.GetExpression(Entity, para);
                        conditions.Add(exp);
                    }
                    if (typeof(IPersistPoco).IsAssignableFrom(typeof(TModel)) && props.Any(x => x.Name.ToLower() == "isvalid") == false)
                    {
                        IPersistPoco ent = Entity as IPersistPoco;
                        ent.IsValid = true;
                        var f = new DuplicatedField<TModel>(x => (x as IPersistPoco).IsValid);
                        Expression exp = f.GetExpression(Entity, para);
                        conditions.Add(exp);
                    }

                    //如果要求判断id不重复，则去掉id不相等的判断，加入id相等的判断
                    //if (props.Any(x => x.Name.ToLower() == "id"))
                    //{
                    //    conditions.RemoveAt(0);
                    //    BinaryExpression idEqual = Expression.Equal(idLeft, idRight);
                    //    conditions.Insert(0, idEqual);
                    //}
                    //int count = 0;
                    if (conditions.Count > 1)
                    {
                        //循环添加条件并生成Where语句
                        //Expression conExp = conditions[0];
                        Expression whereCallExpression = baseExp.Expression;
                        for (int i = 0; i < conditions.Count; i++)
                        {
                            whereCallExpression = Expression.Call(
                                 typeof(Queryable),
                                 "Where",
                                 new Type[] { modelType },
                                 whereCallExpression,
                                 Expression.Lambda<Func<TModel, bool>>(conditions[i], new ParameterExpression[] { para }));
                        }
                        var result = baseExp.Provider.CreateQuery(whereCallExpression);

                        foreach (TopBasePoco res in result)
                        {
                            var id = res.GetID();
                            count.Add(id);
                            innercount.Add(id);
                        }
                    }
                    if (innercount.Count > 0)
                    {
                        //循环拼接所有字段名
                        string AllName = "";
                        foreach (var prop in props)
                        {
                            string name = PropertyHelper.GetPropertyDisplayName(prop);
                            AllName += name + ",";
                        }
                        if (AllName.EndsWith(","))
                        {
                            AllName = AllName.Remove(AllName.Length - 1);
                        }
                        //如果只有一个字段重复，则拼接形成 xxx字段重复 这种提示
                        if (props.Count == 1)
                        {
                            MSD.AddModelError(GetValidationFieldName(props[0])[0], CoreProgram._localizer?["Sys.DuplicateError", AllName]);
                        }
                        //如果多个字段重复，则拼接形成 xx，yy，zz组合字段重复 这种提示
                        else if (props.Count > 1)
                        {
                            MSD.AddModelError(GetValidationFieldName(props.First())[0], CoreProgram._localizer?["Sys.DuplicateGroupError", AllName]);
                        }
                    }
                }
            }
            return count;
        }


        /// <summary>
        /// 根据属性信息获取验证字段名
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <returns>验证字段名称数组，用于ValidationResult</returns>
        private string[] GetValidationFieldName(PropertyInfo pi)
        {
            return new[] { "Entity." + pi.Name };
        }

        public virtual async Task<RunWorkflowResult> StartWorkflowAsync(string flowName=null)
        {
            if (typeof(IWorkflow).IsAssignableFrom(typeof(TModel)) == false)
            {
                MSD.AddModelError(" noworkflow", CoreProgram._localizer?["Sys.NoWorkflow"]);
                return null;
            }
            if(Entity.HasID() == false)
            {
                return null;
            }

            string workflowId = null;
            string workflowname = null;
            if (string.IsNullOrEmpty(flowName))
            {
                var temp = DC.Set<Elsa_WorkflowDefinition>().Where(x => x.Data.Contains($"\"contextType\": \"{typeof(TModel).FullName}, {typeof(TModel).Assembly.GetName().Name}\"")).Select(x => new { id = x.DefinitionId, name = x.Name }).FirstOrDefault();
                workflowId = temp.id;
                workflowname = temp.name;
            }
            else
            {
                var temp = DC.Set<Elsa_WorkflowDefinition>().Where(x => x.Name == flowName).Select(x=> new { id = x.DefinitionId, name = x.Name }).FirstOrDefault();
                workflowId = temp.id;
                workflowname = temp.name;
            }
            if (string.IsNullOrEmpty(workflowId))
            {
                MSD.AddModelError(" noworkflow", CoreProgram._localizer?["Sys.NoWorkflow"]);
                return null;
            }
            var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowLaunchpad>();
            var workflow = await lp.FindStartableWorkflowAsync(workflowId, contextId: Entity.GetID().ToString(), tenantId: Wtm.LoginUserInfo?.CurrentTenant);
            if (workflow != null)
            {
                workflow.WorkflowInstance.Variables.Set("Submitter", Wtm.LoginUserInfo?.ITCode);
                workflow.WorkflowInstance.Name = workflowname;
                var rv = await lp.ExecuteStartableWorkflowAsync(workflow);
                if(rv?.WorkflowInstance?.Faults?.Count > 0)
                {
                    MSD.AddModelError(" workflowfault", rv?.WorkflowInstance?.Faults[0].Message);

                }
                return rv;
            }

            return null;

        }

        public virtual async Task<RunWorkflowResult> ContinueWorkflowAsync(string actionName, string remark, string flowName=null, string tag = null)
        {
            if (typeof(IWorkflow).IsAssignableFrom(typeof(TModel)) == false)
            {
                MSD.AddModelError(" noworkflow", CoreProgram._localizer?["Sys.NoWorkflow"]);
                return null;
            }
            if (Entity.HasID() == false)
            {
                return null;
            }

            try
            {
                var lp = Wtm.ServiceProvider.GetRequiredService<IWorkflowLaunchpad>();
                //不直接使用Wtm.LoginUserInfo，否则elsa会把所有信息序列化保存到WorkflowInstances表中
                LoginUserInfo li = new LoginUserInfo();
                li.ITCode = Wtm.LoginUserInfo.ITCode;
                li.Name = Wtm.LoginUserInfo.Name;
                li.UserId = Wtm.LoginUserInfo.UserId;
                li.PhotoId = Wtm.LoginUserInfo.PhotoId;
                li.Groups = Wtm.LoginUserInfo.Groups;
                li.Roles = Wtm.LoginUserInfo.Roles;
                li.TenantCode = Wtm.LoginUserInfo.CurrentTenant;
                var query = new WorkflowsQuery(nameof(WtmApproveActivity), new WtmApproveBookmark(Wtm.LoginUserInfo.ITCode, string.IsNullOrEmpty(flowName)?typeof(TModel).FullName:flowName, tag, Entity.GetID().ToString()), null, null, null, Wtm.LoginUserInfo.CurrentTenant);
                if (query != null)
                {
                    var flows = await lp.FindWorkflowsAsync(query);
                    foreach (var item in flows)
                    {
                        if (item.WorkflowInstance == null)
                        {
                            var result = await lp.ExecutePendingWorkflowAsync(item, new WorkflowInput { Input = new WtmApproveInput { Action = actionName, CurrentUser = li, Remark = remark } });
                            return result;
                        }
                    }
                }
                if(Wtm.LoginUserInfo.Roles != null)
                {
                    foreach (var role in Wtm.LoginUserInfo.Roles)
                    {
                        query = new WorkflowsQuery(nameof(WtmApproveActivity), new WtmApproveBookmark(role.ID.ToString(), string.IsNullOrEmpty(flowName) ? typeof(TModel).FullName : flowName, tag, Entity.GetID().ToString()), null, null, null, Wtm.LoginUserInfo.CurrentTenant);
                        if (query != null)
                        {
                            var flows = await lp.FindWorkflowsAsync(query);
                            foreach (var item in flows)
                            {
                                if (item.WorkflowInstance == null)
                                {
                                    var result = await lp.ExecutePendingWorkflowAsync(item, new WorkflowInput { Input = new WtmApproveInput { Action = actionName, CurrentUser = li, Remark = remark } });
                                    return result;
                                }
                            }
                        }

                    }
                }
                if (Wtm.LoginUserInfo.Groups != null)
                {
                    foreach (var group in Wtm.LoginUserInfo.Groups)
                    {
                        query = new WorkflowsQuery(nameof(WtmApproveActivity), new WtmApproveBookmark(group.ID.ToString(), string.IsNullOrEmpty(flowName) ? typeof(TModel).FullName : flowName, tag, Entity.GetID().ToString()), null, null, null, Wtm.LoginUserInfo.CurrentTenant);
                        if (query != null)
                        {

                            var flows = await lp.FindWorkflowsAsync(query);
                            foreach (var item in flows)
                            {
                                if (item.WorkflowInstance == null)
                                {
                                    var result = await lp.ExecutePendingWorkflowAsync(item, new WorkflowInput { Input = new WtmApproveInput { Action = actionName, CurrentUser = li, Remark = remark } });
                                    return result;
                                }
                            }
                        }

                    }
                }
                MSD.AddModelError(" noworkflow", CoreProgram._localizer?["Sys.NoWorkflow"]);
                return null;
            }
            catch { }

            return null;
        }

        public virtual async Task<List<ApproveTimeLine>> GetWorkflowTimeLineAsync(string flowName = null)
        {
            var rv = await Wtm.CallAPI<List<ApproveTimeLine>>("", $"{Wtm.HostAddress}/_workflowapi/GetTimeLine?flowname={flowName}&entitytype={typeof(TModel).FullName}&entityid={Entity.GetID()}");
            return rv.Data;
        }
        public virtual async Task<WorkflowInstance> GetWorkflowInstanceAsync(string flowName = null)
        {
            var rv = await Wtm.CallAPI<WorkflowInstance>("", $"{Wtm.HostAddress}/_workflowapi/GetWorkflow?flowname={flowName}&entitytype={typeof(TModel).FullName}&entityid={Entity.GetID()}");
            return rv.Data;
        }

    }

    class IncludeInfo
    {
        public MemberInfo mi { get; set; }
        public Type t { get; set; }
        public IncludeInfo Next { get; set; }
        public IncludeInfo Pre { get; set; }

        private bool? _isnotmapped;
        public bool IsNotMapped
        {
            get
            {
                if (_isnotmapped == null)
                {
                    _isnotmapped = mi.GetCustomAttribute<NotMappedAttribute>() != null;
                }
                return _isnotmapped.Value;
            }
        }

        public bool HasNotMapped
        {
            get
            {
                var cur = this;
                while (cur != null)
                {
                    if (cur.IsNotMapped == true)
                    {
                        return true;
                    }
                    cur = cur.Next;
                }
                return false;
            }
        }

        private string _softkey;
        public string SoftKey
        {
            get
            {
                if (_softkey == null)
                {
                    _softkey = InnerType.GetCustomAttribute<SoftKeyAttribute>()?.PropertyName ?? "";

                }
                return _softkey;
            }
        }

        private string _softfk;
        public string SoftFK
        {
            get
            {
                if (_softfk == null)
                {
                    _softfk = mi.GetCustomAttribute<SoftFKAttribute>()?.PropertyName ?? "";

                }
                return _softfk;
            }
        }

        private Type _innerType;
        public Type InnerType
        {
            get
            {
                if (_innerType == null)
                {
                    Type stype = this.t;
                    if (stype.IsList())
                    {
                        stype = stype.GetGenericArguments()[0];
                    }
                    _innerType = stype;
                }
                return _innerType;
            }
        }

        public Expression SoftSelect(ParameterExpression parentpe, IDataContext DC)
        {
            Expression rv = null;

            if (this.IsNotMapped == false)
            {
                rv = Expression.MakeMemberAccess(parentpe, parentpe.Type.GetSingleProperty(this.mi.Name));
            }
            else
            {
                if (this.t.IsList() == false)
                {
                    if (string.IsNullOrEmpty(this.SoftKey))
                    {
                        return rv;
                    }
                    var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(InnerType);
                    rv = Expression.Call(Expression.Constant(DC), set);
                    ParameterExpression pe = Expression.Parameter(InnerType);
                    Expression member = Expression.MakeMemberAccess(pe, InnerType.GetSingleProperty(this.SoftKey));
                    Expression right = Expression.MakeMemberAccess(parentpe, parentpe.Type.GetSingleProperty(this.mi.Name + "Id"));
                    Expression condition = Expression.Equal(member, right);
                    rv = Expression.Call(
                         typeof(Queryable),
                         "Where",
                         new Type[] { InnerType },
                         rv,
                         Expression.Lambda(condition, new ParameterExpression[] { pe }));
                }
                else
                {
                    if (string.IsNullOrEmpty(this.SoftFK))
                    {
                        return rv;
                    }
                    var parentkey = parentpe.Type.GetCustomAttribute<SoftKeyAttribute>()?.PropertyName;
                    if (string.IsNullOrEmpty(parentkey))
                    {
                        return rv;
                    }
                    var set = DC.GetType().GetMethod("Set", Type.EmptyTypes).MakeGenericMethod(InnerType);
                    rv = Expression.Call(Expression.Constant(DC), set);
                    ParameterExpression pe = Expression.Parameter(InnerType);
                    Expression member = Expression.MakeMemberAccess(pe, InnerType.GetSingleProperty(this.SoftFK));
                    Expression right = Expression.MakeMemberAccess(parentpe, parentpe.Type.GetSingleProperty(parentkey));
                    Expression condition = Expression.Equal(member, right);
                    rv = Expression.Call(
                         typeof(Queryable),
                         "Where",
                         new Type[] { InnerType },
                         rv,
                         Expression.Lambda(condition, new ParameterExpression[] { pe }));
                }
            }
            if (this.Next != null)
            {
                ParameterExpression pe = Expression.Parameter(this.InnerType);
                NewExpression newItem = Expression.New(this.InnerType);
                var pros = this.InnerType.GetAllProperties();
                List<MemberBinding> bindExps = new List<MemberBinding>();
                foreach (var pro in pros)
                {
                    if (pro.GetCustomAttribute<NotMappedAttribute>() == null)
                    {
                        var right = Expression.MakeMemberAccess(pe, pro);
                        MemberBinding bind = Expression.Bind(pro, right);
                        bindExps.Add(bind);
                    }
                    else if (this.Next.mi.Name == pro.Name)
                    {
                        var right = this.Next.SoftSelect(pe, DC);
                        MemberBinding bind = Expression.Bind(pro, right);
                        bindExps.Add(bind);
                    }
                }

                MemberInitExpression init = Expression.MemberInit(newItem, bindExps);
                var lambda = Expression.Lambda(init, pe);

                rv = Expression.Call(
               typeof(Queryable),
               "Select",
               new Type[] { InnerType, InnerType },
               rv,
               lambda);
            }

            if (this.t.IsList() == false)
            {
                rv = Expression.Call(
                   typeof(Enumerable),
                   "FirstOrDefault",
                   new Type[] { InnerType },
                   rv);
            }
            else
            {
                rv = Expression.Call(
              typeof(Enumerable),
              "ToList",
              new Type[] { InnerType },
              rv);
            }
            return rv;
        }
    }
}
