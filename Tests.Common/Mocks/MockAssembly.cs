using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockAssembly : Assembly
{
    public string? AssemblyNameOverride { get; set; }

    public override AssemblyName GetName()
        => new()
        {
            Name = AssemblyNameOverride
        };
}
