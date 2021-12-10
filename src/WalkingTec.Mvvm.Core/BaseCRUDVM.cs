using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
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
using WalkingTec.Mvvm.Core.Support.FileHandlers;

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
            //建立基础查询
            var query = DC.Set<TModel>().AsQueryable();
            //循环添加其他设定的Include
            if (_toInclude != null)
            {
                foreach (var item in _toInclude)
                {                    
                    query = query.Include(item);
                }
            }
            if (typeof(IPersistPoco).IsAssignableFrom(typeof(TModel)))
            {
                var mod = new IsValidModifier();
                var newExp = mod.Modify(query.Expression);
                query = query.Provider.CreateQuery<TModel>(newExp) as IOrderedQueryable<TModel>;
            }

            //获取数据
            rv = query.CheckID(Id).AsNoTracking().SingleOrDefault();
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
                        //界面传过来的子表数据
                        IEnumerable<TopBasePoco> list = pro.GetValue(Entity) as IEnumerable<TopBasePoco>;
                        if (list != null && list.Count() > 0)
                        {
                            string fkname = DC.GetFKName<TModel>(pro.Name);
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
                                                itempro.SetValue(newitem, Entity.GetID());
                                                found = true;
                                            }
                                            catch { }
                                        }
                                    }
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
            pros = pros.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(NotMappedAttribute)) == false).ToList();
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

                        if (pro.GetValue(Entity) is IEnumerable<TopBasePoco> list && list.Count() > 0)
                        {
                            //获取外键字段名称
                            string fkname = DC.GetFKName<TModel>(pro.Name);
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
                                            itempro.SetValue(newitem, Entity.GetID());
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


                            TModel _entity = null;
                            //打开新的数据库联接,获取数据库中的主表和子表数据
                            //using (var ndc = DC.CreateNew())
                            //{
                            _entity = DC.Set<TModel>().Include(pro.Name).AsNoTracking().CheckID(Entity.GetID()).FirstOrDefault();
                            //}
                            if (_entity == null)
                            {
                                MSD.AddModelError(" ", Localizer["Sys.EditFailed"]);
                                return;
                            }
                            //比较子表原数据和新数据的区别
                            IEnumerable<TopBasePoco> toadd = null;
                            IEnumerable<TopBasePoco> toremove = null;
                            IEnumerable<TopBasePoco> data = _entity.GetType().GetSingleProperty(pro.Name).GetValue(_entity) as IEnumerable<TopBasePoco>;
                            Utils.CheckDifference(data, list, out toremove, out toadd);
                            //设定子表应该更新的字段
                            List<string> setnames = new List<string>();
                            foreach (var field in FC.Keys)
                            {
                                var f = field.ToLower();
                                if (f.StartsWith("entity." + pro.Name.ToLower() + "[0]."))
                                {
                                    string name = f.Replace("entity." + pro.Name.ToLower() + "[0].", "");
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
                                                if (itempro.Name != "ID" && notmapped == null && itempro.PropertyType.IsList() == false)
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
                                DC.AddEntity(item);
                            }
                        }
                        else if ((pro.GetValue(Entity) is IEnumerable<TopBasePoco> list2 && list2?.Count() == 0))
                        {
                            var itemPros = ftype.GetAllProperties();
                            var _entity = DC.Set<TModel>().Include(pro.Name).AsNoTracking().CheckID(Entity.GetID()).FirstOrDefault();
                            if (_entity != null)
                            {
                                IEnumerable<TopBasePoco> removeData = _entity.GetType().GetSingleProperty(pro.Name).GetValue(_entity) as IEnumerable<TopBasePoco>;
                                //如果是PersistPoco，则把IsValid设为false，并不进行物理删除
                                if (removeData is IEnumerable<IPersistPoco> removePersistPocoData)
                                {
                                    foreach (var item in removePersistPocoData)
                                    {
                                        (item as IPersistPoco).IsValid = false;
                                        if (typeof(IBasePoco).IsAssignableFrom(item.GetType()))
                                        {
                                            (item as IBasePoco).UpdateTime = DateTime.Now;
                                            (item as IBasePoco).UpdateBy = LoginUserInfo?.ITCode;
                                        }
                                        dynamic i = item;
                                        DC.UpdateEntity(i);
                                    }
                                }
                                else
                                {
                                    foreach (var item in removeData)
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
            }
            #endregion


            if (updateAllFields == false)
            {
                foreach (var field in FC.Keys)
                {
                    var f = field.ToLower();
                    if (f.StartsWith("entity.") && !f.Contains("["))
                    {
                        string name = f.Replace("entity.", "");
                        try
                        {
                            DC.UpdateProperty(Entity, pros.Where(x => x.Name.ToLower() == name).Select(x => x.Name).FirstOrDefault());
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
                var baseExp = DC.Set<TModel>().AsQueryable();
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
                    //如果要求判断id不重复，则去掉id不相等的判断，加入id相等的判断
                    if (props.Any(x => x.Name.ToLower() == "id"))
                    {
                        conditions.RemoveAt(0);
                        BinaryExpression idEqual = Expression.Equal(idLeft, idRight);
                        conditions.Insert(0, idEqual);
                    }
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

    }
}
