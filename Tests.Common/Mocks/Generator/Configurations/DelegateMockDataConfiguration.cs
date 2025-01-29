using System.Linq.Expressions;

namespace Tests.Common.Mocks.Generator.Configurations;

internal sealed class DelegateMockDataConfiguration : IMockDataConfiguration
{
    public bool CanCreate(Type type)
        => typeof(Delegate).IsAssignableFrom(type)
        && type.GetMethod("Invoke") is not null;

    public object Create(Type type, IMockDataGenerator mockDataGenerator)
    {
        var invokeMethod = type.GetMethod("Invoke")!;
        var parameters = invokeMethod.GetParameters();
        var returnType = invokeMethod.ReturnType;

        // Define the parameter expressions for the delegate
        var parameterExpressions = parameters
            .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
            .ToArray();

        // Create a "do-nothing" expression
        Expression body;
        if (returnType == typeof(void))
        {
            // Action type: create an empty block
            body = Expression.Empty();
        }
        else
        {
            // Func type: return the default value for the return type
            body = Expression.Default(returnType);
        }

        return Expression
            .Lambda(type, body, parameterExpressions)
            .Compile();
    }
}
