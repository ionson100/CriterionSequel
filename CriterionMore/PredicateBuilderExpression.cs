using System;
using System.Linq.Expressions;

namespace CriterionMore
{
    /// <summary>
    /// Рисширение для конкатенации выражений
    /// </summary>
    internal static class BuilderExpression
    {

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            if (b == null)
                return a;

            ParameterExpression p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.Subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
        public static Expression<Func<T, object>> And<T>(this Expression<Func<T, object>> a, Expression<Func<T, object>> b)
        {

            if (b == null)
                return a;

            ParameterExpression p = a.Parameters[0];

            var visitor = new SubstExpressionVisitor();
            visitor.Subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, object>>(body, p);
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