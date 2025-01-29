using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockParameterInfo : ParameterInfo
{
    public MockParameterInfo()
    {
        MemberInfoOverride = new MockMemberInfo();
        ParameterTypeOverride = new MockType();
    }

    public MemberInfo MemberInfoOverride
    {
        get => MemberImpl;
        set => MemberImpl = value;
    }

    public string? NameOverride
    {
        get => NameImpl;
        set => NameImpl = value;
    }

    public bool IsOutOverride
    {
        get => IsOut;
        set => SetAttribute(ParameterAttributes.Out, value);
    }

    public bool IsInOverride
    {
        get => IsIn;
        set => SetAttribute(ParameterAttributes.In, value);
    }

    public bool HasDefaultValueOverride { get; set; }

    public object? DefaultValueOverride { get; set; }

    public Type[] IsDefinedOverrides { get; set; } = [];

    public Attribute[] GetCustomAttributesOverride { get; set; } = [];

    public Type? ParameterTypeOverride
    {
        get => ClassImpl;
        set => ClassImpl = value;
    }

    public override bool HasDefaultValue
        => HasDefaultValueOverride;

    public override object? DefaultValue
        => DefaultValueOverride;

    public override bool IsDefined(Type attributeType, bool inherit)
        => IsDefinedOverrides.Contains(attributeType);

    public override object[] GetCustomAttributes(bool inherit)
        => GetCustomAttributesOverride;

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => GetCustomAttributesOverride;

    private void SetAttribute(ParameterAttributes attribute, bool value)
    {
        if (value)
        {
            AttrsImpl |= attribute;
        }
        else
        {
            AttrsImpl &= ~attribute;
        }
    }
}
