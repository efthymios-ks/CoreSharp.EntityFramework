using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using JsonNet = Newtonsoft.Json;
using TextJson = System.Text.Json;

namespace CoreSharp.EntityFramework.Extensions;

/// <summary>
/// <see cref="PropertyBuilder{TProperty}"/> extensions.
/// </summary>
public static class PropertyBuilderExtensions
{
    /// <inheritdoc cref="HasJsonConversion{TProperty}(PropertyBuilder{TProperty}, JsonNet.JsonSerializerSettings)"/>
    public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(
        this PropertyBuilder<TProperty> builder)
        where TProperty : class
        => builder.HasJsonConversion(TextJson.JsonSerializerOptions.Default);

    /// <inheritdoc cref="HasJsonConversionInternal{TProperty}(PropertyBuilder{TProperty}, Func{TProperty, string}, Func{string, TProperty})"/>
    public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(
        this PropertyBuilder<TProperty> builder,
        TextJson.JsonSerializerOptions options)
        where TProperty : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        return builder.HasJsonConversionInternal(ToJson, FromJson);

        string ToJson(TProperty property)
            => TextJson.JsonSerializer.Serialize(property, options);

        TProperty FromJson(string json)
            => TextJson.JsonSerializer.Deserialize<TProperty>(json, options);
    }

    /// <inheritdoc cref="HasJsonConversionInternal{TProperty}(PropertyBuilder{TProperty}, Func{TProperty, string}, Func{string, TProperty})"/>
    public static PropertyBuilder<TProperty> HasJsonConversion<TProperty>(
        this PropertyBuilder<TProperty> builder,
        JsonNet.JsonSerializerSettings settings)
        where TProperty : class
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(settings);

        return builder.HasJsonConversionInternal(ToJson, FromJson);

        string ToJson(TProperty property)
            => JsonNet.JsonConvert.SerializeObject(property, settings);

        TProperty FromJson(string json)
            => JsonNet.JsonConvert.DeserializeObject<TProperty>(json, settings);
    }

    /// <summary>
    /// Convert a property from and to json for database storage.
    /// </summary>
    private static PropertyBuilder<TProperty> HasJsonConversionInternal<TProperty>(
        this PropertyBuilder<TProperty> builder,
        Func<TProperty, string> toJson,
        Func<string, TProperty> fromJson)
        where TProperty : class
    {
        var converter = new ValueConverter<TProperty, string>(
            appValue => toJson(appValue),
            dbValue => fromJson(dbValue));

        var comparer = new ValueComparer<TProperty>(
            (left, right) => toJson(left) == toJson(right),
            value => value == null ? 0 : value.GetHashCode(),
            value => fromJson(toJson(value)));  // Clone 

        builder.HasConversion(converter);
        builder.Metadata.SetValueConverter(converter);
        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }

    /// <inheritdoc cref="HasUtcConversion(PropertyBuilder{DateTime?})" />
    public static PropertyBuilder<DateTime> HasUtcConversion(this PropertyBuilder<DateTime> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var converter = new ValueConverter<DateTime, DateTime>(
            appValue => appValue.ToUniversalTime(),
            dbValue => TimeZoneInfo.ConvertTimeToUtc(dbValue, TimeZoneInfo.Utc)
        );

        var comparer = new ValueComparer<DateTime>(
            (left, right) => left == right,
            value => value.GetHashCode(),
            value => value  // DateTime is a struct, so an assignment will copy the value. 
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
        ArgumentNullException.ThrowIfNull(builder);

        var converter = new ValueConverter<DateTime?, DateTime?>(
            appValue => appValue == null ? null : appValue.Value.ToUniversalTime(),
            dbValue => dbValue == null ? null : TimeZoneInfo.ConvertTimeToUtc(dbValue.Value, TimeZoneInfo.Utc)
        );

        var comparer = new ValueComparer<DateTime?>(
            (left, right) => Equals(left, right),
            value => value == null ? 0 : value.Value.GetHashCode(),
            value => value  // DateTime is a struct, so an assignment will copy the value. 
        );

        builder.HasConversion(converter);
        builder.Metadata.SetValueConverter(converter);
        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }
}
