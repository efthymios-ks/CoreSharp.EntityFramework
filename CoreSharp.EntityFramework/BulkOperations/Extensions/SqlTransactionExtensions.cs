using Microsoft.Data.SqlClient;
using System.Data;

namespace CoreSharp.EntityFramework.Bulkoperations.Extensions;

internal static class SqlTransactionExtensions
{
    /// <summary>
    /// Creates temp table and populates it with data from the source table.<br/>
    /// Uses <see cref="SqlBulkCopy"/> internally to perform the bulk insert operation.<br/>
    /// No need to drop the temp table after use, as it will be automatically dropped when the connection is closed.
    /// </summary> 
    public static async Task<string> CreateTempTableAsync(
        this SqlTransaction transaction,
        DataTable sourceTable,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(sourceTable);

        var tempTableName = $"#Temp_{Guid.NewGuid():N}";

        // CREATE TABLE #TempTable
        var tempTableColumns = string.Join(", ", sourceTable
            .Columns
            .Cast<DataColumn>()
            .Select(column => $"{column.ColumnName} {column.DataType.GetSqlDbType()}")
        );

        var createTableSql = $"CREATE TABLE {tempTableName} ({tempTableColumns})";
        using var createTableCommand = new SqlCommand(createTableSql, transaction.Connection, transaction)
        {
            CommandType = CommandType.Text
        };

        await createTableCommand.ExecuteNonQueryAsync(cancellationToken);

        // INSERT INTO #TempTable
        await transaction.Connection.BulkCopyAsync(tempTableName, sourceTable, transaction, cancellationToken);

        // Return
        return tempTableName;
    }
}
