using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CoreSharp.EntityFramework.Tests.Internal.Database;

public static class DbContextExtensions
{
    public static async Task EnsureValueGeneratedConstraintsAsync(this DbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        foreach (var entityType in dbContext.Model.GetEntityTypes())
        {
            var schema = entityType.GetSchema() ?? "dbo";
            var tableNameOnly = entityType.GetTableName()!;
            var fullTableName = $"[{schema}].[{tableNameOnly}]";
            var keyProperties = entityType
                .GetProperties()
                .Where(property => property.ValueGenerated != ValueGenerated.Never)
                .ToArray();

            foreach (var property in keyProperties)
            {
                var columnName = property.GetColumnName();
                var constraintName = $"DF_{tableNameOnly}_{columnName}";
                var query = $@"
                    IF NOT EXISTS (
                        SELECT 1
                        FROM sys.default_constraints dc
                        JOIN sys.columns c 
                            ON dc.parent_object_id = c.object_id 
                            AND dc.parent_column_id = c.column_id
                        JOIN sys.tables t 
                            ON t.object_id = c.object_id
                        WHERE t.name = '{tableNameOnly}'
                          AND c.name = '{columnName}'
                          AND dc.name = '{constraintName}'
                    )
                    BEGIN
                        ALTER TABLE {fullTableName}
                        ADD CONSTRAINT {constraintName}
                        DEFAULT NEWID() FOR [{columnName}];
                    END
                ";

                await dbContext.Database.ExecuteSqlRawAsync(query);
            }
        }
    }
}
