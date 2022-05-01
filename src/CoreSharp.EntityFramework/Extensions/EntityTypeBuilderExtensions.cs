using CoreSharp.EntityFramework.Entities.Concrete;
using CoreSharp.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="EntityTypeBuilder{TEntity}"/> extensions.
    /// </summary>
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// One-to-many relation with an <see cref="Enum"/>.
        /// The <see cref="Enum"/> must be first configured in <see cref="DbContext.OnModelCreating(ModelBuilder)"/>
        /// by calling <see cref="ModelBuilderExtensions.HasEnum{TEnum}(ModelBuilder, string)"/>.
        /// </summary>
        public static ReferenceCollectionBuilder<EnumShadowEntity<TEnum>, TEntity> HasEnum<TEntity, TEnum>(
            this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TEnum>> propertySelector)
            where TEntity : class
            where TEnum : Enum
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));

            var enumPropertyName = propertySelector.GetMemberName();
            return builder.HasOne<EnumShadowEntity<TEnum>>()
                          .WithMany()
                          .HasForeignKey(enumPropertyName);
        }

        /// <summary>
        /// Many-to-many relation with an <see cref="Enum"/>.
        /// Currently stored as json without any relationship validation.
        /// </summary>
        public static PropertyBuilder<ICollection<TEnum>> HasEnums<TEntity, TEnum>(
            this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, ICollection<TEnum>>> propertySelector)
            where TEntity : class
            where TEnum : Enum
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = propertySelector ?? throw new ArgumentNullException(nameof(propertySelector));

            return builder.Property(propertySelector)
                          .HasJsonConversion();
        }
    }
}
