using CoreSharp.EntityFramework.Entities.Abstracts;

namespace Tests.Internal.Models;

internal sealed class DummyEntity : EntityBase<int>
{
    // Properties 
    public string Name { get; set; }
}

internal enum DummyEnumeration
{
    Value1,
    Value2
}