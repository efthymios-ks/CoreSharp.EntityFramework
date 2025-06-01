namespace CoreSharp.EntityFramework.Bulkoperations.Extensions;

internal static class TypeExtensions
{
    public static string GetSqlDbType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type.IsEnum)
        {
            type = Enum.GetUnderlyingType(type);
        }

        return type switch
        {
            _ when type == typeof(string) => "nvarchar(max)",
            _ when type == typeof(char) => "nchar(1)",
            _ when type == typeof(byte[]) => "varbinary(max)",
            _ when type == typeof(int) => "int",
            _ when type == typeof(uint) => "bigint",
            _ when type == typeof(long) => "bigint",
            _ when type == typeof(ulong) => "decimal(20,0)",
            _ when type == typeof(short) => "smallint",
            _ when type == typeof(ushort) => "int",
            _ when type == typeof(byte) => "tinyint",
            _ when type == typeof(sbyte) => "smallint",
            _ when type == typeof(decimal) => "decimal(18,2)",
            _ when type == typeof(double) => "float",
            _ when type == typeof(float) => "real",
            _ when type == typeof(bool) => "bit",
            _ when type == typeof(DateTime) => "datetime2",
            _ when type == typeof(DateOnly) => "date",
            _ when type == typeof(DateTimeOffset) => "datetimeoffset",
            _ when type == typeof(TimeSpan) => "time",
            _ when type == typeof(TimeOnly) => "time",
            _ when type == typeof(Guid) => "uniqueidentifier",
            _ => throw new NotSupportedException($"The type '{type.FullName}' is not supported for SQL column mapping.")
        };
    }
}
