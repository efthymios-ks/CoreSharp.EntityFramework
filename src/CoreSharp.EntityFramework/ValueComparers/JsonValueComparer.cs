using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using TextJson = System.Text.Json;

namespace CoreSharp.EntityFramework.ValueComparers;

public sealed class JsonValueComparer<TValue> : ValueComparer<TValue>
{
    private static JsonValueComparer<TValue> _default;

    public JsonValueComparer(
        Func<TValue, string> toJson,
        Func<string, TValue> fromJson)
        : base(
            equalsExpression: (left, right) => toJson(left) == toJson(right),
            hashCodeExpression: value => value == null ? 0 : value.GetHashCode(),
            snapshotExpression: value => fromJson(toJson(value)))
    {
    }

    public static JsonValueComparer<TValue> Default
    {
        get
        {
            if (_default == null)
            {
                var options = TextJson.JsonSerializerOptions.Default;
                _default = new JsonValueComparer<TValue>(ToJson, FromJson);

                string ToJson(TValue value)
                    => TextJson.JsonSerializer.Serialize(value, options);
                TValue FromJson(string json)
                    => TextJson.JsonSerializer.Deserialize<TValue>(json, options);
            }

            return _default;
        }
    }
}