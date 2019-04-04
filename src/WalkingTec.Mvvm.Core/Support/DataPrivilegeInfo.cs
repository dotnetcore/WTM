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
        //获取数据权限的下拉菜单
        List<ComboSelectListItem> GetItemList(IDataContext dc, LoginUserInfo user);
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

        public DataPrivilegeInfo(string name, Expression<Func<T, string>> displayField, Expression<Func<T, bool>> where = null)
        {
            ModelName = typeof(T).Name;
            PrivillegeName = name;
            _displayField = displayField;
            _where = where;
        }

        /// <summary>
        /// 获取数据权限的下拉菜单
        /// </summary>
        /// <param name="dc">dc</param>
        /// <param name="user">user</param>
        /// <returns>数据权限关联表的下拉菜单</returns>
        public List<ComboSelectListItem> GetItemList(IDataContext dc, LoginUserInfo user)
        {
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            if (user.Roles?.Where(x => x.RoleCode == "001").FirstOrDefault() == null && user.DataPrivileges?.Where(x=>x.RelateId == null).FirstOrDefault() == null)
            {
                rv = dc.Set<T>().Where(x=>user.DataPrivileges.Select(y=>y.RelateId).Contains(x.ID)).GetSelectListItems(null, _where, _displayField, null, ignorDataPrivilege: true);
            }
            else
            {
                rv = dc.Set<T>().GetSelectListItems(null, _where, _displayField, null, ignorDataPrivilege: true);
            }
            return rv;
        }
    }


}
