using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockMemberInfo : MemberInfo
{
    public MemberTypes MemberTypeOverride { get; set; } = MemberTypes.Property;

    public override MemberTypes MemberType
        => MemberTypeOverride;

    #region Not used
    public override Type? DeclaringType
        => throw new NotImplementedException();

    public override string Name
        => throw new NotImplementedException();

    public override Type? ReflectedType
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(bool inherit)
        => throw new NotImplementedException();

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();
    #endregion
}
