using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// 将数据的List转化为下拉菜单数据List
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="self">源数据List</param>
        /// <param name="textField">指向text字段的表达式</param>
        /// <param name="valueField">指向value字段的表达式</param>
        /// <param name="selectedCondition">默认被选中的条件</param>
        /// <returns>下拉菜单数据List</returns>
        public static List<ComboSelectListItem> ToListItems<T>(this List<T> self
            , Expression<Func<T, object>> textField
            , Expression<Func<T, object>> valueField
            , Expression<Func<T, bool>> selectedCondition = null)
        {
            var rv = new List<ComboSelectListItem>();
            if (self != null)
            {
                //循环列表中的数据
                foreach (var item in self)
                {
                    //获取textField的值作为text
                    string text = textField.Compile().Invoke(item).ToString();
                    //获取valueField的值作为value
                    string value = valueField.Compile().Invoke(item).ToString();
                    //添加到下拉菜单List中
                    ComboSelectListItem li = new ComboSelectListItem();
                    li.Text = text;
                    li.Value = value;
                    //如果有默认选择的条件，则将当前数据带入到判断表达式中，如果返回true，则将下拉数据的selected属性设为true
                    if (selectedCondition != null)
                    {
                        if (selectedCondition.Compile().Invoke(item))
                        {
                            li.Selected = true;
                        }
                    }
                    rv.Add(li);
                }
            }
            return rv;
        }

    }
}
