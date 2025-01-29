using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockType : Type
{
    public Assembly? AssemblyOverride { get; set; }
    public Type? DeclaringTypeOverride { get; set; }
    public string? FullNameOverride { get; set; }
    public Attribute[] GetCustomAttributesOverride { get; set; } = [];
    public FieldInfo[] GetFieldsOverride { get; set; } = [];
    public Type? GetGenericTypeDefinitionOverride { get; set; }
    public int? GetHashCodeOverride { get; set; }
    public Type[] GetInterfacesOverride { get; set; } = [];
    public MethodInfo[] GetMethodsOverride { get; set; } = [];
    public bool IsArrayImplOverride { get; set; }
    public bool IsByRefImplOverride { get; set; }
    public bool IsGenericTypeOverride { get; set; }
    public string? NameOverride { get; set; }
    public string? NamespaceOverride { get; set; }

    public override Assembly Assembly
        => AssemblyOverride!;

    public override Type? DeclaringType
        => DeclaringTypeOverride;

    public override string? FullName
        => FullNameOverride;

    public override bool IsGenericType
        => IsGenericTypeOverride;

    public override string Name
        => NameOverride!;

    public override string? Namespace
        => NamespaceOverride;

    public override object[] GetCustomAttributes(bool inherit)
        => GetCustomAttributesOverride;

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        => GetCustomAttributesOverride;

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        => GetFieldsOverride;

    public override Type GetGenericTypeDefinition()
        => GetGenericTypeDefinitionOverride ?? base.GetGenericTypeDefinition();

    public override int GetHashCode()
        => GetHashCodeOverride ?? default;

    public override Type[] GetInterfaces()
        => GetInterfacesOverride;

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        => GetMethodsOverride;

    protected override bool IsArrayImpl()
        => IsArrayImplOverride;

    protected override bool IsByRefImpl()
        => IsByRefImplOverride;

    #region Not used
    public override string? AssemblyQualifiedName
        => throw new NotImplementedException();

    public override Type? BaseType
        => throw new NotImplementedException();

    public override Guid GUID
        => throw new NotImplementedException();

    public override Module Module
        => throw new NotImplementedException();

    public override Type UnderlyingSystemType
        => throw new NotImplementedException();

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override Type? GetElementType()
        => throw new NotImplementedException();

    public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
    public override Type? GetInterface(string name, bool ignoreCase)
        => throw new NotImplementedException();

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        => throw new NotImplementedException();

    public override bool IsDefined(Type attributeType, bool inherit)
        => throw new NotImplementedException();

    protected override TypeAttributes GetAttributeFlagsImpl()
        => throw new NotImplementedException();

    protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        => throw new NotImplementedException();

    protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        => throw new NotImplementedException();

    protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        => throw new NotImplementedException();

    protected override bool HasElementTypeImpl()
        => throw new NotImplementedException();

    protected override bool IsCOMObjectImpl()
        => throw new NotImplementedException();

    protected override bool IsPointerImpl()
        => throw new NotImplementedException();

    protected override bool IsPrimitiveImpl()
        => throw new NotImplementedException();
    public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        => throw new NotImplementedException();
    #endregion
}
