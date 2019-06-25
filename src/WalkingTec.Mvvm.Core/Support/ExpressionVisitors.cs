using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 分析x==y这种类型的表达式，并储存在Dictionary中
    /// </summary>
    public class SetValuesParser : ExpressionVisitor
    {
        private Dictionary<string, object> _rv = new Dictionary<string, object>();

        /// <summary>
        /// 开始分析表达式
        /// </summary>
        /// <param name="expression">源表达式</param>
        /// <returns>将x==y这种表达式变为Dictionary<x,y>并返回</returns>
        public Dictionary<string, object> Parse(Expression expression)
        {
            Visit(expression);
            return _rv;
        }

        /// <summary>
        /// 处理所有二进制类型的表达式
        /// </summary>
        /// <param name="node">当前表达式节点</param>
        /// <returns>修改后的表达式</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            //如果表达式是x==y的类型，则获取y的值，并使用x最为key，y的值作为value添加到Dictionary中保存
            if (node.NodeType == ExpressionType.Equal)
            {
                var pi = PropertyHelper.GetPropertyName(node.Left.NodeType == ExpressionType.Convert ? ((UnaryExpression)node.Left).Operand : node.Left);
                if (!_rv.ContainsKey(pi))
                {
                    _rv.Add(pi, Expression.Lambda(node.Right).Compile().DynamicInvoke());
                }
            }
            return base.VisitBinary(node);
        }
    }

    /// <summary>
    /// 替换表达式中的OrderBy语句
    /// </summary>
    public class OrderReplaceModifier : ExpressionVisitor
    {
        private bool _addMode = false;
        private SortInfo _sortinfo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sortinfo">sortinfo</param>
        public OrderReplaceModifier(SortInfo sortinfo)
        {
            _sortinfo = sortinfo;
        }

        /// <summary>
        /// 获取表达式的源数据类
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式的源数据类型</returns>
        private Type GetDCModel(Expression expression)
        {
            //如果表达式是方法调用的类型，则一直向上寻找，知道找到参数为ObjectQuery<>的表达式，它的类型就是整个表达式的源数据类型
            if (expression.NodeType == ExpressionType.Call)
            {
                var exp = (expression as MethodCallExpression).Arguments[0];
                if (exp.Type.IsGeneric(typeof(EntityQueryable<>)))
                {
                    return exp.Type.GenericTypeArguments[0];
                }
                else
                {
                    return GetDCModel(exp);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 修改where
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>修改后的表达式</returns>
        public Expression Modify(Expression expression)
        {
            //先调用一次Visit，删除所有的where表达式
            var rv = Visit(expression);
            if(rv.NodeType == ExpressionType.Constant)
            {
                if ((rv.Type.IsGeneric(typeof(EntityQueryable<>)) || rv.Type.IsGeneric(typeof(EnumerableQuery<>))))
                {
                    var modelType = rv.Type.GenericTypeArguments[0];
                    ParameterExpression pe = Expression.Parameter(modelType, "x");
                    Expression left1 = Expression.Constant(1);
                    Expression right1 = Expression.Constant(1);
                    Expression trueExp = Expression.Equal(left1, right1);
                    rv = Expression.Call(
 typeof(Queryable),
 "Where",
 new Type[] { modelType },
 rv,
 Expression.Lambda(trueExp, new ParameterExpression[] { pe }));

                }
            }
            //将模式设为addMode，再调用一次Visit来添加新的表达式
            _addMode = true;
            rv = Visit(rv);
            return rv;
        }

        /// <summary>
        /// 向表达式树上层寻找不是where的节点
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns>返回表达式上层第一个不是where的节点</returns>
        private Expression GetParentExpNotOrder(MethodCallExpression exp)
        {
            var parentNode = exp.Arguments[0] as MethodCallExpression;
            if (parentNode == null || (parentNode.Method.Name.ToLower() != "orderby" && parentNode.Method.Name.ToLower() != "orderbydescending"))
            {
                if(parentNode == null){
                    return exp.Arguments[0];
                }
                return parentNode;
            }
            else
            {
                return GetParentExpNotOrder(parentNode);
            }
        }

        /// <summary>
        /// 检查方法调用类型的表达式
        /// </summary>
        /// <param name="node">表达式节点</param>
        /// <returns>修改后的表达式</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //如果不是添加模式，那么删除所有的order
            if (_addMode == false)
            {
                var aType = node.Arguments[0].Type;
                //如果节点是order
                if (node != null && (node.Method.Name.ToLower() == "orderby" || node.Method.Name.ToLower() == "orderbydescending" || node.Method.Name.ToLower() == "thenby" || node.Method.Name.ToLower() == "thenbydescending") && aType.GetTypeInfo().IsGenericType)
                {
                    //继续往上找到不是where的节点
                    return GetParentExpNotOrder(node);
                }
            }
            //如果是添加模式
            else
            {
                var modelType = node.Type.GenericTypeArguments[0];
                List<SortInfo> info = new List<SortInfo>() { _sortinfo };
                Expression rv = null;
                foreach (var item in info)
                {
                    ParameterExpression pe = Expression.Parameter(modelType,"x");
                    Expression pro = Expression.PropertyOrField(pe, item.Property);
                    Type proType = modelType.GetProperty(item.Property).PropertyType;
                    if (item.Direction == SortDir.Asc)
                    {
                        if (rv == null)
                        {
                            rv = Expression.Call(
                                   typeof(Queryable),
                                   "OrderBy",
                                   new Type[] { modelType, proType },
                                   node,
                                   Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        }
                        else
                        {
                            rv = Expression.Call(
                                    typeof(Queryable),
                                    "ThenBy",
                                    new Type[] { modelType, proType },
                                    rv,
                                    Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        }
                    }
                    if (item.Direction == SortDir.Desc)
                    {
                        if (rv == null)
                        {
                            rv = Expression.Call(
                                   typeof(Queryable),
                                   "OrderByDescending",
                                   new Type[] { modelType, proType },
                                   node,
                                   Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        }
                        else
                        {
                            rv = Expression.Call(
                                   typeof(Queryable),
                                   "ThenByDescending",
                                   new Type[] { modelType, proType },
                                   rv,
                                   Expression.Lambda(pro, new ParameterExpression[] { pe }));
                        }
                    }
                }
                return rv;
            }
            return base.VisitMethodCall(node);
        }
    }


    /// <summary>
    /// 替换表达式中的Where语句
    /// </summary>
    public class WhereReplaceModifier : ExpressionVisitor
    {
        private Type _modelType;
        private bool _addMode = false;
        private Expression<Func<TopBasePoco, bool>> _where;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="where">需要替换的新where语句</param>
        public WhereReplaceModifier(Expression<Func<TopBasePoco, bool>> where)
        {
            _where = where;
        }

        /// <summary>
        /// 获取表达式的源数据类
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式的源数据类型</returns>
        private Type GetDCModel(Expression expression)
        {
            //如果表达式是方法调用的类型，则一直向上寻找，知道找到参数为ObjectQuery<>的表达式，它的类型就是整个表达式的源数据类型
            if (expression.NodeType == ExpressionType.Call)
            {
                var exp = (expression as MethodCallExpression).Arguments[0];
                if (exp.Type.IsGeneric(typeof(EntityQueryable<>)))
                {
                    return exp.Type.GenericTypeArguments[0];
                }
                else
                {
                    return GetDCModel(exp);
                }
            }
            else if(expression.NodeType == ExpressionType.Constant)
            {
                if (expression.Type.IsGeneric(typeof(EnumerableQuery<>)))
                {
                    return expression.Type.GenericTypeArguments[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 修改where
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>修改后的表达式</returns>
        public Expression Modify(Expression expression)
        {
            //获取源类型
            var checkType = GetDCModel(expression);

            if (checkType != null)
            {
                _modelType = checkType;
                //先调用一次Visit，删除所有的where表达式
                var rv = Visit(expression);
                //将模式设为addMode，再调用一次Visit来添加新的表达式
                _addMode = true;
                rv = Visit(rv);
                return rv;
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// 向表达式树上层寻找不是where的节点
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns>返回表达式上层第一个不是where的节点</returns>
        private Expression GetParentExpNotWhere(MethodCallExpression exp)
        {
            if (!(exp.Arguments[0] is MethodCallExpression parentNode))
            {
                return exp.Arguments[0];
            }
            else if (parentNode.Method.Name.ToLower() != "where")
            {
                return parentNode;
            }
            else
            {
                return GetParentExpNotWhere(parentNode);
            }
        }
      
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_addMode == true)
            {
                if ((node.Type.IsGeneric(typeof(EntityQueryable<>)) || node.Type.IsGeneric(typeof(EnumerableQuery<>))) && node.Type.GenericTypeArguments[0] == _modelType)
                {
                    ParameterExpression pe = Expression.Parameter(_modelType);
                    ChangePara cp = new ChangePara();
                    //传递过来的where条件，其源类型不一定和表达式中原有的相符，所以先将新where条件的源类型修改成和query一样的
                    //也就是说新的where条件可能是 x=>x.id==1， 而在表达式中不一定有x，所以要先统一
                    var modifiedWhere = cp.Change(_where.Body, pe);
                    var rv = Expression.Call(
                     typeof(Queryable),
                     "Where",
                     new Type[] { _modelType },
                     node,
                     Expression.Lambda(modifiedWhere, new ParameterExpression[] { pe }));
                    return rv;
                }
            }
            return base.VisitConstant(node);
        }

        /// <summary>
        /// 检查方法调用类型的表达式
        /// </summary>
        /// <param name="node">表达式节点</param>
        /// <returns>修改后的表达式</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //如果不是添加模式，那么删除所有的where条件
            if (_addMode == false)
            {
                if(node.Arguments.Count == 0)
                {
                    return base.VisitMethodCall(node);
                }
                var aType = node.Arguments[0].Type;
                //如果节点的上一个节点是where
                if (node.Arguments[0] is MethodCallExpression parentNode && parentNode.Method.Name.ToLower() == "where" && aType.GetTypeInfo().IsGenericType)
                {
                    //继续往上找到不是where的节点
                    var nowhereNode = GetParentExpNotWhere(parentNode);
                    Type gType = aType.GetGenericTypeDefinition();
                    Type argType = aType.GenericTypeArguments[0];
                    //使用上面不是where的节点直接拼接本节点，从而删除了中间的where
                    if ((gType == typeof(IQueryable<>) || gType == typeof(EntityQueryable<>)) && argType == _modelType)
                    {
                        var paras = new List<Expression>
                        {
                            nowhereNode
                        };
                        paras.AddRange(node.Arguments.Skip(1).ToList());
                        var rv = Expression.Call(
                         node.Method,
                         paras);
                        return rv;
                    }
                }
            }
            return base.VisitMethodCall(node);
        }
    }

    /// <summary>
    /// 用于PersistPoco的搜索，检查查询语句中是否有IsValid的搜索条件，如果没有，默认加上IsValid=true
    /// </summary>
    public class IsValidModifier : ExpressionVisitor
    {
        private Type _modelType;
        private bool _needAdd = true;
        private int _mode = 0; //0是搜素模式，1是添加模式
        private Expression<Func<PersistPoco, bool>> _where;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IsValidModifier()
        {
            _where = x=>x.IsValid==true;
        }

        /// <summary>
        /// 获取表达式的源数据类
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>表达式的源数据类型</returns>
        private Type GetDCModel(Expression expression)
        {
            //如果表达式是方法调用的类型，则一直向上寻找，知道找到参数为ObjectQuery<>的表达式，它的类型就是整个表达式的源数据类型
            if (expression.NodeType == ExpressionType.Call)
            {
                var exp = (expression as MethodCallExpression).Arguments[0];
                if (exp.Type.IsGeneric(typeof(EntityQueryable<>)))
                {
                    return exp.Type.GenericTypeArguments[0];
                }
                else
                {
                    return GetDCModel(exp);
                }
            }
            else if (expression.NodeType == ExpressionType.Constant)
            {
                if (expression.Type.IsGeneric(typeof(EnumerableQuery<>)))
                {
                    return expression.Type.GenericTypeArguments[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 修改where
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>修改后的表达式</returns>
        public Expression Modify(Expression expression)
        {
            //获取源类型
            var checkType = GetDCModel(expression);

            if (checkType != null)
            {
                _modelType = checkType;
                //先调用一次Visit，删除所有的where表达式
                var rv = Visit(expression);
                //将模式设为addMode，再调用一次Visit来添加新的表达式
                if (_needAdd == true)
                {
                    _mode = 1;
                    rv = Visit(rv);
                }
                return rv;
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// 向表达式树上层寻找不是where的节点
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns>返回表达式上层第一个不是where的节点</returns>
        private Expression GetParentExpNotWhere(MethodCallExpression exp)
        {
            if (!(exp.Arguments[0] is MethodCallExpression parentNode))
            {
                return exp.Arguments[0];
            }
            else if (parentNode.Method.Name.ToLower() != "where")
            {
                return parentNode;
            }
            else
            {
                return GetParentExpNotWhere(parentNode);
            }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_mode == 1)
            {
                if ((node.Type.IsGeneric(typeof(EntityQueryable<>)) || node.Type.IsGeneric(typeof(EnumerableQuery<>))) && node.Type.GenericTypeArguments[0] == _modelType)
                {
                    ParameterExpression pe = Expression.Parameter(_modelType);
                    ChangePara cp = new ChangePara();
                    //传递过来的where条件，其源类型不一定和表达式中原有的相符，所以先将新where条件的源类型修改成和query一样的
                    //也就是说新的where条件可能是 x=>x.id==1， 而在表达式中不一定有x，所以要先统一
                    var modifiedWhere = cp.Change(_where.Body, pe);
                    var rv = Expression.Call(
                     typeof(Queryable),
                     "Where",
                     new Type[] { _modelType },
                     node,
                     Expression.Lambda(modifiedWhere, new ParameterExpression[] { pe }));
                    return rv;
                }
            }
            return base.VisitConstant(node);
        }

        /// <summary>
        /// 查找where中是否出现过对于IsValid的判断
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            //如果表达式是x==y的类型，则获取y的值，并使用x最为key，y的值作为value添加到Dictionary中保存
            if (_mode == 0)
            {
                if (node.NodeType == ExpressionType.Equal)
                {
                    var pi = PropertyHelper.GetPropertyName(node.Left);
                    if (pi.ToLower() == "isvalid")
                    {
                        _needAdd = false;
                    }
                }
            }
            return base.VisitBinary(node);
        }

    }


    /// <summary>
    /// 修改表达式参数
    /// </summary>
    public class ChangePara : ExpressionVisitor
    {
        ParameterExpression _pe;

        /// <summary>
        /// 修改参数
        /// </summary>
        /// <param name="expression">要修改的表达式</param>
        /// <param name="pe">新的参数</param>
        /// <returns>修改后的表达式</returns>
        public Expression Change(Expression expression, ParameterExpression pe)
        {
            _pe = pe;
            return Visit(expression);
        }

        /// <summary>
        /// 检查所有参数类型的表达式
        /// </summary>
        /// <param name="node">表达式节点</param>
        /// <returns>新的参数类型</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _pe;
        }

        /// <summary>
        /// 检查所有成员访问类型的表达式
        /// </summary>
        /// <param name="node">表达式节点</param>
        /// <returns>修改后的表达式</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression.NodeType == ExpressionType.Parameter)
            {
                var rv = Expression.MakeMemberAccess(_pe, node.Member);
                return rv;
            }
            else
            {
                return base.VisitMember(node);
            }
        }
    }

    /// <summary>
    /// 获取Select语句中选择的列的信息
    /// </summary>
    public class SelectInfo : ExpressionVisitor
    {
        private List<string> _columns;
        private bool _found;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectInfo()
        {
        }

        /// <summary>
        /// 获取Select语句中选择的列的名字
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>列的名字</returns>
        public List<string> GetColumns(Expression expression)
        {
            Visit(expression);
            return _columns;
        }


        /// <summary>
        /// 检查方法调用类型的表达式
        /// </summary>
        /// <param name="node">表达式节点</param>
        /// <returns>修改后的表达式</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (_found == false)
            {
                if (node.Method.Name.ToLower() == "select")
                {
                    if (node.Arguments[1] is UnaryExpression ue)
                    {
                        var inner = ue.Operand as LambdaExpression;
                        if (inner.Body is MemberInitExpression memberinit)
                        {
                            _columns = new List<string>();
                            foreach (var m in memberinit.Bindings)
                            {
                                _columns.Add(m.Member.Name);
                            }
                            _found = true;
                        }
                    }
                }
            }
            return base.VisitMethodCall(node);
        }
    }

    public class ClearSelectMany : ExpressionVisitor
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ClearSelectMany()
        {
        }

        /// <summary>
        /// 获取Select语句中选择的列的名字
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>列的名字</returns>
        public Expression Clear(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            List<MemberBinding> newbinding = new List<MemberBinding>();
            for (int i = 0; i < node.Bindings.Count; i++)
            {
                bool islist = false;
                if (node.Bindings[i] is MemberAssignment ma && ma.Expression.Type.GetTypeInfo().IsGenericType == true)
                {
                    if (ma.Expression.Type.GetGenericTypeDefinition() != typeof(Nullable<>))
                    {
                        islist = true;
                    }
                }
                if (islist == false)
                {
                    newbinding.Add(node.Bindings[i]);
                }
            }
            if (newbinding.Count < node.Bindings.Count)
            {
                return node.Update(node.NewExpression, newbinding);
            }
            else
            {
                return base.VisitMemberInit(node);
            }
        }
    }
}
