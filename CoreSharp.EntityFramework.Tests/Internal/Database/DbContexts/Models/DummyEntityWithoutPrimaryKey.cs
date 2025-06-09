using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;

[Keyless]
public class DummyEntityWithoutPrimaryKey
{
    // Properties
    [MaxLength(100)]
    public string? Name { get; set; }
}
