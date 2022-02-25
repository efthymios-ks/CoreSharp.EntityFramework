using CoreSharp.Extensions;
using CoreSharp.Models.Newtonsoft.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;

namespace CoreSharp.EntityFramework.Extensions
{
    /// <summary>
    /// <see cref="PropertyBuilder{TProperty}"/> extensions.
    /// </summary>
    public static class PropertyBuilderExtensions
    {
        /// <inheritdoc cref="HasJsonConversion{TProperty}(PropertyBuilder{TProperty}, JsonSerializerSettings)"/>
        public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(this PropertyBuilder<TProperty> builder)
            where TProperty : class
            => builder.HasJsonConversion(DefaultJsonSettings.Instance);

        /// <summary>
        /// Convert a property from and to json for database storage.
        /// </summary>
        public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(this PropertyBuilder<TProperty> builder, JsonSerializerSettings settings)
            where TProperty : class
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            _ = settings ?? throw new ArgumentNullException(nameof(settings));

            var converter = new ValueConverter<TProperty, string>(
                appValue => appValue.ToJson(settings),
                dbValue => dbValue.FromJson<TProperty>(settings));

            var comparer = new ValueComparer<TProperty>(
                (left, right) => left.ToJson(settings) == right.ToJson(settings),
                value => value == null ? 0 : value.GetHashCode(),
                value => value.JsonClone(settings));

            builder.HasConversion(converter);
            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            return builder;
        }

        /// <inheritdoc cref="HasUtcConversion(PropertyBuilder{DateTime?})" />
        public static PropertyBuilder<DateTime> HasUtcConversion(this PropertyBuilder<DateTime> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var converter = new ValueConverter<DateTime, DateTime>(
                appValue => appValue.ToUniversalTime(),
                dbValue => TimeZoneInfo.ConvertTimeToUtc(dbValue, TimeZoneInfo.Utc)
            );

            var comparer = new ValueComparer<DateTime>(
                (left, right) => left == right,
                value => value.GetHashCode(),
                value => value  //DateTime is a struct, so an assignment will copy the value. 
            );

            builder.HasConversion(converter);
            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            return builder;
        }

        /// <summary>
        /// If needed, converts <see cref="DateTime" /> to UTC from and to database.
        /// </summary>
        public static PropertyBuilder<DateTime?> HasUtcConversion(this PropertyBuilder<DateTime?> builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            var converter = new ValueConverter<DateTime?, DateTime?>(
                appValue => appValue == null ? null : appValue.Value.ToUniversalTime(),
                dbValue => dbValue == null ? null : TimeZoneInfo.ConvertTimeToUtc(dbValue.Value, TimeZoneInfo.Utc)
            );

            var comparer = new ValueComparer<DateTime?>(
                (left, right) => Equals(left, right),
                value => value == null ? 0 : value.Value.GetHashCode(),
                value => value  //DateTime is a struct, so an assignment will copy the value. 
            );

            builder.HasConversion(converter);
            builder.Metadata.SetValueConverter(converter);
            builder.Metadata.SetValueComparer(comparer);

            return builder;
        }
    }
}
