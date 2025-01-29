using System.Globalization;
using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockMethodInfo : MethodInfo
{
    public Type? DeclaringTypeOverride { get; set; }
    public Attribute[] GetCustomAttributesOverride { get; set; } = [];
    public Type[] GetGenericArgumentsOverride { get; set; } = [];
    public ParameterInfo[] GetParametersOverride { get; set; } = [];
    public bool IsGenericMethodOverride { get; set; }
    public string NameOverride { get; set; } = null!;
    public Type? ReflectedTypeOverride { get; set; }
    public ParameterInfo ReturnParameterOverride { get; set; } = new MockParameterInfo();

    public override Type? DeclaringType
        => DeclaringTypeOverride;

    public override bool IsGenericMethod
        => IsGenericMethodOverride;

    public override string Name
        => NameOverride;

    public override Type? ReflectedType
        => ReflectedTypeOverride;

    public override ParameterInfo ReturnParameter
        => ReturnParameterOverride;

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => GetCustomAttributesOverride
            .Where(attribute => attribute.GetType() == attributeType)
            .ToArray();

    public override object[] GetCustomAttributes(bool inherit)
        => GetCustomAttributesOverride;

    public override Type[] GetGenericArguments()
        => GetGenericArgumentsOverride;

    public override ParameterInfo[] GetParameters()
        => GetParametersOverride;

    #region Not used
    public override ICustomAttributeProvider ReturnTypeCustomAttributes
        => throw new NotImplementedException();

    public override MethodAttributes Attributes
        => throw new NotImplementedException();

    public override RuntimeMethodHandle MethodHandle
        => throw new NotImplementedException();

    public override MethodInfo GetBaseDefinition()
        => throw new NotImplementedException();

    public override MethodImplAttributes GetMethodImplementationFlags()
        => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();
    public override object? Invoke(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? parameters, CultureInfo? culture)
        => throw new NotImplementedException();
    #endregion
}
