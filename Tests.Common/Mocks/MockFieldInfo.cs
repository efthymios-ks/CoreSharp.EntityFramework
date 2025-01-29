using System.Globalization;
using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockFieldInfo : FieldInfo
{
    public string? NameOverride { get; set; }
    public Type? DeclaringTypeOverride { get; set; }
    public Type? FieldTypeOverride { get; set; }
    public object? GetValueOverride { get; set; }
    public override string Name
        => NameOverride!;

    public override Type? DeclaringType
        => DeclaringTypeOverride!;

    public override Type FieldType
        => FieldTypeOverride!;

    public override object? GetValue(object? obj)
        => GetValueOverride;

    #region Not used
    public override FieldAttributes Attributes
        => throw new NotImplementedException();

    public override RuntimeFieldHandle FieldHandle
        => throw new NotImplementedException();

    public override Type? ReflectedType
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit)
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, CultureInfo? culture)
        => throw new NotImplementedException();
    #endregion
}
