using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 批量操作VM接口
    /// </summary>
    /// <typeparam name="TEditModel">批量修改的VM</typeparam>
    public interface IBaseBatchVM<out TEditModel>
        where TEditModel : BaseVM
    {
        /// <summary>
        /// 批量修改的VM
        /// </summary>
        TEditModel LinkedVM { get; }

        /// <summary>
        /// 批量列表VM
        /// </summary>
        IBasePagedListVM<TopBasePoco, ISearcher> ListVM { get; }

        /// <summary>
        /// 列表数据的Id数组
        /// </summary>
        //IEnumerable<Guid> Ids { get; set; }
        string[] Ids { get; set; }

        /// <summary>
        /// 批量操作的错误
        /// </summary>
        Dictionary<string, string> ErrorMessage { get; set; }
    }

    /// <summary>
    /// 批量操作的基础VM，所有批量操作的VM应继承本类
    /// </summary>
    /// <typeparam name="TModel">批量修改的VM</typeparam>
    /// <typeparam name="TLinkModel">批量列表VM</typeparam>
    public class BaseBatchVM<TModel, TLinkModel> : BaseVM, IBaseBatchVM<TLinkModel> where TModel : TopBasePoco,new() where TLinkModel : BaseVM
    {
        /// <summary>
        /// 批量修改的VM
        /// </summary>
        public TLinkModel LinkedVM { get; set; }

        /// <summary>
        /// 批量列表VM
        /// </summary>
        public IBasePagedListVM<TopBasePoco, ISearcher> ListVM { get; set; }

        /// <summary>
        /// 批量操作的错误
        /// </summary>
        public Dictionary<string, string> ErrorMessage { get; set; }

        /// <summary>
        /// 列表数据的Id数组
        /// </summary>
        public string[] Ids { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseBatchVM()
        {
            //this.Ids = new List<Guid>();
            ErrorMessage = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="e">错误</param>
        /// <param name="id">数据Id</param>
        protected void SetExceptionMessage(Exception e, string id)
        {
            if (id != null)
            {
                ErrorMessage.Add(id, e.Message);
            }
        }

        /// <summary>
        /// 检查是否可以删除，当进行批量删除操作时会调用本函数。子类如果有特殊判断应重载本函数
        /// </summary>
        /// <param name="id">数据Id</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true代表可以删除，false代表不能删除</returns>
        protected virtual bool CheckIfCanDelete(object id, out string errorMessage)
        {
            errorMessage = null;
            return true;
        }

        /// <summary>
        /// 批量删除，默认对Ids中包含的主键的数据进行删除。子类如果有特殊判断应重载本函数
        /// </summary>
        /// <returns>true代表成功，false代表失败</returns>
        public virtual bool DoBatchDelete()
        {
            bool rv = true;
            //循环所有数据Id
            List<string> idsData = Ids.ToList();
            for (int i = 0; i < idsData.Count; i++)
            {
                string checkErro = null;
                //检查是否可以删除，如不能删除则直接跳过
                if (CheckIfCanDelete(idsData[i], out checkErro) == false)
                {
                    ErrorMessage.Add(idsData[i], checkErro);
                    rv = false;
                    break;
                }
                //进行删除
                try
                {
                    var ctor = typeof(TModel).GetConstructor(Type.EmptyTypes);
                    if (typeof(TModel).IsSubclassOf(typeof(PersistPoco)))
                    {
                        var pp = DC.Set<TModel>().CheckID(idsData[i]).FirstOrDefault();
                        (pp as PersistPoco).IsValid = false;
                        (pp as PersistPoco).UpdateTime = DateTime.Now;
                        (pp as PersistPoco).UpdateBy = LoginUserInfo.ITCode;
                        DC.UpdateProperty(pp, "IsValid");
                        DC.UpdateProperty(pp, "UpdateTime");
                        DC.UpdateProperty(pp, "UpdateBy");
                    }
                    else
                    {
                        TModel m = ctor.Invoke(null) as TModel;

                        m.SetPropertyValue("ID", idsData[i]);
                        DC.Set<TModel>().Attach(m);
                        DC.DeleteEntity(m);
                    }
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, idsData[i]);
                    rv = false;
                }
            }
            //进行数据库的删除操作
            if (rv == true)
            {
                try
                {
                    DC.SaveChanges();
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, null);
                    rv = false;
                }
            }
            //如果失败，添加错误信息
            if (rv == false)
            {
                if (ErrorMessage.Count > 0)
                {
                    foreach (var id in idsData)
                    {
                        if (!ErrorMessage.ContainsKey(id))
                        {
                            ErrorMessage.Add(id, Program._localizer["Rollback"]);
                        }
                    }
                }
                ListVM?.DoSearch();
                if (ListVM != null)
                {
                    foreach (var item in ListVM?.GetEntityList())
                    {
                        item.BatchError = ErrorMessage.Where(x => x.Key == item.GetID().ToString()).Select(x => x.Value).FirstOrDefault();
                    }
                }
                MSD.AddModelError("", Program._localizer["DataCannotDelete"]);
            }
            return rv;
        }


        /// <summary>
        /// 批量修改，默认对Ids中包含的数据进行修改，子类如果有特殊判断应重载本函数
        /// </summary>
        /// <returns>true代表成功，false代表失败</returns>
        public virtual bool DoBatchEdit()
        {
            //获取批量修改VM的所有属性
            var pros = LinkedVM.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            bool rv = true;
            List<string> idsData = Ids.ToList();
            //找到对应的BaseCRUDVM，并初始化
            var vmtype = this.GetType().Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(typeof(BaseCRUDVM<TModel>))).FirstOrDefault();
            IBaseCRUDVM<TModel> vm = null;
            if (vmtype != null)
            {
                vm = vmtype.GetConstructor(System.Type.EmptyTypes).Invoke(null) as IBaseCRUDVM<TModel>;
                vm.CopyContext(this);
            }
            //循环所有数据
            for (int i = 0; i < idsData.Count; i++)
            {
                try
                {
                    //如果找不到对应数据，则输出错误
                    TModel entity = null;
                    entity = DC.Set<TModel>().CheckID(idsData[i]).FirstOrDefault();
                    if (vm != null)
                    {
                        vm.SetEntity(entity);
                    }
                    if (entity == null)
                    {
                        ErrorMessage.Add(idsData[i], Program._localizer["DataNotExist"]);
                        rv = false;
                        break;
                    }
                    //如果能找到，则循环LinkedVM中的属性，给entity中同名属性赋值
                    foreach (var pro in pros)
                    {
                        var proToSet = entity.GetType().GetSingleProperty(pro.Name);
                        var val = FC.ContainsKey("LinkedVM." + pro.Name) ? FC["LinkedVM." + pro.Name] : null;
                        if (proToSet != null && val != null)
                        {
                            var hasvalue = true;
                            if ( val is StringValues sv && StringValues.IsNullOrEmpty(sv) == true)
                            {
                                hasvalue = false;
                            }
                            if (hasvalue)
                            {
                                proToSet.SetValue(entity, pro.GetValue(LinkedVM));
                            }
                        }
                    }

                    //调用controller方法验证model
                    try
                    {
                        Controller.GetType().GetMethod("RedoValidation").Invoke(Controller, new object[] { entity });
                    }
                    catch { }
                    //如果有对应的BaseCRUDVM则使用其进行数据验证
                    if (vm != null)
                    {
                        vm.Validate();
                        var errors = vm.MSD;
                        if (errors != null && errors.Count > 0)
                        {
                            var error = "";
                            foreach (var key in errors.Keys)
                            {
                                if (errors[key].Count > 0)
                                {
                                    error += errors[key].Select(x => x.ErrorMessage).ToSpratedString();
                                }
                            }
                            if (error != "")
                            {
                                ErrorMessage.Add(idsData[i], error);
                                rv = false;
                                break;
                            }
                        }
                    }
                    if (typeof(TModel).IsSubclassOf(typeof(BasePoco)))
                    {
                        BasePoco ent = entity as BasePoco;
                        if (ent.UpdateTime == null)
                        {
                            ent.UpdateTime = DateTime.Now;
                        }
                        if (string.IsNullOrEmpty(ent.UpdateBy))
                        {
                            ent.UpdateBy = LoginUserInfo?.ITCode;
                        }
                    }
                    DC.UpdateEntity(entity);
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, idsData[i]);
                    rv = false;
                }
            }
            //进行数据库的修改操作
            if (rv == true)
            {
                try
                {
                    DC.SaveChanges();
                }
                catch (Exception e)
                {
                    SetExceptionMessage(e, null);
                    rv = false;
                }
            }

            //如果有错误，输出错误信息
            if (rv == false)
            {
                if (ErrorMessage.Count > 0)
                {
                    foreach (var id in idsData)
                    {
                        if (!ErrorMessage.ContainsKey(id))
                        {
                            ErrorMessage.Add(id, Program._localizer["Rollback"]);
                        }
                    }
                }
                ListVM.DoSearch();
                foreach (var item in ListVM.GetEntityList())
                {
                    item.BatchError = ErrorMessage.Where(x => x.Key == item.GetID().ToString()).Select(x => x.Value).FirstOrDefault();
                }
            }
            return rv;
        }
    }
}
