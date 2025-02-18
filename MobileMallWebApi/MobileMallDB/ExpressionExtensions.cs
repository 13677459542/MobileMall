using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MobileMallDB
{
    /// <summary>
    /// 添加自定义的扩展方法，用于动态生成表达式。
    /// </summary>
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var combined = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
            combined = Expression.AndAlso(combined, new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }
    }
}
