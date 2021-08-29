using System;
using System.Linq.Expressions;

namespace List.Extensions
{
    public static class ExpressionExtensions<T>
    {
        public static Expression<Func<T, bool>> AndAlso(Expression<Func<T, bool>> leftExpr, Expression<Func<T, bool>> rightExpr)
        {
            var exprBody = Expression.AndAlso(leftExpr.Body, rightExpr.Body);
            return ReplaceParam(exprBody);
        }

        private static Expression<Func<T, bool>> ReplaceParam(Expression exprBody)
        {
            var exprParam = Expression.Parameter(typeof(T));
            exprBody = (BinaryExpression)new ReplaceParameterVisitor(exprParam).Visit(exprBody);
            return Expression.Lambda<Func<T, bool>>(exprBody, exprParam);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(_parameter);
            }

            internal ReplaceParameterVisitor(ParameterExpression parameter)
            {
                _parameter = parameter;
            }
        }

    }
}