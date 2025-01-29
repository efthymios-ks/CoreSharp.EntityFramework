using System.Globalization;
using System.Reflection;

namespace Tests.Common.Mocks;
public sealed class MockConstructorInfo : ConstructorInfo
{
    public MethodAttributes MethodAttributesOverride { get; set; } = MethodAttributes.Public;
    public Type? DeclaringTypeOverride { get; set; }
    public Type[] GetGenericArgumentsOverride { get; set; } = [];
    public ParameterInfo[] GetParametersOverride { get; set; } = [];
    public bool IsGenericMethodOverride { get; set; }

    public override Type? DeclaringType
        => DeclaringTypeOverride;

    public override bool IsGenericMethod
        => IsGenericMethodOverride;

    public override MethodAttributes Attributes
        => MethodAttributesOverride;

    public override Type[] GetGenericArguments()
        => GetGenericArgumentsOverride;

    public override ParameterInfo[] GetParameters()
        => GetParametersOverride;

    #region Not used

    public override RuntimeMethodHandle MethodHandle
        => throw new NotImplementedException();

    public override string Name
        => throw new NotImplementedException();

    public override Type? ReflectedType
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit)
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    public override MethodImplAttributes GetMethodImplementationFlags()
        => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    public override object Invoke(BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
        => throw new NotImplementedException();

    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
        => throw new NotImplementedException();
    #endregion
}
