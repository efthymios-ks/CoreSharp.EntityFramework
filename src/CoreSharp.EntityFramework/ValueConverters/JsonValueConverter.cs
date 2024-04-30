using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using TextJson = System.Text.Json;

namespace CoreSharp.EntityFramework.ValueConverters;

public sealed class JsonValueConverter<TValue> : ValueConverter<TValue, string>
{
    private static JsonValueConverter<TValue> _default;

    public JsonValueConverter(
        Func<TValue, string> toJson,
        Func<string, TValue> fromJson)
        : base(
            convertToProviderExpression: appValue => toJson(appValue),
            convertFromProviderExpression: dbValue => fromJson(dbValue))
    {
    }

    public static JsonValueConverter<TValue> Default
    {
        get
        {
            if (_default == null)
            {
                var options = TextJson.JsonSerializerOptions.Default;
                _default = new JsonValueConverter<TValue>(ToJson, FromJson);

                string ToJson(TValue value)
                    => TextJson.JsonSerializer.Serialize(value, options);
                TValue FromJson(string json)
                    => TextJson.JsonSerializer.Deserialize<TValue>(json, options);
            }

            return _default;
        }
    }
}