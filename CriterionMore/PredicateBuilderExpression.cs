using System;
using System.Linq.Expressions;

namespace CriterionMore
{
    /// <summary>
    /// Рисширение для конкатенации выражений
    /// </summary>
    internal static class PredicateBuilderExpression
    {

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            if (b == null)
                return a;

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.Subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
            if (b == null)
                return a;

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.Subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}