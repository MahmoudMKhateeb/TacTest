using System;
using System.Linq;
using System.Linq.Expressions;

namespace TACHYON.Extension
{
    public static class ExpressionCombiner
    {
        public static Expression<Func<TEntity, bool>> CombineExpressions<TEntity>(
            params Expression<Func<TEntity, bool>>[] expressions)
            where TEntity : class
        {
            if (expressions == null || expressions.Length == 0)
            {
                return t => true;
            }

            var parameter = Expression.Parameter(typeof(TEntity), "x");

            var combinedExpression = expressions
                .Select(expr => ReplaceParameter(expr.Body, expr.Parameters[0], parameter))
                .Aggregate(Expression.AndAlso);

            return Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
        }

        private static Expression ReplaceParameter(Expression expression, ParameterExpression sourceParameter,
            ParameterExpression targetParameter)
        {
            return new ReplaceParameterVisitor(sourceParameter, targetParameter).Visit(expression);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _sourceParameter;
            private readonly ParameterExpression _targetParameter;

            public ReplaceParameterVisitor(ParameterExpression sourceParameter, ParameterExpression targetParameter)
            {
                _sourceParameter = sourceParameter;
                _targetParameter = targetParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _sourceParameter ? _targetParameter : base.VisitParameter(node);
            }
        }
    }
}