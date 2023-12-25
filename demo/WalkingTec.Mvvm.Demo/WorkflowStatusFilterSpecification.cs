using Elsa.Models;
using Elsa.Persistence.Specifications;
using System;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Demo
{
    public class WorkflowStatusFilterSpecification : Specification<WorkflowInstance>
    {
        public WorkflowStatusFilterSpecification(params string[] names)
        {
            Names = names;
        }

        public string[] Names { get; }

        public override Expression<Func<WorkflowInstance, bool>> ToExpression()
        {

            Expression exp = null;
            var i = 0;
            var param = Expression.Parameter(typeof(WorkflowInstance), "instance");

            foreach (var name in Names)
            {
                var equality = Expression.Equal(Expression.Property(param, "Name"), Expression.Constant(name));
                if (i == 0)
                    exp = equality;
                else
                    exp = Expression.Or(exp, equality);
                i++;
            }

            Expression<Func<WorkflowInstance, bool>> lambda =
                Expression.Lambda<Func<WorkflowInstance, bool>>(
                    exp,
                    new ParameterExpression[] { param });

            return lambda;
        }


    }
}
