using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreSharp.EntityFramework.Tests.Internal.Database.DbContexts.Models;

public class DummyWithPrimaryKeyAndNoValueGeneration
{
    // Properties
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Name { get; set; }
}
