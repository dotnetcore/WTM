using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class EnumExtension
    {
        #region 将枚举类型转化为下拉列表
        /// <summary>
        /// 将枚举类型转化为下拉列表
        /// </summary>
        /// <param name="self">枚举类型</param>
        /// <param name="value">value</param>
        /// <returns>下拉菜单数据列表</returns>
        public static List<ComboSelectListItem> ToListItems(this Type self,object value=null, bool pleaseSelect = false)
        {
            string[] names = null;
            //如果是枚举
            if (self.IsEnum)
            {
                names = Enum.GetNames(self);
            }
            //如果是nullable的枚举
            if (self.IsGenericType && self.GenericTypeArguments[0].IsEnum)
            {
                names = Enum.GetNames(self.GenericTypeArguments[0]);
            }
            //生成下拉菜单数据
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            if (names != null)
            {
                foreach (var name in names)
                {
                    var newitem = new ComboSelectListItem { Text = PropertyHelper.GetEnumDisplayName(self, name), Value = name };
                    if(value != null && name == value.ToString())
                    {
                        newitem.Selected = true;
                    }
                    rv.Add(newitem);
                }
            }
            if(pleaseSelect == true)
            {
                rv.Insert(0, new ComboSelectListItem { Text = "请选择", Value = "" });
            }
            return rv;
        }
        #endregion
    }
}
