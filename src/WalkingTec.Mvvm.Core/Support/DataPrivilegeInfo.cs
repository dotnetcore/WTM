using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 数据权限信息接口
    /// </summary>
    public interface IDataPrivilege
    {
        //数据权限关联的表名
        string ModelName { get; set; }
        //数据权限名称
        string PrivillegeName { get; set; }

        /// <summary>
        /// 数据权限关联的表类型
        /// </summary>
        Type ModelType { get; set; }
        //获取数据权限的下拉菜单
        List<ComboSelectListItem> GetItemList (WTMContext wtmcontext, string filter = null, List<string> ids = null);
        /// <summary>
        /// 获取数据权限对应树形数据的所有父节点ID
        /// </summary>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="dps">数据权限列表</param>
        /// <returns>父节点ID列表</returns>
        List<string> GetTreeParentIds(WTMContext wtmcontext, List<DataPrivilege> dps);
        /// <summary>
        /// 获取数据权限对应树形数据的所有子节点ID
        /// </summary>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="pids">父节点ID列表</param>
        /// <returns>子节点ID列表</returns>
        List<string> GetTreeSubIds(WTMContext wtmcontext, List<string> pids);
    }

    /// <summary>
    /// 数据权限信息
    /// </summary>
    /// <typeparam name="T">关联表类</typeparam>
    public class DataPrivilegeInfo<T> : IDataPrivilege where T : TopBasePoco
    {
        //数据权限关联的表名
        public string ModelName { get; set; }
        //数据权限名称
        public string PrivillegeName { get; set; }
        //显示字段
        private Expression<Func<T, string>> _displayField;
        //where过滤条件
        private Expression<Func<T, bool>> _where;
        /// <summary>
        /// 数据权限关联的表类型
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// 构造数据权限信息
        /// </summary>
        /// <param name="name">数据权限名称</param>
        /// <param name="displayField">关联表数据的显示字段</param>
        /// <param name="where">关联表数据的过滤条件</param>
        public DataPrivilegeInfo(string name, Expression<Func<T, string>> displayField, Expression<Func<T, bool>> where = null)
        {
            ModelType = typeof(T);
            ModelName = ModelType.Name;
            PrivillegeName = name;
            _displayField = displayField;
            _where = where;
        }

        /// <summary>
        /// 获取数据权限的下拉菜单
        /// </summary>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="filter">filter</param>
        /// <param name="ids">ids</param>
        /// <returns>数据权限关联表的下拉菜单</returns>
        public List<ComboSelectListItem> GetItemList(WTMContext wtmcontext, string filter = null,List<string> ids= null)
        {
            var user = wtmcontext?.LoginUserInfo;
            Expression<Func<T, bool>> where = null;

            if (ids != null)
            {
                where = ids.GetContainIdExpression<T>();
            }
            else
            {
                if (string.IsNullOrEmpty(filter) == false)
                {
                    //动态拼接 显示字段.ToLower().Contains(filter) 形式的过滤条件，并与自定义where条件合并
                    ChangePara cp = new ChangePara();
                    ParameterExpression pe = Expression.Parameter(typeof(T));
                    //var toString = Expression.Call(cp.Change(_displayField.Body, pe), "Cast", new Type[] { typeof(string)});
                    var tolower = Expression.Call(cp.Change(_displayField.Body, pe), "ToLower", new Type[] { });
                    var exp = Expression.Call(tolower, "Contains", null, Expression.Constant(filter.ToLower()));
                    where = Expression.Lambda<Func<T, bool>>(exp, pe);
                    if (_where != null)
                    {
                        var temp = cp.Change(_where.Body, pe);
                        var together = Expression.AndAlso(where.Body, temp);
                        where = Expression.Lambda<Func<T, bool>>(together, pe);
                    }
                }
                else
                {
                    where = _where;
                }
                if (where == null)
                {
                    where = x => 1 == 1;
                }
            }
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            //非管理员角色(001)且未拥有全表数据权限时，按用户数据权限限定的ID范围过滤下拉数据
            if (user.Roles?.Where(x => x.RoleCode == "001").FirstOrDefault() == null && user.DataPrivileges?.Where(x => x.RelateId == null).FirstOrDefault() == null)
            {
                rv = wtmcontext.DC.Set<T>().CheckIDs(user.DataPrivileges.Select(y => y.RelateId).ToList()).Where(where).GetSelectListItems(wtmcontext, _displayField, null, ignorDataPrivilege: true);
            }
            else
            {
                rv = wtmcontext.DC.Set<T>().Where(where).GetSelectListItems(wtmcontext, _displayField, null, ignorDataPrivilege: true);
            }
            return rv;
        }

        /// <summary>
        /// 获取数据权限对应树形数据的所有父节点ID
        /// </summary>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="dps">数据权限列表</param>
        /// <returns>父节点ID列表</returns>
        public List<string> GetTreeParentIds(WTMContext wtmcontext, List<DataPrivilege> dps)
        {
            //筛选本表的数据权限，查出其关联数据中ParentId非空的值作为父节点ID
            var ids = dps.Where(x => x.TableName == this.ModelName).Select(x => x.RelateId).ToList();
            var idscheck = ids.GetContainIdExpression<T>();
            var modified = wtmcontext.DC.Set<T>().Where(idscheck).CheckNotNull("ParentId").DynamicSelect("ParentId");
            var skipids = modified.ToList();
            return skipids;
        }

        /// <summary>
        /// 获取数据权限对应树形数据的所有子节点ID
        /// </summary>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="pids">父节点ID列表</param>
        /// <returns>子节点ID列表</returns>
        public List<string> GetTreeSubIds(WTMContext wtmcontext, List<string> pids)
        {
            //以ParentId属性为条件构建过滤表达式，查询所有子节点的ID
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression parentid = Expression.Property(pe, typeof(T).GetSingleProperty("ParentId"));
            return wtmcontext.DC.Set<T>().Where(pids.GetContainIdExpression<T>(parentid)).DynamicSelect("ID").ToList();
        }
    }


}
