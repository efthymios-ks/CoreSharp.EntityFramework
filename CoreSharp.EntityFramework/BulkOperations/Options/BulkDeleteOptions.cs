using System.Linq.Expressions;

namespace CoreSharp.EntityFramework.Bulkoperations.Options;

public sealed class BulkDeleteOptions<TEntity> : BulkOperationOptionsBase
    where TEntity : class
{
    public Expression<Func<TEntity, object?>>[] PropertiesToMatch { get; set; } = [];
}
