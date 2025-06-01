using Microsoft.Data.SqlClient;
using System.Data;

namespace CoreSharp.EntityFramework.Bulkoperations.Extensions;

internal static class SqlConnectionExtensions
{
    public static async Task BulkCopyAsync(
        this SqlConnection connection,
        string tableName,
        DataTable dataTable,
        SqlTransaction? transaction = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
        ArgumentNullException.ThrowIfNull(dataTable);

        var isConnectionOwned = false;
        if (!connection.State.HasFlag(ConnectionState.Open))
        {
            await connection.OpenAsync(cancellationToken);
            isConnectionOwned = true;
        }

        SqlBulkCopy sqlBulkCopy = null!;
        try
        {
            sqlBulkCopy = new(connection, SqlBulkCopyOptions.Default, transaction)
            {
                DestinationTableName = tableName,
                BatchSize = 50_000
            };

            foreach (DataColumn column in dataTable.Columns)
            {
                sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            await sqlBulkCopy.WriteToServerAsync(dataTable, cancellationToken);
        }
        finally
        {
            ((IDisposable)sqlBulkCopy).Dispose();

            if (isConnectionOwned)
            {
                await connection.CloseAsync();
            }
        }
    }
}
