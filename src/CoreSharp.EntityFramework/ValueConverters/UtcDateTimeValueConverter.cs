using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CoreSharp.EntityFramework.ValueConverters;

public sealed class UtcDateTimeValueConverter : ValueConverter<DateTime?, DateTime?>
{
    private static UtcDateTimeValueConverter _instance;

    public UtcDateTimeValueConverter()
        : base(
            convertToProviderExpression: appValue => appValue == null ? null : appValue.Value.ToUniversalTime(),
            convertFromProviderExpression: dbValue => dbValue == null ? null : dbValue.Value.ToUniversalTime())
    {
    }

    public static UtcDateTimeValueConverter Instance
        => _instance ??= new();
}
