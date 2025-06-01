using CoreSharp.EntityFramework.Entities.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;

public class DummyEntity : EntityBase<Guid>
{
    // Properties
    [MaxLength(100)]
    public string? Name { get; set; }
}
