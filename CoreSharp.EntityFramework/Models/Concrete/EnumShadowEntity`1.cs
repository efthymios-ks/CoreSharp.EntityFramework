using System;
using System.ComponentModel.DataAnnotations;

namespace CoreSharp.EntityFramework.Models.Concrete
{
    /// <summary>
    /// Enum shadow entity used for one-to-many relationship.
    /// </summary>
    public sealed class EnumShadowEntity<TEnum> where TEnum : Enum
    {
        //Constructors
        public EnumShadowEntity()
        {
        }

        public EnumShadowEntity(TEnum value)
        {
            Value = value;
            Name = $"{value}";
        }

        //Properties
        [Key]
        public TEnum Value { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
