using System.Linq.Expressions;
using System.Reflection;

namespace CoreSharp.EntityFramework.Bulkoperations.Extensions;

internal static class LambdaExpressionExtensions
{
    public static PropertyInfo GetProperty(this LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var body = expression.Body;
        if (body is MemberExpression memberExpression
            && memberExpression.Member is PropertyInfo memberProperty
        )
        {
            return memberProperty;
        }

        if (body is UnaryExpression unaryExpression
            && unaryExpression.Operand is MemberExpression operandMemberExpression
            && operandMemberExpression.Member is PropertyInfo operandMemberProperty)
        {
            return operandMemberProperty;
        }

        throw new InvalidOperationException("The expression does not represent a property access.");
    }
}
