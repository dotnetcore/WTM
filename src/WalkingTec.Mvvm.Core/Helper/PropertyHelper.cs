using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 属性辅助类
    /// </summary>
    public static class PropertyHelper
    {

        /// <summary>
        /// 解析表达式树，获取表达式的根对象（常量表达式的值）
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>根对象的值，解析不到时返回null</returns>
        public static object GetExpressionRootObj(Expression expression)
        {
            if (expression == null)
            {
                return "";
            }
            //从MemberExpression或Lambda表达式的Body中提取MemberExpression（兼容一元转换表达式）
            Expression me = null;
            LambdaExpression le = null;
            if (expression is MemberExpression)
            {
                me = expression as MemberExpression;
            }
            if (expression is LambdaExpression)
            {
                le = expression as LambdaExpression;
                if (le.Body is MemberExpression)
                {
                    me = le.Body as MemberExpression;
                }
                if (le.Body is UnaryExpression)
                {
                    me = (le.Body as UnaryExpression).Operand as MemberExpression;
                }
            }
            //沿表达式树逐级向上追溯，直到找到根部的常量表达式
            while (me != null && me.NodeType == ExpressionType.MemberAccess)
            {
                Expression exp = (me as MemberExpression).Expression;
                if (exp is MemberExpression)
                {
                    me = exp as MemberExpression;
                }
                else if (exp is MethodCallExpression)
                {
                    //遇到数组或字典的索引调用（get_Item）时，跳过索引直接取被索引的成员继续向上追溯
                    var mexp = exp as MethodCallExpression;
                    if (mexp.Method.Name == "get_Item")
                    {
                        object index = 0;
                        if (mexp.Arguments[0] is MemberExpression)
                        {
                            var obj = ((mexp.Arguments[0] as MemberExpression).Expression as ConstantExpression).Value;
                            index = obj.GetType().GetField((mexp.Arguments[0] as MemberExpression).Member.Name).GetValue(obj);
                        }
                        else
                        {
                            index = (mexp.Arguments[0] as ConstantExpression).Value;
                        }
                        me = mexp.Object as MemberExpression;
                    }
                }
                else
                {
                    me = exp;
                    break;
                }
            }
            if(me.NodeType == ExpressionType.Constant)
            {
                return (me as ConstantExpression)?.Value;
            }
            return null;
        }

        /// <summary>
        /// 根据类型和属性名（支持多级属性格式）构建属性读取表达式的委托
        /// </summary>
        /// <param name="objtype">属性所在实例的类型</param>
        /// <param name="property">属性名，支持a.b.c多级格式</param>
        /// <returns>读取属性值的委托</returns>
        public static Func<object, object> GetPropertyExpression(Type objtype, string property)
        {
            //去掉属性名中的数组下标，如xxx[0]中的[0]
            property = Regex.Replace(property, @"\[[^\]]*\]", string.Empty);
            List<string> level = new List<string>();
            if (property.Contains('.'))
            {
                level.AddRange(property.Split('.'));
            }
            else
            {
                level.Add(property);
            }

            //按“.”拆分的属性级别，逐级构建属性访问表达式链
            var pe = Expression.Parameter(objtype);
            var member = Expression.Property(pe, objtype.GetSingleProperty(level[0]));
            for (int i = 1; i < level.Count; i++)
            {
                member = Expression.Property(member, member.Type.GetSingleProperty(level[i]));
            }
            return Expression.Lambda<Func<object, object>>(member, pe).Compile();
        }

        /// <summary>
        /// 获取属性名
        /// </summary>
        /// <param name="expression">属性表达式</param>
        /// <param name="getAll">是否获取全部级别名称，比如a.b.c</param>
        /// <returns>属性名</returns>
        public static string GetPropertyName(this Expression expression, bool getAll = true)
        {
            if (expression == null)
            {
                return "";
            }
            MemberExpression me = null;
            LambdaExpression le = null;
            if (expression is MemberExpression)
            {
                me = expression as MemberExpression;
            }
            if (expression is LambdaExpression)
            {
                le = expression as LambdaExpression;
                if (le.Body is MemberExpression)
                {
                    me = le.Body as MemberExpression;
                }
                else if (le.Body is UnaryExpression)
                {
                    me = (le.Body as UnaryExpression).Operand as MemberExpression;
                }
                else if(le.Body is MethodCallExpression mexp && mexp.Method.Name == "get_Item")
                {
                    object index = 0;
                    me = mexp.Object as MemberExpression;
                }
            }
            string rv = "";
            if (me != null)
            {
                rv = me.Member.Name;
            }
            while (me != null && getAll && me.NodeType == ExpressionType.MemberAccess)
            {
                Expression exp = me.Expression;
                if (exp is MemberExpression)
                {
                    rv = (exp as MemberExpression).Member.Name + "." + rv;
                    me = exp as MemberExpression;
                }
                else if (exp is MethodCallExpression)
                {
                    var mexp = exp as MethodCallExpression;
                    if (mexp.Method.Name == "get_Item")
                    {
                        object index = 0;
                        if (mexp.Arguments[0] is MemberExpression)
                        {
                            var obj = ((mexp.Arguments[0] as MemberExpression).Expression as ConstantExpression).Value;
                            index = obj.GetType().GetField((mexp.Arguments[0] as MemberExpression).Member.Name).GetValue(obj);
                        }
                        else
                        {
                            index = (mexp.Arguments[0] as ConstantExpression).Value;
                        }
                        rv = (mexp.Object as MemberExpression).Member.Name + "[" + index + "]." + rv;
                        me = mexp.Object as MemberExpression;
                    }
                }
                else
                {
                    break;
                }
            }
            return rv;
        }

        /// <summary>
        /// 根据参数表达式和成员表达式构建属性访问表达式
        /// </summary>
        /// <param name="self">参数表达式</param>
        /// <param name="member">成员表达式</param>
        /// <returns>属性访问表达式</returns>
        public static Expression GetMemberExp(this ParameterExpression self, Expression member)
        {
            return self.GetMemberExp(member.GetPropertyName());
        }

        /// <summary>
        /// 根据参数表达式和逗号分隔的属性名链构建属性访问表达式
        /// </summary>
        /// <param name="self">参数表达式</param>
        /// <param name="memberName">逗号分隔的属性名链</param>
        /// <returns>属性访问表达式</returns>
        public static Expression GetMemberExp(this ParameterExpression self, string memberName)
        {
            var names = memberName.Split(',');
            Expression rv = Expression.PropertyOrField(self, names[0]); ;
            for (int i = 1; i < names.Length; i++)
            {
                rv = Expression.PropertyOrField(rv, names[i]);
            }
            return rv;
        }


        /// <summary>
        /// 获取属性名的Id形式，将属性名中的.转换为_，适合作为HTML中的Id使用
        /// </summary>
        /// <param name="expression">属性表达式</param>
        /// <param name="getAll">是否获取全部级别名称，比如a.b.c</param>
        /// <returns>属性Id</returns>
        public static string GetPropertyId(this Expression expression, bool getAll = true)
        {
            return GetPropertyName(expression, getAll).GetIdByName();
        }

        /// <summary>
        /// 获取正则表达式错误
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <returns>错误文本</returns>
        public static string GetRegexErrorMessage(this MemberInfo pi)
        {
            string rv = "";

            if (pi.GetCustomAttributes(typeof(RegularExpressionAttribute), false).FirstOrDefault() is RegularExpressionAttribute dis && !string.IsNullOrEmpty(dis.ErrorMessage))
            {
                rv = dis.ErrorMessage;
                if (CoreProgram._localizer != null)
                {
                    rv = CoreProgram._localizer[rv];
                }
            }
            else
            {
                rv = "";
            }
            return rv;
        }

        /// <summary>
        /// 获取属性显示名称
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <param name="local"></param>
        /// <returns>属性名称</returns>
        public static string GetPropertyDisplayName(this MemberInfo pi, IStringLocalizer local = null)
        {
            string rv = "";
            if (pi.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute dis && !string.IsNullOrEmpty(dis.Name))
            {
                rv = dis.Name;
                if (local == null)
                {
                    if (CoreProgram._localizer != null)
                    {
                        rv = CoreProgram._localizer[rv];
                    }
                }
                else
                {
                    rv = local[rv];
                }
            }
            else
            {
                rv = pi.Name;
            }
            return rv;
        }

        /// <summary>
        /// 获取属性显示名称
        /// </summary>
        /// <param name="expression">属性表达式</param>
        /// <param name="local"></param>
        /// <returns>属性显示名称</returns>
        public static string GetPropertyDisplayName(this Expression expression, IStringLocalizer local = null)
        {
            return GetPropertyDisplayName(expression.GetPropertyInfo(), local);
        }


        /// <summary>
        /// 获取枚举显示名称
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>枚举显示名称</returns>
        public static string GetEnumDisplayName(this Enum value)
        {
            return GetEnumDisplayName(value.GetType(), value.ToString());
        }

        /// <summary>
        /// 获取属性信息
        /// </summary>
        /// <param name="expression">属性表达式</param>
        /// <returns>属性信息</returns>
        public static PropertyInfo GetPropertyInfo(this Expression expression)
        {
            MemberExpression me = null;
            LambdaExpression le = null;
            if (expression is MemberExpression)
            {
                me = expression as MemberExpression;
            }
            if (expression is LambdaExpression)
            {
                le = expression as LambdaExpression;
                if (le.Body is MemberExpression)
                {
                    me = le.Body as MemberExpression;
                }
                if (le.Body is UnaryExpression)
                {
                    me = (le.Body as UnaryExpression).Operand as MemberExpression;
                }
            }
            PropertyInfo rv = null;
            if (me != null)
            {
                rv = me.Member.DeclaringType.GetSingleProperty(me.Member.Name);
            }
            return rv;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="exp">属性表达式</param>
        /// <param name="obj">属性所在实例</param>
        /// <returns>属性值</returns>
        public static object GetPropertyValue(this object obj, LambdaExpression exp)
        {
            //获取表达式的值，并过滤单引号
            try
            {
                var expValue = exp.Compile().DynamicInvoke(obj);
                object val = expValue;
                return val;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 根据属性名获取属性值
        /// </summary>
        /// <param name="obj">属性所在实例</param>
        /// <param name="property">属性名</param>
        /// <returns>属性值，获取失败时返回空字符串</returns>
        public static object GetPropertyValue(this object obj, string property)
        {
            //获取表达式的值，并过滤单引号
            try
            {
                return obj.GetType().GetSingleProperty(property).GetValue(obj); ;
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 获取子表同级别属性的值集合：根据带数组下标形式的属性名，返回列表中所有对象的指定属性值
        /// </summary>
        /// <param name="obj">属性所在实例</param>
        /// <param name="propertyName">带数组下标形式的属性名，如“xxx[0].yyy”</param>
        /// <returns>同级属性值列表，无法解析时返回空列表</returns>
        public static List<string> GetPropertySiblingValues(this object obj, string propertyName)
        {
            if(obj == null)
            {
                return new List<string>();
            }
            //用正则匹配“列表属性[下标].子属性”形式的属性名
            Regex reg = new Regex("(.*?)\\[\\-?\\d?\\]\\.(.*?)$");
            var match = reg.Match(propertyName);
            if (match.Success)
            {
                var name1 = match.Groups[1].Value;
                var name2 = match.Groups[2].Value;

                //按“.”拆分属性路径，逐级构建访问列表属性的表达式链
                var levels = name1.Split('.');
                var objtype = obj.GetType();
                var pe = Expression.Parameter(objtype);
                var member = Expression.Property(pe, objtype.GetSingleProperty(levels[0]));
                for (int i = 1; i < levels.Length; i++)
                {
                    member = Expression.Property(member, member.Type.GetSingleProperty(levels[i]));
                }
                //构建Cast与Select表达式，将列表中每一项的指定属性转为字符串
                var pe2 = Expression.Parameter(member.Type.GetGenericArguments()[0]);
                var cast = Expression.Call(typeof(Enumerable), "Cast", new Type[] { pe2.Type }, member);

                var name2exp = Expression.Property(pe2, pe2.Type.GetSingleProperty(name2));
                var selectexp = Expression.Call(name2exp, "ToString", Type.EmptyTypes);

                Expression select = Expression.Call(
                               typeof(Enumerable),
                               "Select",
                               new Type[] { pe2.Type, typeof(string) },
                               cast,
                               Expression.Lambda(selectexp, pe2));


                var lambda = Expression.Lambda(select, pe);
                var rv = new List<string>();
                try
                {
                    rv = (lambda.Compile().DynamicInvoke(obj) as IEnumerable<string>)?.ToList();
                }
                catch { }
                return rv;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// 判断属性是否必填
        /// </summary>
        /// <param name="pi">属性信息</param>
        /// <returns>是否必填</returns>
        public static bool IsPropertyRequired(this MemberInfo pi)
        {
            bool isRequired = false;
            if (pi != null)
            {
                //如果需要显示星号，则判断是否是必填项，如果是必填则在内容后面加上星号
                //所有int，float。。。这种Primitive类型的，肯定都是必填
                Type t = pi.GetMemberType();
                if (t != null && (t.IsPrimitive() || t.IsEnum() || t == typeof(decimal) || t == typeof(Guid)))
                {
                    isRequired = true;
                }
                else
                {
                    //对于其他类，检查是否有RequiredAttribute，如果有就是必填
                    if (pi.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault() is RequiredAttribute required && required.AllowEmptyStrings == false)
                    {
                        isRequired = true;
                    }
                    else if (pi.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() != null)
                    {
                        isRequired = true;
                    }
                }
            }
            return isRequired;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="source">属性所在实例</param>
        /// <param name="property">属性名</param>
        /// <param name="value">要赋的值</param>
        /// <param name="prefix">属性前缀</param>
        /// <param name="stringBasedValue">是否为字符串格式的值</param>
        public static void SetPropertyValue(this object source, string property, object value, string prefix = null, bool stringBasedValue = false)
        {
            try
            {
                //去掉属性名中的数组下标（如xxx[0]中的[0]），并按“.”拆分为多级属性
                property = Regex.Replace(property, @"\[[^\]]*\]", string.Empty);
                List<string> level = new List<string>();
                if (property.Contains('.'))
                {
                    level.AddRange(property.Split('.'));
                }
                else
                {
                    level.Add(property);
                }

                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    level.Insert(0, prefix);
                }
                //沿多级属性逐级深入，中间层对象为null时自动创建默认实例
                object temp = source;
                Type tempType = source.GetType();
                for (int i = 0; i < level.Count - 1; i++)
                {
                    var member = tempType.GetMember(level[i])[0];
                    if (member != null)
                    {
                        var va = member.GetMemberValue(temp);
                        if (va != null)
                        {
                            temp = va;
                        }
                        else
                        {
                            var newInstance = member.GetMemberType().GetConstructor(Type.EmptyTypes).Invoke(null);
                            member.SetMemberValue(temp, newInstance, null);
                            temp = newInstance;
                        }
                        tempType = member.GetMemberType();

                    }
                }

                //获取最后一级属性的成员信息，找不到则直接返回
                var memberInfos = tempType.GetMember(level.Last());
                if (!memberInfos.Any())
                {
                    return;
                }
                var fproperty = memberInfos[0];
                if (value == null || ((value is StringValues s) && StringValues.IsNullOrEmpty(s)))
                {
                    fproperty.SetMemberValue(temp, null, null);
                    return;
                }

                bool isArray = false;
                if (value != null && value.GetType().IsArray == true)
                {
                    isArray = true;
                }

                //值为字符串形式（如表单提交的数据）时，按目标类型做特殊转换处理
                if (stringBasedValue == true)
                {
                    Type propertyType = fproperty.GetMemberType();
                    //目标为泛型List类型：将字符串值逐项转换后填入列表
                    if (propertyType.IsGeneric(typeof(List<>)) == true)
                    {
                        var list = propertyType.GetConstructor(Type.EmptyTypes).Invoke(null) as IList;

                        var gs = propertyType.GenericTypeArguments;
                        try
                        {
                            if (value.GetType() == typeof(StringValues))
                            {
                                var strVals = (StringValues)value;
                                var a = strVals.ToArray();
                                for (int i = 0; i < a.Length; i++)
                                {
                                    list.Add(a[i].ConvertValue(gs[0]));
                                }
                            }
                            else if (isArray)
                            {
                                var a = (value as object[]);
                                for (int i = 0; i < a.Length; i++)
                                {
                                    list.Add(a[i].ConvertValue(gs[0]));
                                }
                            }
                            else
                            {
                                list = value.ConvertValue(propertyType) as IList;
                            }
                        }
                        catch { }
                        fproperty.SetMemberValue(temp, list, null);
                    }
                    else if (propertyType.IsArray)
                    {
                        //目标为数组类型：按元素类型逐项转换后创建数组
                        try
                        {
                            var strVals = (StringValues)value;
                            var eletype = propertyType.GetElementType();
                            var arr = Array.CreateInstance(eletype, strVals.Count);
                            for (int i = 0; i < arr.Length; i++)
                            {
                                arr.SetValue(strVals[i].ConvertValue(eletype), i);
                            }
                            fproperty.SetMemberValue(temp, arr, null);
                        }
                        catch { }
                    }
                    else
                    {
                        //其他类型：值为单元素数组时取出第一个元素，否则直接赋值
                        if (isArray)
                        {
                            var a = (value as object[]);
                            if (a.Length == 1)
                            {
                                value = a[0];
                            }
                        }

                        if (value is string)
                        {
                            value = value.ToString().Replace("\\", "/");
                        }
                        fproperty.SetMemberValue(temp, value, null);
                    }
                }
                else
                {
                    if (value is string)
                    {
                        value = value.ToString().Replace("\\", "/");
                    }
                    fproperty.SetMemberValue(temp, value, null);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 根据MemberInfo获取值
        /// </summary>
        /// <param name="mi">MemberInfo</param>
        /// <param name="obj">所在实例</param>
        /// <param name="index">如果是数组，指定数组下标。默认为null</param>
        /// <returns>MemberInfo的值</returns>
        public static object GetMemberValue(this MemberInfo mi, object obj, object[] index = null)
        {
            object rv = null;
            if (mi.MemberType == MemberTypes.Property)
            {
                rv = ((PropertyInfo)mi).GetValue(obj, index);
            }
            else if (mi.MemberType == MemberTypes.Field)
            {
                rv = ((FieldInfo)mi).GetValue(obj);
            }
            return rv;
        }

        /// <summary>
        /// 设定MemberInfo的值
        /// </summary>
        /// <param name="mi">MemberInfo</param>
        /// <param name="obj">所在实例</param>
        /// <param name="val">要赋的值</param>
        /// <param name="index">如果是数组，指定数组下标。默认为null</param>
        public static void SetMemberValue(this MemberInfo mi, object obj, object val, object[] index = null)
        {
            object newval = val;
            if (val is string s)
            {
                if (string.IsNullOrEmpty(s))
                {
                    val = null;
                }
            }
            if (val != null && val.GetType() != mi.GetMemberType())
            {
                newval = val.ConvertValue(mi.GetMemberType());
            }
            if (mi.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo)mi).SetValue(obj, newval, index);
            }
            else if (mi.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)mi).SetValue(obj, newval);
            }
        }

        /// <summary>
        /// 获取某个MemberInfo的类型
        /// </summary>
        /// <param name="mi">MemberInfo</param>
        /// <returns>类型</returns>
        public static Type GetMemberType(this MemberInfo mi)
        {
            Type rv = null;
            if (mi != null)
            {
                if (mi.MemberType == MemberTypes.Property)
                {
                    rv = ((PropertyInfo)mi).PropertyType;
                }
                else if (mi.MemberType == MemberTypes.Field)
                {
                    rv = ((FieldInfo)mi).FieldType;
                }
            }
            return rv;
        }

        /// <summary>
        /// 获取枚举显示名称
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="value">枚举值</param>
        /// <returns>枚举显示名称</returns>
        public static string GetEnumDisplayName(Type enumType, string value)
        {
            string rv = "";
            FieldInfo field = null;

            if (enumType.IsEnum())
            {
                field = enumType.GetField(value);
            }
            //如果是nullable的枚举
            if (enumType.IsGeneric(typeof(Nullable<>)) && enumType.GetGenericArguments()[0].IsEnum())
            {
                field = enumType.GenericTypeArguments[0].GetField(value);
            }

            if (field != null)
            {

                var attribs = field.GetCustomAttributes(typeof(DisplayAttribute), true).ToList();
                if (attribs.Count > 0)
                {
                    rv = ((DisplayAttribute)attribs[0]).GetName();
                    if (CoreProgram._localizer != null)
                    {
                        rv = CoreProgram._localizer[rv];
                    }
                }
                else
                {
                    rv = value;
                }
            }
            return rv;
        }

        /// <summary>
        /// 获取枚举显示名称
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="value">枚举对应的整数值</param>
        /// <returns>枚举显示名称</returns>
        public static string GetEnumDisplayName(Type enumType, int value)
        {
            string rv = "";
            FieldInfo field = null;
            string ename = "";
            if (enumType.IsEnum())
            {
                ename = enumType.GetEnumName(value);
                field = enumType.GetField(ename);
            }
            //如果是nullable的枚举
            if (enumType.IsGeneric(typeof(Nullable<>)) && enumType.GetGenericArguments()[0].IsEnum())
            {
                ename = enumType.GenericTypeArguments[0].GetEnumName(value);
                field = enumType.GenericTypeArguments[0].GetField(ename);
            }

            if (field != null)
            {

                var attribs = field.GetCustomAttributes(typeof(DisplayAttribute), true).ToList();
                if (attribs.Count > 0)
                {
                    rv = ((DisplayAttribute)attribs[0]).GetName();
                    if (CoreProgram._localizer != null)
                    {
                        rv = CoreProgram._localizer[rv];
                    }
                }
                else
                {
                    rv = ename;
                }
            }
            return rv;
        }

        /// <summary>
        /// 转化值
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="propertyType">转换后的类型</param>
        /// <returns>转换后的值</returns>
        public static object ConvertValue(this object value, Type propertyType)
        {
            object val = null;
            if (propertyType.IsGeneric(typeof(Nullable<>)) == true)
            {
                var gs = propertyType.GenericTypeArguments;
                if(value == null)
                {
                    return null;
                }
                try
                {
                    val = ConvertValue(value, gs[0]);
                }
                catch { }
            }
            else if (propertyType.IsEnum())
            {
                val = Enum.Parse(propertyType, value.ToString());
            }
            else if (propertyType == typeof(string))
            {
                val = value?.ToString().Trim();
            }
            else if (propertyType == typeof(Guid))
            {
                bool suc = Guid.TryParse(value?.ToString(), out Guid g);
                if (suc)
                {
                    val = g;
                }
                else
                {
                    val = Guid.Empty;
                }
            }
            else if (propertyType == typeof(DateRange))
            {
                if (DateRange.TryParse(value.ToString(), out var result))
                {
                    val = result;
                }
                else
                {
                    val = DateRange.Default;
                }
            }
            else
            {
                try
                {
                    //以反引号包裹的值代表集合（如“1,2,3”），按目标集合的元素类型逐项转换后填充
                    if (value.ToString().StartsWith("`") && value.ToString().EndsWith("`"))
                    {
                        string inner = value.ToString().Trim('`').TrimEnd(',');
                        if (!string.IsNullOrWhiteSpace(inner))
                        {
                            val = propertyType.GetConstructor(Type.EmptyTypes).Invoke(null);
                            string[] pair = inner.Split(',');
                            var gs = propertyType.GetGenericArguments();
                            foreach (var p in pair)
                            {
                                (val as IList).Add(Convert.ChangeType(p, gs[0]));
                            }
                        }
                    }
                    else
                    {
                        val = Convert.ChangeType(value.ToString(), propertyType);
                    }
                }
                catch
                {
                }
            }
            return val;
        }

        /// <summary>
        /// 通过反射创建指定元素类型的泛型列表，并将值数组逐个包装成实体后加入列表
        /// </summary>
        /// <param name="innerType">列表的元素类型</param>
        /// <param name="propertyName">为元素赋值的属性名</param>
        /// <param name="values">要赋给各元素属性的值数组</param>
        /// <returns>生成的泛型列表对象</returns>
        public static object MakeList(Type innerType, string propertyName, object[] values)
        {
            //通过反射创建泛型列表实例并获取其Add方法
            object rv = typeof(List<>).MakeGenericType(innerType).GetConstructor(Type.EmptyTypes).Invoke(null);
            var mi = rv.GetType().GetMethod("Add");
            var con = innerType.GetConstructor(Type.EmptyTypes);
            foreach (var item in values)
            {
                var newobj = con.Invoke(null);
                newobj.SetPropertyValue(propertyName, item);
                mi.Invoke(rv, new object[] { newobj });
            }
            return rv;
        }
    }
}
