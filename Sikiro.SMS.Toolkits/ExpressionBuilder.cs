using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sikiro.SMS.Toolkits
{
    public static class ExpressionBuilder
    {
        /// <summary>
        /// 默认True条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Init<T>()
        {
            return expression => true;
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Merge(first, second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return Merge(first, second, Expression.OrElse);
        }

        private static Expression<T> Merge<T>(Expression<T> oldExpression, Expression<T> newExpression, Func<Expression, Expression, Expression> combineType)
        {
            var afterRebindNewExpression = ParameterRebinder<T>.ReplaceParameters(oldExpression, newExpression);

            return Expression.Lambda<T>(combineType(oldExpression.Body, afterRebindNewExpression), oldExpression.Parameters);
        }
    }
    internal class ParameterRebinder<T> : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap;

        ParameterRebinder(Expression<T> oldExpression, Expression<T> newExpression)
        {
            _parameterMap = oldExpression.Parameters
                .Select((f, i) => new { f, s = newExpression.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
        }

        public static Expression ReplaceParameters(Expression<T> oldExpression, Expression<T> newExpression)
        {
            return new ParameterRebinder<T>(oldExpression, newExpression).Visit(newExpression.Body);
        }

        protected override Expression VisitParameter(ParameterExpression newParameter)
        {
            if (_parameterMap.TryGetValue(newParameter, out var replacement))
            {
                newParameter = replacement;
            }

            return base.VisitParameter(newParameter);
        }
    }
}
