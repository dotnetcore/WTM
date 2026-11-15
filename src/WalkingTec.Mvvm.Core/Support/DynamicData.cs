using System;
using System.Collections.Generic;
using System.Dynamic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 动态数据类，基于DynamicObject实现，允许在运行时动态添加、读取和调用成员
    /// </summary>
    public class DynamicData : DynamicObject
    {
        /// <summary>
        /// 动态字段字典，保存所有动态添加的成员及其值
        /// </summary>
        public Dictionary<string, object> Fields = new Dictionary<string, object>();

        /// <summary>
        /// 动态字段数量
        /// </summary>
        public int Count { get { return Fields.Keys.Count; } }

        /// <summary>
        /// 添加一个动态字段；同名字段已存在时更新其值
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="val">字段值，默认为null</param>
        public void Add(string name, object val = null)
        {
            if (!Fields.ContainsKey(name))
            {
                Fields.Add(name, val);
            }
            else
            {
                Fields[name] = val;
            }
        }

        /// <summary>
        /// 动态获取成员值，对应动态对象上的属性读取操作
        /// </summary>
        /// <param name="binder">动态成员绑定信息</param>
        /// <param name="result">获取到的字段值</param>
        /// <returns>字段存在返回true，否则交给基类处理</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Fields.ContainsKey(binder.Name))
            {
                result = Fields[binder.Name];
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        /// <summary>
        /// 动态设置成员值，对应动态对象上的属性赋值操作，字段不存在时自动添加
        /// </summary>
        /// <param name="binder">动态成员绑定信息</param>
        /// <param name="value">要设置的字段值</param>
        /// <returns>始终返回true</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!Fields.ContainsKey(binder.Name))
            {
                Fields.Add(binder.Name, value);
            }
            else
            {
                Fields[binder.Name] = value;
            }
            return true;
        }

        /// <summary>
        /// 动态调用成员，当字段中保存的是委托时执行该委托
        /// </summary>
        /// <param name="binder">动态成员调用绑定信息</param>
        /// <param name="args">调用参数</param>
        /// <param name="result">委托执行结果</param>
        /// <returns>字段为委托时返回true，否则交给基类处理</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (Fields.ContainsKey(binder.Name) &&
                Fields[binder.Name] is Delegate)
            {
                Delegate del = Fields[binder.Name] as Delegate;
                result = del.DynamicInvoke(args);
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }
    }
}
