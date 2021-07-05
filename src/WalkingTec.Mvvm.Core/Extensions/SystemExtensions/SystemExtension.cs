using System;
using System.Reflection;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// System Extension
    /// </summary>
    public static class SystemExtension
    {
        #region Guid Extensions

        public static string ToNoSplitString(this Guid self)
        {
            return self.ToString().Replace("-", string.Empty);
        }

        #endregion

        /// <summary>
        /// 将CrudVM中Entity的关联字段设为空并返回一个新的CrudVM
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static object GetCleanCrudVM(this object self)
        {
            var mtype = self.GetType();
            if(typeof(IBaseCRUDVM<TopBasePoco>).IsAssignableFrom(mtype))
            {
                var rv = mtype.GetConstructor(Type.EmptyTypes).Invoke(null);
                var toppros = mtype.GetAllProperties();
                foreach (var tpro in toppros)
                {
                    if(tpro.Name == "Entity")
                    {
                        var entity = tpro.GetValue(self);
                        var pros = tpro.PropertyType.GetAllProperties();
                        var newEntity = tpro.PropertyType.GetConstructor(Type.EmptyTypes).Invoke(null);
                        bool isBasePoco = typeof(IBasePoco).IsAssignableFrom(tpro.PropertyType);
                        //将所有TopBasePoco的属性赋空值，防止添加关联的重复内容
                        foreach (var pro in pros)
                        {
                            if (pro.PropertyType.GetTypeInfo().IsSubclassOf(typeof(TopBasePoco)) == false)
                            {
                                if (isBasePoco == false || (pro.Name != "UpdateTime" && pro.Name != "UpdateBy"))
                                {
                                    pro.SetValue(newEntity, pro.GetValue(entity));
                                }
                            }
                        }
                        tpro.SetValue(rv, newEntity);
                    }
                    else
                    {
                        if (tpro.CanWrite)
                        {
                            tpro.SetValue(rv, tpro.GetValue(self));
                        }
                    }
                }
                return rv;
            }
            return null;
        }

    }
}
