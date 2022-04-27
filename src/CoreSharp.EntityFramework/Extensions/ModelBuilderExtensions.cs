using CoreSharp.EntityFramework.Models.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pluralize.NET.Core;
using System;
using System.Diagnostics;
using System.Linq;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="ModelBuilder"/> extensions.
    /// </summary>
    public static class ModelBuilderExtensions
    {
        //Fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static Pluralizer _pluralizer;

        //Methods
        /// <inheritdoc cref="HasEnum{TEnum}(ModelBuilder, string)" />
        public static ModelBuilder HasEnum<TEnum>(this ModelBuilder builder)
            where TEnum : Enum
        {
            _pluralizer ??= new Pluralizer();
            var enumName = typeof(TEnum).Name;
            var tableName = _pluralizer.Pluralize(enumName);
            return builder.HasEnum<TEnum>(tableName);
        }

        /// <summary>
        /// Configure and seed <see cref="Enum"/> to database table.
        /// </summary>
        public static ModelBuilder HasEnum<TEnum>(this ModelBuilder builder, string tableName)
            where TEnum : Enum
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            builder.ConfigureEnum<TEnum>(tableName);
            builder.SeedEnum<TEnum>();
            return builder;
        }

        /// <summary>
        /// Configure database column type for given <see cref="Enum"/>.
        /// </summary>
        private static EntityTypeBuilder ConfigureEnum<TEnum>(this ModelBuilder builder, string tableName) where TEnum : Enum
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));

            var entityBuilder = builder.Entity<EnumShadowEntity<TEnum>>();
            entityBuilder.ToTable(tableName);
            entityBuilder.HasKey(e => e.Value);
            entityBuilder.Property(e => e.Value);
            entityBuilder.Property(e => e.Name).IsRequired();
            return entityBuilder;
        }

        /// <summary>
        /// Seed data to database column for given enum <see cref="Enum"/>.
        /// </summary>
        private static void SeedEnum<TEnum>(this ModelBuilder builder) where TEnum : Enum
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var entries = Enum.GetValues(typeof(TEnum))
                              .Cast<TEnum>()
                              .Select(v => new EnumShadowEntity<TEnum>(v));
            var entityBuilder = builder.Entity<EnumShadowEntity<TEnum>>();
            foreach (var entry in entries)
                entityBuilder.HasData(entry);
        }
    }
}
