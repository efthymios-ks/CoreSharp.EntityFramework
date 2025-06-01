using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace CoreSharp.EntityFramework.Bulkoperations;

internal sealed class SqlConnectionWrapper : IAsyncDisposable
{
    private bool _isDisposed = false;

    public SqlConnection Connection { get; private set; } = null!;
    public SqlTransaction Transaction { get; private set; } = null!;
    public IDbContextTransaction EfTransaction { get; private set; } = null!;
    public bool IsConnectionOwned { get; private set; }
    public bool IsTransactionOwned { get; private set; }

    public static async Task<SqlConnectionWrapper> EnsureConnectAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        if (dbContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.SqlServer")
        {
            throw new InvalidOperationException("The database provider is not SQL Server.");
        }

        var connection = (SqlConnection)dbContext.Database.GetDbConnection();
        var isConnectionOwned = false;
        if (!connection.State.HasFlag(ConnectionState.Open))
        {
            await connection.OpenAsync(cancellationToken);
            isConnectionOwned = true;
        }

        var efTransaction = dbContext.Database.CurrentTransaction;
        var transaction = efTransaction?.GetDbTransaction() as SqlTransaction;
        var isTransactionOwned = false;
        if (efTransaction is null)
        {
            efTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            transaction = (SqlTransaction)efTransaction.GetDbTransaction();
            isTransactionOwned = true;
        }

        return new SqlConnectionWrapper
        {
            Connection = connection,
            Transaction = transaction!,
            EfTransaction = efTransaction!,
            IsConnectionOwned = isConnectionOwned,
            IsTransactionOwned = isTransactionOwned
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        if (IsTransactionOwned)
        {
            await EfTransaction.CommitAsync();
            await EfTransaction.DisposeAsync();
            await Transaction.DisposeAsync();
        }

        if (IsConnectionOwned)
        {
            await Connection.CloseAsync();
        }
    }
}
