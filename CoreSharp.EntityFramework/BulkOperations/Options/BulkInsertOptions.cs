using System.Linq.Expressions;

namespace CoreSharp.EntityFramework.Bulkoperations.Options;

public sealed class BulkInsertOptions<TEntity> : BulkOperationOptionsBase
    where TEntity : class
{
    public Expression<Func<TEntity, object>>[] PropertiesToMatch { get; set; } = [];
    public Expression<Func<TEntity, object?>>[] PropertiesToInsert { get; set; } = [];
}
