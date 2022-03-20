using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// DC相关扩展函数
    /// </summary>
    public static class DCExtension
    {
        #region 树形下拉
        /// <summary>
        /// 查询数据源，并转化成TreeSelectListItem列表
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="baseQuery">基础查询</param>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="textField">表达式用来获取Text字段对应的值</param>
        /// <param name="valueField">表达式用来获取Value字段对应的值，不指定则默认使用Id字段</param>
        /// <param name="iconField">表达式用来获取icon字段对应的值</param>
        /// <param name="urlField">表达式用来获取Url字段对应的值</param>
        /// <param name="tagField">表达式用来获取Tag字段对应的值</param>
        /// <param name="expandField">表达式用来获取Expanded字段对应的值，指示节点是否展开</param>
        /// <param name="ignorDataPrivilege">忽略数据权限判断</param>
        /// <param name="SortByName">是否根据Text字段排序，默认为是</param>
        /// <returns>SelectListItem列表</returns>
        public static List<TreeSelectListItem> GetTreeSelectListItems<T>(this IQueryable<T> baseQuery
            , WTMContext wtmcontext
            , Expression<Func<T, string>> textField
            , Expression<Func<T, string>> valueField = null
            , Expression<Func<T, string>> iconField = null
            , Expression<Func<T, string>> urlField = null
            , Expression<Func<T, string>> tagField = null
            , Expression<Func<T, bool>> expandField = null
            , bool ignorDataPrivilege = false
            , bool SortByName = true)
            where T : TreePoco
        {
            var dps = wtmcontext?.LoginUserInfo?.DataPrivileges;
            var query = baseQuery.AsNoTracking();

            //如果没有指定忽略权限，则拼接权限过滤的where条件
            if (ignorDataPrivilege == false)
            {
                query = AppendSelfDPWhere(query, wtmcontext, dps);
            }

            if (typeof(IPersistPoco).IsAssignableFrom(typeof(T)))
            {
                var mod = new IsValidModifier();
                var newExp = mod.Modify(query.Expression);
                query = query.Provider.CreateQuery<T>(newExp) as IOrderedQueryable<T>;
            }

            //处理后面要使用的expression
            //if (valueField == null)
            //{
            valueField = x => x.GetID().ToString();
            //}
            Expression<Func<T, string>> parentField = x => x.GetParentID().ToString();

            //定义PE
            ParameterExpression pe = Expression.Parameter(typeof(T));
            ChangePara cp = new ChangePara();

            //创建新类，形成类似 new SimpleTreeTextAndValue() 的表达式
            NewExpression newItem = Expression.New(typeof(TreeSelectListItem));

            //绑定Text字段，形成类似 Text = textField 的表达式
            var textMI = typeof(TreeSelectListItem).GetMember("Text")[0];
            MemberBinding textBind = Expression.Bind(textMI, cp.Change(textField.Body, pe));

            //绑定Value字段，形成类似 Value = valueField 的表达式
            var valueMI = typeof(TreeSelectListItem).GetMember("Value")[0];
            MemberBinding valueBind = Expression.Bind(valueMI, cp.Change(valueField.Body, pe));

            //绑定ParentId字段，形成类似 Value = valueField 的表达式
            var parentMI = typeof(TreeSelectListItem).GetMember("ParentId")[0];
            MemberBinding parentBind = Expression.Bind(parentMI, cp.Change(parentField.Body, pe));

            //绑定Url字段，形成类似 Value = valueField 的表达式
            MemberBinding urlBind = null;
            var urlMI = typeof(TreeSelectListItem).GetMember("Url")[0];
            if (urlField != null)
            {
                urlBind = Expression.Bind(urlMI, cp.Change(urlField.Body, pe));
            }
            else
            {
                urlBind = Expression.Bind(urlMI, Expression.Constant(string.Empty));
            }

            //绑定icon字段，形成类似 Icon = iconField 的表达式
            MemberBinding iconBind = null;
            var iconMI = typeof(TreeSelectListItem).GetMember("Icon")[0];
            if (iconField != null)
            {
                iconBind = Expression.Bind(iconMI, cp.Change(iconField.Body, pe));
            }
            else
            {
                iconBind = Expression.Bind(iconMI, Expression.Constant(string.Empty));
            }

            //绑定Tag字段，形成类似 Value = valueField 的表达式
            MemberBinding tagBind = null;
            var tagMI = typeof(TreeSelectListItem).GetMember("Tag")[0];
            if (tagField != null)
            {
                tagBind = Expression.Bind(tagMI, cp.Change(tagField.Body, pe));
            }
            else
            {
                tagBind = Expression.Bind(tagMI, Expression.Constant(""));
            }

            //绑定Tag字段，形成类似 Value = valueField 的表达式
            MemberBinding expandBind = null;
            var expandMI = typeof(TreeSelectListItem).GetMember("Expended")[0];
            if (expandField != null)
            {
                expandBind = Expression.Bind(expandMI, cp.Change(expandField.Body, pe));
            }
            else
            {
                expandBind = Expression.Bind(expandMI, Expression.Constant(false));
            }

            //合并创建新类和绑定字段的表达式，形成类似 new SimpleTextAndValue{ Text = textField, Value = valueField} 的表达式
            MemberInitExpression init = Expression.MemberInit(newItem, textBind, valueBind, iconBind, parentBind, urlBind, tagBind, expandBind);

            //将最终形成的表达式转化为Lambda，形成类似 x=> new SimpleTextAndValue { Text = x.textField, Value = x.valueField} 的表达式
            var lambda = Expression.Lambda<Func<T, TreeSelectListItem>>(init, pe);

            List<TreeSelectListItem> rv = null;

            //根据Text对下拉菜单数据排序
            if (SortByName == true)
            {
                rv = query.Select(lambda).ToList().OrderBy(x => x.Text).ToList();
            }
            else
            {
                rv = query.Select(lambda).ToList();
            }

            List<TreeSelectListItem> toDel = new List<TreeSelectListItem>();

            rv.ForEach(x =>
            {
                var c = rv.Where(y => y.ParentId == x.Value.ToString()).ToList();
                x.Children = c;
                toDel.AddRange(c);
            });
            toDel.ForEach(x => rv.Remove(x));
            return rv.ToList();
        }

        #endregion

        #region 下拉
        /// <summary>
        /// 查询数据源，并转化成SelectListItem列表
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="baseQuery">基础查询</param>
        /// <param name="wtmcontext">Wtm Context</param>
        /// <param name="textField">SelectListItem中Text字段对应的值</param>
        /// <param name="valueField">SelectListItem中Value字段对应的值，默认为Id列</param>
        /// <param name="ignorDataPrivilege">忽略数据权限判断</param>
        /// <param name="SortByName">是否根据Text字段排序，默认为是</param>
        /// <returns>SelectListItem列表</returns>
        public static List<ComboSelectListItem> GetSelectListItems<T>(this IQueryable<T> baseQuery
            , WTMContext wtmcontext
            , Expression<Func<T, string>> textField
            , Expression<Func<T, object>> valueField = null
            , bool ignorDataPrivilege = false
            , bool SortByName = true)
            where T : TopBasePoco
        {
            var dps = wtmcontext?.LoginUserInfo?.DataPrivileges;
            var query = baseQuery.AsNoTracking();

            //如果value字段为空，则默认使用Id字段作为value值
            if (valueField == null)
            {
                valueField = x => x.GetID().ToString();
            }

            //如果没有指定忽略权限，则拼接权限过滤的where条件
            if (ignorDataPrivilege == false)
            {
                query = AppendSelfDPWhere(query, wtmcontext, dps);
            }

            if (typeof(IPersistPoco).IsAssignableFrom(typeof(T)))
            {
                var mod = new IsValidModifier();
                var newExp = mod.Modify(query.Expression);
                query = query.Provider.CreateQuery<T>(newExp) as IOrderedQueryable<T>;
            }


            //定义PE
            ParameterExpression pe = Expression.Parameter(typeof(T));
            ChangePara cp = new ChangePara();
            //创建新类，形成类似 new SimpleTextAndValue() 的表达式
            NewExpression newItem = Expression.New(typeof(ComboSelectListItem));

            //绑定Text字段，形成类似 Text = textField 的表达式
            var textMI = typeof(ComboSelectListItem).GetMember("Text")[0];
            MemberBinding textBind = Expression.Bind(textMI, cp.Change(textField.Body, pe));


            //绑定Value字段，形成类似 Value = valueField 的表达式
            var valueMI = typeof(ComboSelectListItem).GetMember("Value")[0];
            MemberBinding valueBind = Expression.Bind(valueMI, cp.Change(valueField.Body, pe));

            //如果是树形结构，给ParentId赋值
            MemberBinding parentBind = null;
            var parentMI = typeof(ComboSelectListItem).GetMember("ParentId")[0];
            if (typeof(TreePoco<>).IsAssignableFrom(typeof(T)))
            {
                var parentMember = Expression.MakeMemberAccess(pe, typeof(TreePoco<>).GetSingleProperty("ParentId"));
                var p = Expression.Call(parentMember, "ToString", new Type[] { });
                //var p1 = Expression.Call(p, "ToLower", new Type[] { });
                parentBind = Expression.Bind(parentMI, p);
            }
            else
            {
                parentBind = Expression.Bind(parentMI, Expression.Constant(string.Empty));
            }

            //合并创建新类和绑定字段的表达式，形成类似 new SimpleTextAndValue{ Text = textField, Value = valueField} 的表达式
            MemberInitExpression init = Expression.MemberInit(newItem, textBind, valueBind, parentBind);

            //将最终形成的表达式转化为Lambda，形成类似 x=> new SimpleTextAndValue { Text = x.textField, Value = x.valueField} 的表达式
            var lambda = Expression.Lambda<Func<T, ComboSelectListItem>>(init, pe);


            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            //根据Text对下拉菜单数据排序
            if (SortByName == true)
            {
                rv = query.Select(lambda).ToList().OrderBy(x => x.Text).ToList();
            }
            else
            {
                rv = query.Select(lambda).ToList();
            }

            return rv;
        }

        #endregion

        /// <summary>
        /// 拼接本表的数据权限过滤
        /// </summary>
        /// <typeparam name="T">数据类</typeparam>
        /// <param name="query">源query</param>
        /// <param name="wtmcontext">Wtm context</param>
        /// <param name="dps">数据权限列表</param>
        /// <returns>拼接好where条件的query</returns>
        private static IQueryable<T> AppendSelfDPWhere<T>(IQueryable<T> query, WTMContext wtmcontext, List<SimpleDataPri> dps) where T : TopBasePoco
        {
            var dpsSetting = wtmcontext?.DataPrivilegeSettings;
            Type modelTye = typeof(T);
            bool isBasePoco = typeof(IBasePoco).IsAssignableFrom(modelTye);
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression peid = Expression.Property(pe, typeof(T).GetSingleProperty("ID"));
            //循环数据权限，加入到where条件中，达到自动过滤的效果
            if (dpsSetting?.Where(x => x.ModelName == query.ElementType.Name).SingleOrDefault() != null)
            {
                //如果dps参数是空，则生成 1!=1 这种错误的表达式，这样就查不到任何数据了
                if (dps == null)
                {
                    query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Constant(1), Expression.Constant(1)), pe));
                }
                else
                {
                    //在dps中找到和baseQuery源数据表名一样的关联id
                    var ids = dps.Where(x => x.TableName == query.ElementType.Name).Select(x => x.RelateId).ToList();
                    if (ids == null || ids.Count() == 0)
                    {
                        //if (isBasePoco == true)
                        //{
                        //    var selfexp = Expression.Equal(Expression.Property(pe, "CreateBy"), Expression.Constant(wtmcontext.LoginUserInfo?.ITCode));
                        //    query = query.Where(Expression.Lambda<Func<T, bool>>(selfexp, pe));
                        //}
                        //else
                        //{
                            query = query.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(Expression.Constant(1), Expression.Constant(1)), pe));
                        //}
                    }
                    else
                    {
                        if (!ids.Contains(null))
                        {
                            query = query.Where(ids.GetContainIdExpression<T>());
                        }
                    }
                }
            }
            return query;
        }

        /// <summary>
        /// 为查询语句添加关联表的权限过滤
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="baseQuery">源Query</param>
        /// <param name="wtmcontext"></param>
        /// <param name="IdFields">关联表外键</param>
        /// <returns>修改后的查询语句</returns>
        //public static IQueryable<T> DPWhere<T>(this IQueryable<T> baseQuery, WTMContext wtmcontext, params Expression<Func<T, object>>[] IdFields) where T : TopBasePoco
        //{
        //    var dps = wtmcontext?.LoginUserInfo?.DataPrivileges;
        //    //循环所有关联外键
        //    List<string> tableNameList = new List<string>();
        //    foreach (var IdField in IdFields)
        //    {

        //        //将外键 Id 用.分割，循环生成指向最终id的表达式，比如x=> x.a.b.Id
        //        var fieldName = IdField.GetPropertyName(false);
        //        //获取关联的类
        //        string typename = "";
        //        //如果外键名称不是‘id’，则根据model层的命名规则，它应该是xxxId，所以抹掉最后的 Id 应该是关联的类名
        //        if (fieldName.ToLower() != "id")
        //        {
        //            fieldName = fieldName.Remove(fieldName.Length - 2);
        //            var dtype = IdField.GetPropertyInfo().DeclaringType;
        //            if (dtype == typeof(TreePoco) && fieldName == "Parent")
        //            {
        //                typename = typeof(T).Name;
        //            }
        //            else
        //            {
        //                typename = dtype.GetSingleProperty(fieldName).PropertyType.Name;
        //            }
        //        }
        //        //如果是 Id，则本身就是关联的类
        //        else
        //        {
        //            typename = typeof(T).Name;
        //        }
        //        tableNameList.Add(typename);

        //    }
        //    //var test = DPWhere(baseQuery, dps, tableNameList, IdFields);
        //    return DPWhere(baseQuery, wtmcontext, tableNameList, IdFields);
        //}

        #region AddBy YOUKAI 20160310
        /// <summary>
        /// 为查询语句添加关联表的权限过滤
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="baseQuery">源Query</param>
        /// <param name="wtmcontext">wtm context</param>
        /// <param name="IdFields">关联表外键</param>
        /// <returns>修改后的查询语句</returns>
        public static IQueryable<T> DPWhere<T>(this IQueryable<T> baseQuery, WTMContext wtmcontext, params Expression<Func<T, object>>[] IdFields) where T : TopBasePoco
        {
            var dps = wtmcontext?.LoginUserInfo?.DataPrivileges;

            // var dpsSetting = BaseVM.AllDPS;
            ParameterExpression pe = Expression.Parameter(typeof(T));
            Expression left1 = Expression.Constant(1);
            Expression right1 = Expression.Constant(1);
            Expression trueExp = Expression.Equal(left1, right1);
            Expression falseExp = Expression.NotEqual(left1, right1);
            Expression finalExp = null;
            int tindex = 0;
            //循环所有关联外键
            foreach (var IdField in IdFields)
            {
                Expression exp = trueExp;
                //将外键Id用.分割，循环生成指向最终id的表达式，比如x=> x.a.b.Id
                var fullname = IdField.GetPropertyName();
                string[] splits = fullname.Split('.');

                List<(Expression exp, ParameterExpression pe, bool islist)> data = new System.Collections.Generic.List<(Expression, ParameterExpression, bool)>();
                Expression iexp = pe;
                ParameterExpression ipe = pe;

                //格式化idfeild，保存在data中
                for (int i=0;i<splits.Length;i++)
                {
                    var item = splits[i];
                    var proname = item;
                    int lindex = proname.IndexOf('[');
                    bool islist = false;
                    if (lindex > 0)
                    {
                        islist = true;
                        proname = proname.Substring(0, lindex);
                    }
                    iexp = Expression.MakeMemberAccess(iexp, iexp.Type.GetSingleProperty(proname));

                    Type petype = null;
                    if (islist == true)
                    {
                        petype = iexp.Type.GetGenericArguments()[0];
                    }
                    else
                    {
                        petype = iexp.Type;
                    }
                    if (petype == typeof(TreePoco))
                    {
                        petype = typeof(T);
                    }
                    if(islist == true || i == splits.Length-1)
                    {
                        data.Add((iexp, ipe, islist));
                        ipe = Expression.Parameter(petype);
                        iexp = ipe;
                    }
                }

                //确定最终关联的表名
                string tableName = "";
                if(data.Count > 0)
                {
                    var last = data.Last().exp as MemberExpression;
                    string fieldname = last?.Member?.Name;
                    if (string.IsNullOrEmpty(fieldname) == false)
                    {
                        if (fieldname.ToLower() == "id")
                        {
                            tableName = last.Member.ReflectedType.Name;
                        }
                        else
                        {
                            var pro2 = wtmcontext.DC.GetPropertyNameByFk(last.Member.ReflectedType, fieldname);
                            if (string.IsNullOrEmpty(pro2) == false)
                            {
                                tableName = last.Member.ReflectedType.GetSingleProperty(pro2).PropertyType.Name;
                            }
                        }
                    }
                }
                
                //如果dps为空，则拼接一个返回假的表达式，这样就查询不出任何数据
                if (dps == null || tableName == "")
                {
                    exp = falseExp;
                }
                else
                {
                    var dpsSetting = wtmcontext.DataPrivilegeSettings;
                    //循环系统设定的数据权限，如果没有和关联类一样的表，则跳过
                    if (dpsSetting.Where(x => x.ModelName == tableName).FirstOrDefault() == null)
                    {
                        continue;
                    }
                    //获取dps中关联到关联类的id列表
                    var ids = dps.Where(x => x.TableName == tableName).Select(x => x.RelateId).ToList();
                    //如果没有关联的id，则拼接一个返回假的where，是语句查询不到任何数据
                    if (ids == null || ids.Count == 0)
                    {
                        //bool isBasePoco = typeof(IBasePoco).IsAssignableFrom(data.Last().pe.Type);
                        //if (isBasePoco)
                        //{
                        //    exp = Expression.Equal(Expression.Property(data.Last().pe, "CreateBy"), Expression.Constant(wtmcontext.LoginUserInfo?.ITCode));
                        //}
                        //else
                        //{
                            exp = falseExp;
                        //}
                    }
                    //如果有关联 Id
                    else
                    {
                        //如果关联 Id 不包含null，则生成类似 x=> ids.Contains(x.a.b.Id) 这种条件
                        //如果关联 Id 包括null，则代表可以访问所有数据，就不需要再拼接where条件了
                        if (!ids.Contains(null))
                        {
                            for(int i=data.Count-1; i>=0; i--)
                            {
                                var d = data[i];
                                if(d.islist == true)
                                {
                                    var lastd = data[i + 1];
                                    var queryable = Expression.Call(
                                         typeof(Queryable),
                                         "AsQueryable",
                                         new Type[] { lastd.pe.Type },
                                         d.exp);

                                    exp = Expression.Call(
                                         typeof(Queryable),
                                         "Any",
                                         new Type[] { lastd.pe.Type },
                                         queryable,
                                         Expression.Lambda(typeof(Func<,>).MakeGenericType(lastd.pe.Type, typeof(bool)), exp, new ParameterExpression[] { lastd.pe }));

                                }
                                else
                                {
                                    exp = ids.GetContainIdExpression(d.pe.Type, d.pe, d.exp).Body;
                                }
                            }
                        }
                    }
                }
                //把所有where里的条件用And拼接在一起
                if (finalExp == null)
                {
                    finalExp = exp;
                }
                else
                {
                    finalExp = Expression.OrElse(finalExp, exp);
                }
                tindex++;
            }
            //如果没有进行任何修改，则还返回baseQuery
            if (finalExp == null)
            {
                return baseQuery;
            }
            else
            {
                //返回加入了where条件之后的baseQuery
                var query = baseQuery.Where(Expression.Lambda<Func<T, bool>>(finalExp, pe));
                return query;
            }
        }
        #endregion

        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> baseQuery, string sortInfo, params SortInfo[] defaultSorts) where T : TopBasePoco
        {
            List<SortInfo> info = new List<SortInfo>();
            IOrderedQueryable<T> rv = null;
            if (string.IsNullOrEmpty(sortInfo))
            {
                if (defaultSorts == null || defaultSorts.Length == 0)
                {
                    ParameterExpression pe = Expression.Parameter(typeof(T));
                    var idproperty = typeof(T).GetSingleProperty("ID");
                    Expression pro = Expression.Property(pe, idproperty);
                    Type proType = typeof(Guid);
                    Expression final = Expression.Call(
                                                   typeof(Queryable),
                                                   "OrderBy",
                                                   new Type[] { typeof(T), proType },
                                                   baseQuery.Expression,
                                                   Expression.Lambda(pro, new ParameterExpression[] { pe }));
                    rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    return rv;
                }
                else
                {
                    info.AddRange(defaultSorts);
                }
            }
            else
            {
                var temp = JsonSerializer.Deserialize<List<SortInfo>>(sortInfo);
                info.AddRange(temp);
            }
            foreach (var item in info)
            {
                ParameterExpression pe = Expression.Parameter(typeof(T));
                var idproperty = typeof(T).GetSingleProperty(item.Property);
                Expression pro = Expression.Property(pe, idproperty);
                Type proType = typeof(T).GetSingleProperty(item.Property).PropertyType;
                if (item.Direction == SortDir.Asc)
                {
                    if (rv == null)
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "OrderBy",
                               new Type[] { typeof(T), proType },
                               baseQuery.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                    else
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "ThenBy",
                               new Type[] { typeof(T), proType },
                               rv.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = rv.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                }
                if (item.Direction == SortDir.Desc)
                {
                    if (rv == null)
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "OrderByDescending",
                               new Type[] { typeof(T), proType },
                               baseQuery.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = baseQuery.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                    else
                    {
                        Expression final = Expression.Call(
                               typeof(Queryable),
                               "ThenByDescending",
                               new Type[] { typeof(T), proType },
                               rv.Expression,
                               Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        rv = rv.Provider.CreateQuery<T>(final) as IOrderedQueryable<T>;
                    }
                }
            }
            return rv;
        }

        public static IQueryable<T> CheckID<T>(this IQueryable<T> baseQuery, object val, Expression<Func<T, object>> member = null)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = null;
            if (member == null)
            {
                idproperty = typeof(T).GetSingleProperty("ID");
            }
            else
            {
                idproperty = member.GetPropertyInfo();
            }
            Expression peid = Expression.Property(pe, idproperty);
            var convertid = PropertyHelper.ConvertValue(val, idproperty.PropertyType);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(peid, Expression.Constant(convertid)), pe));
        }

        public static IQueryable<T> CheckParentID<T>(this IQueryable<T> baseQuery, string val)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = null;
            idproperty = typeof(T).GetSingleProperty("ParentId");
            Expression peid = Expression.Property(pe, idproperty);
            var p = Expression.Call(peid, "ToString", new Type[] { });
            if (val == null)
            {
                return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(peid, Expression.Constant(null)), pe));
            }
            else
            {
                return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(p, Expression.Constant(val)), pe));
            }
        }


        public static IQueryable<T> CheckIDs<T>(this IQueryable<T> baseQuery, List<string> val, Expression<Func<T, object>> member = null)
        {
            if (val == null)
            {
                return baseQuery;
            }
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = null;
            if (member == null)
            {
                idproperty = typeof(T).GetSingleProperty("ID");
            }
            else
            {
                idproperty = member.GetPropertyInfo();
            }
            Expression peid = Expression.Property(pe, idproperty);
            var exp = val.GetContainIdExpression(typeof(T), pe, peid).Body;
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(exp, pe));
        }


        public static IQueryable<T> CheckNotNull<T>(this IQueryable<T> baseQuery, Expression<Func<T, object>> member)
        {
            return baseQuery.CheckNotNull<T>(member.GetPropertyName());
        }

        public static IQueryable<T> CheckNotNull<T>(this IQueryable<T> baseQuery, string member)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = typeof(T).GetSingleProperty(member);
            Expression peid = Expression.Property(pe, idproperty);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.NotEqual(peid, Expression.Constant(null)), pe));
        }


        public static IQueryable<T> CheckNull<T>(this IQueryable<T> baseQuery, Expression<Func<T, object>> member)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            PropertyInfo idproperty = typeof(T).GetSingleProperty(member.GetPropertyName());
            Expression peid = Expression.Property(pe, idproperty);
            return baseQuery.Where(Expression.Lambda<Func<T, bool>>(Expression.Equal(peid, Expression.Constant(null)), pe));
        }

        /// <summary>
        /// val不为空时，附加查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="baseQuery"></param>
        /// <param name="val"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IQueryable<T> CheckWhere<T, S>(this IQueryable<T> baseQuery, S val, Expression<Func<T, bool>> where)
        {
            if (val == null)
            {
                return baseQuery;
            }
            else if (val is string s && string.IsNullOrEmpty(s))
            {
                return baseQuery;
            }
            else
            {
                if (typeof(IList).IsAssignableFrom(val.GetType()))
                {
                    if (((IList)val).Count == 0)
                    {
                        return baseQuery;
                    }
                }
                return baseQuery.Where(where);
            }
        }

        /// <summary>
        /// 条件为true时，附加查询条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQuery"></param>
        /// <param name="val">bool?</param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> baseQuery, bool? val, Expression<Func<T, bool>> where)
        {
            if (val == null || val == false)
            {
                return baseQuery;
            }
            return baseQuery.Where(where);
        }

        public static IQueryable<T> CheckEqual<T>(this IQueryable<T> baseQuery, string val, Expression<Func<T, string>> field)
        {
            if (val == null || val == "")
            {
                return baseQuery;
            }
            else
            {
                val = val.Trim();
                var equal = Expression.Equal(field.Body, Expression.Constant(val));
                var where = Expression.Lambda<Func<T, bool>>(equal, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckEqual<T, S>(this IQueryable<T> baseQuery, S? val, Expression<Func<T, S?>> field)
            where S : struct
        {
            if (val == null)
            {
                return baseQuery;
            }
            else
            {
                var equal = Expression.Equal(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(val));
                var where = Expression.Lambda<Func<T, bool>>(equal, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckEqual<T, S>(this IQueryable<T> baseQuery, S val, Expression<Func<T, S?>> field)
    where S : struct
        {
            S? a = val;
            return baseQuery.CheckEqual(a, field);
        }


        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S? valMin, S? valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
    where S : struct
        {
            if (valMin == null && valMax == null)
            {
                return baseQuery;
            }
            else
            {
                IQueryable<T> rv = baseQuery;
                if (valMin != null)
                {
                    BinaryExpression exp1 = !includeMin ? Expression.GreaterThan(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMin)) : Expression.GreaterThanOrEqual(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMin));
                    rv = rv.Where(Expression.Lambda<Func<T, bool>>(exp1, field.Parameters[0]));
                }
                if (valMax != null)
                {
                    BinaryExpression exp2 = !includeMax ? Expression.LessThan(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMax)) : Expression.LessThanOrEqual(Expression.PropertyOrField(field.Body, "Value"), Expression.Constant(valMax));
                    rv = rv.Where(Expression.Lambda<Func<T, bool>>(exp2, field.Parameters[0]));
                }
                return rv;
            }
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S valMin, S valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S? valMin, S valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckBetween<T, S>(this IQueryable<T> baseQuery, S valMin, S? valMax, Expression<Func<T, S?>> field, bool includeMin = true, bool includeMax = true)
where S : struct
        {
            S? a = valMin;
            S? b = valMax;
            return CheckBetween(baseQuery, a, b, field, includeMin, includeMax);
        }

        public static IQueryable<T> CheckContain<T>(this IQueryable<T> baseQuery, string val, Expression<Func<T, string>> field, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val))
            {
                return baseQuery;
            }
            else
            {
                val = val.Trim();
                Expression exp = null;
                if (ignoreCase == true)
                {
                    var tolower = Expression.Call(field.Body, "ToLower", null);
                    exp = Expression.Call(tolower, "Contains", null, Expression.Constant(val.ToLower()));
                }
                else
                {
                    exp = Expression.Call(field.Body, "Contains", null, Expression.Constant(val));

                }
                var where = Expression.Lambda<Func<T, bool>>(exp, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<T> CheckContain<T, S>(this IQueryable<T> baseQuery, List<S> val, Expression<Func<T, S>> field)
        {
            if (val == null || val.Count == 0 || (val.Count == 1 && val[0] == null))
            {
                return baseQuery;
            }
            else
            {
                Expression exp = null;
                exp = Expression.Call(Expression.Constant(val), "Contains", null, field.Body);

                var where = Expression.Lambda<Func<T, bool>>(exp, field.Parameters[0]);
                return baseQuery.Where(where);
            }
        }

        public static IQueryable<string> DynamicSelect<T>(this IQueryable<T> baseQuery, string fieldName)
        {
            ParameterExpression pe = Expression.Parameter(typeof(T));
            var idproperty = typeof(T).GetSingleProperty(fieldName);
            Expression pro = Expression.Property(pe, idproperty);
            Expression tostring = Expression.Call(pro, "ToString", new Type[] { });
            Type proType = typeof(string);
            Expression final = Expression.Call(
                                           typeof(Queryable),
                                           "Select",
                                           new Type[] { typeof(T), proType },
                                           baseQuery.Expression,
                                           Expression.Lambda(tostring, new ParameterExpression[] { pe }));
            var rv = baseQuery.Provider.CreateQuery<string>(final) as IOrderedQueryable<string>;
            return rv;
        }


        public static string GetTableName<T>(this IDataContext self)
        {
            return self.Model.FindEntityType(typeof(T)).GetTableName();
        }

        /// <summary>
        /// 通过模型和模型的某个List属性的名称来判断List的字表中关联到主表的主键名称
        /// </summary>
        /// <typeparam name="T">主表Model</typeparam>
        /// <param name="self">DataContext</param>
        /// <param name="listFieldName">主表中的子表List属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName<T>(this IDataContext self, string listFieldName) where T : class
        {
            return GetFKName(self, typeof(T), listFieldName);
        }

        /// <summary>
        /// 通过模型和模型的某个List属性的名称来判断List的字表中关联到主表的主键名称
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="sourceType">主表model类型</param>
        /// <param name="listFieldName">主表中的子表List属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName(this IDataContext self, Type sourceType, string listFieldName)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetReferencingForeignKeys().Where(x => x.PrincipalToDependent?.Name == listFieldName).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.Properties[0].Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }


        /// <summary>
        /// 通过子表模型和模型关联到主表的属性名称来判断该属性对应的主键名称
        /// </summary>
        /// <typeparam name="T">子表Model</typeparam>
        /// <param name="self">DataContext</param>
        /// <param name="FieldName">关联主表的属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName2<T>(this IDataContext self, string FieldName) where T : class
        {
            return GetFKName2(self, typeof(T), FieldName);
        }

        /// <summary>
        /// 通过模型和模型关联到主表的属性名称来判断该属性对应的主键名称
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="sourceType">子表model类型</param>
        /// <param name="FieldName">关联主表的属性名称</param>
        /// <returns>主键名称</returns>
        public static string GetFKName2(this IDataContext self, Type sourceType, string FieldName)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetForeignKeys().Where(x => x.DependentToPrincipal?.Name == FieldName).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.Properties[0].Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static string GetFieldName<T>(this IDataContext self, Expression<Func<T, object>> field)
        {
            string pname = field.GetPropertyName();
            return self.GetFieldName<T>(pname);
        }


        public static string GetFieldName<T>(this IDataContext self, string fieldname)
        {
            var rv = self.Model.FindEntityType(typeof(T)).FindProperty(fieldname);
            return rv?.GetColumnName(new Microsoft.EntityFrameworkCore.Metadata.StoreObjectIdentifier());
        }

        public static string GetPropertyNameByFk(this IDataContext self, Type sourceType, string fkname)
        {
            try
            {
                var test = self.Model.FindEntityType(sourceType).GetForeignKeys().Where(x => x.DependentToPrincipal?.ForeignKey?.Properties[0]?.Name == fkname).FirstOrDefault();
                if (test != null && test.Properties.Count > 0)
                {
                    return test.DependentToPrincipal.Name;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }


        public static Expression<Func<TModel, bool>> GetContainIdExpression<TModel>(this List<string> Ids, Expression peid = null)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TModel));
            var rv = Ids.GetContainIdExpression(typeof(TModel), pe, peid) as Expression<Func<TModel, bool>>;
            return rv;
        }

        public static LambdaExpression GetContainIdExpression(this List<string> Ids, Type modeltype, ParameterExpression pe, Expression peid = null)
        {
            if (Ids == null)
            {
                Ids = new List<string>();
            }
            if (peid == null)
            {
                peid = Expression.Property(pe, modeltype.GetSingleProperty("ID"));
            }
            else
            {
                ChangePara cp = new ChangePara();
                peid = cp.Change(peid, pe);
            }
            List<object> newids = new List<object>();
            foreach (var item in Ids)
            {
                object vv = PropertyHelper.ConvertValue(item, peid.Type);
                if (vv != null)
                {
                    newids.Add(vv);
                }
            }
            Expression dpleft = Expression.Constant(newids, typeof(IEnumerable<object>));
            Expression dpleft2 = Expression.Call(typeof(Enumerable), "Cast", new Type[] { peid.Type }, dpleft);
            Expression dpleft3 = Expression.Call(typeof(Enumerable), "ToList", new Type[] { peid.Type }, dpleft2);
            Expression dpcondition = Expression.Call(typeof(Enumerable), "Contains", new Type[] { peid.Type }, dpleft3, peid);
            var rv = Expression.Lambda(typeof(Func<,>).MakeGenericType(modeltype, typeof(bool)), dpcondition, pe);
            return rv;
        }

        /// <summary>
        /// 开始一个事务，当使用同一IDataContext时，嵌套的两个事务不会引起冲突，当嵌套的事务执行时引起的异常会通过回滚方法向上层抛出异常
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <returns>可用的事务实例</returns>
        public static Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction BeginTransaction(this IDataContext self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (self.Database == null)
                throw new ArgumentNullException(nameof(self.Database));
            if (@"Microsoft.EntityFrameworkCore.InMemory".Equals(self.Database.ProviderName, StringComparison.OrdinalIgnoreCase))
                return FakeNestedTransaction.DefaultTransaction;
            return self.Database.CurrentTransaction == null ? self.Database.BeginTransaction() : FakeNestedTransaction.DefaultTransaction;
        }

        /// <summary>
        /// 开始一个事务，当使用同一IDataContext时，嵌套的两个事务不会引起冲突，当嵌套的事务执行时引起的异常会通过回滚方法向上层抛出异常
        /// </summary>
        /// <param name="self">DataContext</param>
        /// <param name="isolationLevel"></param>
        /// <returns>可用的事务实例</returns>
        public static Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction BeginTransaction(this IDataContext self, System.Data.IsolationLevel isolationLevel)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (self.Database == null)
                throw new ArgumentNullException(nameof(self.Database));
            if (@"Microsoft.EntityFrameworkCore.InMemory".Equals(self.Database.ProviderName, StringComparison.OrdinalIgnoreCase))
                return FakeNestedTransaction.DefaultTransaction;
            return self.Database.CurrentTransaction == null ? self.Database.BeginTransaction(isolationLevel) : FakeNestedTransaction.DefaultTransaction;
        }
    }

    public static class DbCommandExtension
    {
        public static void AddParameter(this DbCommand command)
        {

        }
    }

    internal class FakeNestedTransaction : Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction
    {
        internal static readonly FakeNestedTransaction DefaultTransaction = new FakeNestedTransaction();

        private FakeNestedTransaction() { }

        public void Dispose()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
            throw new TransactionInDoubtException("an exception occurs while executing the nested transaction or processing the results");
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public Guid TransactionId => Guid.Empty;
    }
}
