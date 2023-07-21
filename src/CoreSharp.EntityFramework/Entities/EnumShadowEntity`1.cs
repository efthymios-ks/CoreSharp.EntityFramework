using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreSharp.EntityFramework.Entities;

/// <summary>
/// <see cref="Enum"/> shadow entity used for table relationships.
/// </summary>
public sealed class EnumShadowEntity<TEnum>
    where TEnum : Enum
{
    // Constructors
    public EnumShadowEntity()
    {
    }

    public EnumShadowEntity(TEnum value)
    {
        Value = value;
        Name = $"{value}";
    }

    // Properties
    [Key]
    [Column(Order = 0)]
    public TEnum Value { get; set; }

    [Required]
    [Column(Order = 1)]
    public string Name { get; set; }
}
