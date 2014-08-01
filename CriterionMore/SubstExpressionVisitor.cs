using System.Collections.Generic;
using System.Linq.Expressions;

namespace CriterionMore
{
    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression newValue;
            if (Subst.TryGetValue(node, out newValue))
            {
                return newValue;
            }
            return node;
        }
    }
}