using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 重复数据组
    /// </summary>
    /// <typeparam name="T">重复数据类</typeparam>
    public class DuplicatedGroup<T>
    {
        public List<DuplicatedField<T>> Fields { get; set; }
    }

    /// <summary>
    /// 重复数据信息
    /// </summary>
    /// <typeparam name="T">数据类</typeparam>
    public class DuplicatedInfo<T>
    {
        //重复数据分组
        public List<DuplicatedGroup<T>> Groups { get; set; }

        public DuplicatedInfo()
        {
            Groups = new List<DuplicatedGroup<T>>();
        }

        /// <summary>
        /// 添加一组重复信息，一组中的多个字段必须同时重复才认为是重复数据
        /// </summary>
        /// <param name="FieldExps">一个或多个重复数据字段</param>
        public DuplicatedInfo<T> AddGroup(params DuplicatedField<T>[] FieldExps)
        {
            DuplicatedGroup<T> newGroup = new DuplicatedGroup<T>()
            {
                Fields = new List<DuplicatedField<T>>()
            };
            foreach (var exp in FieldExps)
            {
                newGroup.Fields.Add(exp);
            }
            Groups.Add(newGroup);
            return this;
        }
    }

    /// <summary>
    /// 简单重复数据字段信息
    /// </summary>
    /// <typeparam name="T">重复数据类</typeparam>
    public class DuplicatedField<T>
    {
        //直接可顺序关联出的字段
        protected Expression<Func<T, object>> _directFieldExp { get; set; }

        public virtual List<PropertyInfo> GetProperties()
        {
            List<PropertyInfo> rv = new List<PropertyInfo>
            {
                PropertyHelper.GetPropertyInfo(_directFieldExp)
            };
            return rv;
        }


        /// <summary>
        /// 根据设定的字段，生成查询重复数据的Lambda，最终返回类似 x=>x.property == val的lambda
        /// </summary>
        /// <param name="Entity">要验证字段的实体类</param>
        /// <param name="para">ParameterExpression</param>
        /// <returns></returns>
        public virtual Expression GetExpression(T Entity, ParameterExpression para)
        {
            var propName = PropertyHelper.GetPropertyName(_directFieldExp);
            var prop = PropertyHelper.GetPropertyInfo(_directFieldExp);
            var func = _directFieldExp.Compile();
            var val = func.Invoke(Entity);

            ////如果字段值为null则跳过，因为一般情况下null值不会被认为重复
            //if (val == null)
            //{
            //    return res;
            //}

            //如果字段值是空字符串，则跳过
            if (val is string && val.ToString() == string.Empty)
            {
                var requiredAttrs = prop.GetCustomAttributes(typeof(RequiredAttribute), false).ToList();

                if (requiredAttrs == null || requiredAttrs.Count == 0)
                {
                    return null;
                }
                else
                {
                    var requiredAtt = requiredAttrs[0] as RequiredAttribute;
                    if (requiredAtt.AllowEmptyStrings == true)
                    {
                        return null;
                    }
                }
            }
            //生成一个表达式，类似于 x=>x.field == val
            var splits = propName.Split('.');
            Expression left = Expression.PropertyOrField(para, splits[0]);
            for (int i = 1; i < splits.Length; i++)
            {
                left = Expression.PropertyOrField(left, splits[i]);
            }

            if (val != null && left.Type.IsGeneric(typeof(Nullable<>)))
            {
                left = Expression.Property(left, "Value");
            }
            if (left.Type == typeof(string))
            {
                left = Expression.Call(left, typeof(String).GetMethod("Trim", Type.EmptyTypes));
            }
            if (val is string)
            {
                val = val.ToString().Trim();
            }
            var right = Expression.Constant(val);
            var equal = Expression.Equal(left, right);
            return equal;
        }

        protected DuplicatedField()
        {

        }

        /// <summary>
        /// 创建一个包含可顺序关联出字段的简单重复字段信息
        /// </summary>
        /// <param name="FieldExp">字段</param>
        /// <returns>字段信息</returns>
        public DuplicatedField(Expression<Func<T, object>> FieldExp)
        {
            _directFieldExp = FieldExp;
        }

    }

    /// <summary>
    /// 复杂重复字段信息接口
    /// </summary>
    public interface IComplexDuplicatedField
    {
        Type GetMiddleTableType();
    }


    /// <summary>
    /// 复杂重复数据字段信息
    /// </summary>
    /// <typeparam name="T">重复数据类</typeparam>
    /// <typeparam name="V">重复数据关联的List中的类</typeparam>
    public class ComplexDuplicatedField<T, V> : DuplicatedField<T>, IComplexDuplicatedField
    {
        /// <summary>
        /// 中间字段
        /// </summary>
        private Expression<Func<T, List<V>>> _middleExp { get; set; }
        /// <summary>
        /// 最终字段
        /// </summary>
        private List<Expression<Func<V, object>>> _subFieldExps { get; set; }

        protected ComplexDuplicatedField()
        {

        }

        /// <summary>
        /// 创建一个复杂字段
        /// </summary>
        /// <param name="MiddleExp">中间字段类</param>
        /// <param name="FieldExps">最终字段类</param>
        /// <returns></returns>
        public ComplexDuplicatedField(Expression<Func<T, List<V>>> MiddleExp, params Expression<Func<V, object>>[] FieldExps)
        {
            _middleExp = MiddleExp;
            _subFieldExps = new List<Expression<Func<V, object>>>();
            _subFieldExps.AddRange(FieldExps);
        }

        public Type GetMiddleTableType()
        {
            return typeof(V);
        }

        /// <summary>
        /// 生成验证复杂字段是否重复的Lambda
        /// </summary>
        /// <param name="Entity">源数据</param>
        /// <param name="para">源数据类型</param>
        /// <returns>Where语句</returns>
        public override Expression GetExpression(T Entity, ParameterExpression para)
        {
            ParameterExpression midPara = Expression.Parameter(typeof(V), "tm2");
            //获取中间表的List
            var list = _middleExp.Compile().Invoke(Entity);
            if (list == null)
            {
                return null;
            }
            List<Expression> allExp = new List<Expression>();
            Expression rv = null;
            //循环中间表数据
            foreach (var li in list)
            {
                List<Expression> innerExp = new List<Expression>();
                bool needBreak = false;
                //循环中间表要检查重复的字段
                foreach (var SubFieldExp in _subFieldExps)
                {
                    //拼接字段表达式，使left等于类似 x.field 的形式
                    Expression left = Expression.Property(midPara, SubFieldExp.GetPropertyName());
                    //如果字段是nullable类型的，则拼接value，形成类似 x.field.Value的形式
                    if (left.Type.IsGeneric(typeof(Nullable<>)) == true)
                    {
                        left = Expression.Property(left, "Value");
                    }
                    //如果字段是string类型，则拼接trim，形成类似 x.field.Trim()的形式
                    if (left.Type == typeof(string))
                    {
                        left = Expression.Call(left, typeof(String).GetMethod("Trim", Type.EmptyTypes));
                    }
                    //使用当前循环的中间表的数据获取字段的值
                    object vv = SubFieldExp.Compile().Invoke(li);
                    //如果值为空则跳过
                    if (vv == null)
                    {
                        needBreak = true;
                        continue;
                    }
                    //如果值为空字符串且没要求必填，则跳过
                    if (vv is string && vv.ToString() == "")
                    {
                        var requiredAttrs = li.GetType().GetProperty(SubFieldExp.GetPropertyName()).GetCustomAttributes(typeof(RequiredAttribute), false).ToList();

                        if (requiredAttrs == null || requiredAttrs.Count == 0)
                        {
                            needBreak = true;
                            continue;
                        }
                        else
                        {
                            var requiredAtt = requiredAttrs[0] as RequiredAttribute;
                            if (requiredAtt.AllowEmptyStrings == true)
                            {
                                needBreak = true;
                                continue;
                            }
                        }
                    }
                    //如果值为字符串，调用trim函数
                    if (vv is string)
                    {
                        vv = vv.ToString().Trim();
                    }
                    //拼接形成 x.field == value的形式
                    ConstantExpression right = Expression.Constant(vv);
                    BinaryExpression equal = Expression.Equal(left, right);
                    innerExp.Add(equal);
                }
                if (needBreak)
                {
                    continue;
                }
                //拼接多个 x.field==value，形成 x.field==value && x.field1==value1 .....的形式
                Expression exp = null;
                if (innerExp.Count == 1)
                {
                    exp = innerExp[0];
                }
                if (innerExp.Count > 1)
                {
                    exp = Expression.And(innerExp[0], innerExp[1]);
                    for (int i = 2; i < innerExp.Count; i++)
                    {
                        exp = Expression.And(exp, innerExp[i]);
                    }
                }
                //调用any函数，形成 .Any(x=> x.field==value && x.field1==value1....)的形式
                if (exp != null)
                {
                    var any = Expression.Call(
                       typeof(Enumerable),
                       "Any",
                       new Type[] { typeof(V) },
                       Expression.Property(para, _middleExp.GetPropertyName()),
                       Expression.Lambda<Func<V, bool>>(exp, new ParameterExpression[] { midPara }));
                    allExp.Add(any);
                }
            }
            //拼接多个any函数形成 .Any(x=> x.field==value && x.field1==value1....) || .Any(x=> x.field==value && x.field1==value1....)的形式并返回
            if (allExp.Count == 1)
            {
                rv = allExp[0];
            }
            if (allExp.Count > 1)
            {
                rv = Expression.Or(allExp[0], allExp[1]);
                for (int i = 2; i < allExp.Count; i++)
                {
                    rv = Expression.Or(rv, allExp[i]);
                }
            }
            return rv;
        }

        /// <summary>
        /// 获取字段属性
        /// </summary>
        /// <returns>字段属性列表</returns>
        public override List<PropertyInfo> GetProperties()
        {
            List<PropertyInfo> rv = new List<PropertyInfo>();
            foreach (var subField in _subFieldExps)
            {
                var pro = subField.GetPropertyInfo();
                if (pro != null)
                {
                    rv.Add(pro);
                }
            }
            return rv;
        }
    }
}
