﻿using CoreSharp.EntityFramework.Bulkoperations.Options;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace CoreSharp.EntityFramework.Bulkoperations.Extensions;

public static class DbContextExtensions
{
    private const string IndexColumnName = "__Index";
    private const string ActionColumnName = "__Action";

    /// <summary>
    /// Bulk inserts entities into the database using a temporary table for performance optimization.<br/>
    /// Skips matching entities based on primary key or specified properties.<br/>
    /// Maps auto-generated primary key values back to the entities after insertion.
    /// </summary>
    public static async Task BulkInsertAsync<TEntity>(
        this DbContext dbContext,
        TEntity[] entities,
        BulkInsertOptions<TEntity>? options = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return;
        }

        // Options preparation
        options ??= new();
        var optionsPropertiesToMatch = ToDictionary(options.PropertiesToMatch);
        var optionsPropertiesToInsert = ToDictionary(options.PropertiesToInsert);

        // EF preparation
        var efEntityType = dbContext.GetEfEntityType<TEntity>();
        var efKeyProperties = efEntityType.GetEfPrimaryKeyProperties();
        var efProperties = efEntityType.GetEfProperties();
        var efAutoGeneratedProperties = efKeyProperties
            .Where(property => property.ValueGenerated != ValueGenerated.Never)
            .ToArray();

        // Keep order of properties specified (e.g. to match indices)
        var efPropertiesToMatch = optionsPropertiesToMatch.Count > 0
            ? optionsPropertiesToMatch
                .Select(propertyToMatch => efProperties.First(efProperty => efProperty.Name == propertyToMatch.Key))
                .ToArray()
            : [.. efKeyProperties];

        var efPropertiesToInsert = optionsPropertiesToInsert.Count > 0
            ? efProperties.Where(property => optionsPropertiesToInsert.ContainsKey(property.Name)).ToArray()
            : [.. efProperties];

        // Do not insert auto-generated properties
        efPropertiesToInsert = [.. efPropertiesToInsert
            .Where(property => !efAutoGeneratedProperties
            .Any(autoGeneratedProperty => autoGeneratedProperty.PropertyInfo == property.PropertyInfo))
        ];

        if (efPropertiesToInsert.Length == 0)
        {
            throw new InvalidOperationException($"The entity type '{typeof(TEntity).FullName}' does not have any properties to insert with current configuration.");
        }

        var propertiesForTempTable = efPropertiesToInsert
            .Union(efPropertiesToMatch)
            .Distinct()
            .ToArray();

        // Execute
        var tableName = efEntityType.GetSchemaQualifiedTableName()!;
        using (var sourceTable = GetDataTable(propertiesForTempTable, entities))
        await using (var sqlConnectionWrapper = await SqlConnectionWrapper.EnsureConnectAsync(dbContext, cancellationToken))
        {
            // CREATE TABLE + INSERT TO #TempTable
            var tempTableName = await sqlConnectionWrapper.Transaction.CreateTempTableAsync(sourceTable, cancellationToken);

            // INSERT INTO TargetTable
            var insertColumns = string.Join(", ", efPropertiesToInsert.Select(property => $"[{property.GetColumnName()}]"));
            var selectFromColumns = string.Join(", ", efPropertiesToInsert.Select(property => $"Source.[{property.GetColumnName()}]"));
            var leftJoinOnColumns = string.Join(" AND ", efPropertiesToMatch.Select(property =>
            {
                var columnName = property.GetColumnName();
                return $"Target.[{columnName}] = Source.[{columnName}]";
            }));

            var whereCondition = string.Join(" AND ", efPropertiesToMatch.Select(property => $"Target.[{property.GetColumnName()}] IS NULL"));
            var hasOutputColumns = efAutoGeneratedProperties.Length > 0;
            var outputStatement = string.Empty;
            if (hasOutputColumns)
            {
                var outputColumns = string.Join(", ", efAutoGeneratedProperties.Select(property => $"INSERTED.[{property.GetColumnName()}]"));
                outputStatement = "OUTPUT " + outputColumns;
            }

            var insertSql = $"""
            INSERT INTO {tableName} 
            ({insertColumns})
            {outputStatement}
            SELECT {selectFromColumns}
            FROM {tempTableName} AS Source
            LEFT JOIN {tableName} AS Target
            ON {leftJoinOnColumns}
            WHERE {whereCondition}
            """;

            await using var insertCommand = new SqlCommand(insertSql, sqlConnectionWrapper.Connection, sqlConnectionWrapper.Transaction);

            if (hasOutputColumns)
            {
                using var insertReader = await insertCommand.ExecuteReaderAsync(cancellationToken);
                var index = 0;
                while (await insertReader.ReadAsync(cancellationToken))
                {
                    foreach (var keyProperty in efAutoGeneratedProperties)
                    {
                        var keyValue = insertReader[keyProperty.GetColumnName()];
                        keyProperty.PropertyInfo!.SetValue(entities[index], keyValue);
                    }

                    index++;
                }
            }
            else
            {
                await insertCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Bulk updates entities in the database using a temporary table for performance optimization.<br/>
    /// Match entities based on primary key or specified properties.
    /// </summary>
    public static async Task BulkUpdateAsync<TEntity>(
        this DbContext dbContext,
        TEntity[] entities,
        BulkUpdateOptions<TEntity>? options = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entities);

        if (entities.Length == 0)
        {
            return;
        }

        // Options preparation
        options ??= new();
        var optionsPropertiesToMatch = ToDictionary(options.PropertiesToMatch);
        var optionsPropertiesToUpdate = ToDictionary(options.PropertiesToUpdate);

        // EF preparation
        var efEntityType = dbContext.GetEfEntityType<TEntity>();
        var efKeyProperties = efEntityType.GetEfPrimaryKeyProperties();
        var efProperties = efEntityType.GetEfProperties();

        // Keep order of properties specified (e.g. to match indices)
        var efPropertiesToMatch = optionsPropertiesToMatch.Count > 0
            ? optionsPropertiesToMatch
                .Select(propertyToMatch => efProperties.First(efProperty => efProperty.Name == propertyToMatch.Key))
                .ToArray()
            : [.. efKeyProperties];

        var efPropertiesToUpdate = optionsPropertiesToUpdate.Count > 0
            ? efProperties.Where(property => optionsPropertiesToUpdate.ContainsKey(property.Name)).ToArray()
            : [.. efProperties];

        // Do not update PK properties
        efPropertiesToUpdate = [.. efPropertiesToUpdate
            .Where(property => !efKeyProperties
            .Any(autoGeneratedProperty => autoGeneratedProperty.PropertyInfo == property.PropertyInfo))
        ];

        if (efPropertiesToUpdate.Length == 0)
        {
            throw new InvalidOperationException($"The entity type '{typeof(TEntity).FullName}' does not have any properties to insert with current configuration.");
        }

        var propertiesForTempTable = efPropertiesToUpdate
            .Union(efPropertiesToMatch)
            .Distinct()
            .ToArray();

        // Execute
        var tableName = efEntityType.GetSchemaQualifiedTableName()!;
        using (var sourceTable = GetDataTable(propertiesForTempTable, entities))
        await using (var sqlConnectionWrapper = await SqlConnectionWrapper.EnsureConnectAsync(dbContext, cancellationToken))
        {
            // CREATE TABLE + INSERT TO #TempTable
            var tempTableName = await sqlConnectionWrapper.Transaction.CreateTempTableAsync(sourceTable, cancellationToken);

            // UPDATE TargetTable
            var setColumns = string.Join(", ", efPropertiesToUpdate.Select(property =>
            {
                var columnName = property.GetColumnName();
                return $"Target.[{columnName}] = Source.[{columnName}]";
            }));

            var innerJoinCondition = string.Join(" AND ", efPropertiesToMatch.Select(property =>
            {
                var columnName = property.GetColumnName();
                return $"Target.[{columnName}] = Source.[{columnName}]";
            }));

            var updateSql = $"""
            UPDATE Target
            SET {setColumns}
            FROM {tableName} AS Target
            INNER JOIN {tempTableName} AS Source
            ON {innerJoinCondition}
            """;

            using var updateCommand = new SqlCommand(updateSql, sqlConnectionWrapper.Connection, sqlConnectionWrapper.Transaction);
            await updateCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Bulk deletes entities from the database using a temporary table for performance optimization.<br/>
    /// Match entities based on primary key or specified properties.
    /// </summary>
    public static async Task BulkDeleteAsync<TEntity>(
        this DbContext dbContext,
        object?[][] valuesToMatch,
        BulkDeleteOptions<TEntity>? options = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(valuesToMatch);

        if (valuesToMatch.Length == 0)
        {
            return;
        }

        // Options preparation
        options ??= new();
        var optionsPropertiesToMatch = ToDictionary(options.PropertiesToMatch);

        // EF preparation
        var efEntityType = dbContext.GetEfEntityType<TEntity>();
        var efKeyProperties = efEntityType.GetEfPrimaryKeyProperties();
        var efProperties = efEntityType.GetEfProperties();

        // Keep order of properties specified (e.g. to match indices)
        var efPropertiesToMatch = optionsPropertiesToMatch.Count > 0
            ? optionsPropertiesToMatch
                .Select(propertyToMatch => efProperties.First(efProperty => efProperty.Name == propertyToMatch.Key))
                .ToArray()
            : [.. efKeyProperties];

        var tableName = efEntityType.GetSchemaQualifiedTableName()!;
        using (var sourceTable = GetDataTable(efPropertiesToMatch, valuesToMatch))
        await using (var sqlConnectionWrapper = await SqlConnectionWrapper.EnsureConnectAsync(dbContext, cancellationToken))
        {
            // CREATE TABLE + INSERT TO #TempTable
            var tempTableName = await sqlConnectionWrapper.Transaction.CreateTempTableAsync(sourceTable, cancellationToken);

            // DELETE FROM TargetTable
            var joinConditions = string.Join(" AND ", efPropertiesToMatch.Select(property =>
            {
                var columnName = property.GetColumnName();
                return $"(Target.[{columnName}] = Source.[{columnName}] OR (Target.[{columnName}] IS NULL AND Source.[{columnName}] IS NULL))";
            }));

            var deleteSql = $"""
            DELETE Target
            FROM {tableName} AS Target
            INNER JOIN {tempTableName} AS Source
            ON {joinConditions}
            """;

            using var deleteCommand = new SqlCommand(deleteSql, sqlConnectionWrapper.Connection, sqlConnectionWrapper.Transaction);
            var count = await deleteCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Bulk merges entities into the database using a temporary table for performance optimization.<br/>
    /// Match entities based on primary key or specified properties.
    /// </summary>
    public static async Task BulkMergeAsync<TEntity>(
        this DbContext dbContext,
        TEntity[] entities,
        BulkMergeOptions<TEntity>? options = null,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(entities);
        ArgumentOutOfRangeException.ThrowIfZero(entities.Length, nameof(entities));

        // Options preparation
        options ??= new();
        var optionsPropertiesToMatch = ToDictionary(options.PropertiesToMatch);
        var optionsPropertiesToInsert = ToDictionary(options.PropertiesToInsert);
        var optionsPropertiesToUpdate = ToDictionary(options.PropertiesToUpdate);

        // EF preparation
        var efEntityType = dbContext.GetEfEntityType<TEntity>();
        var efKeyProperties = efEntityType.GetEfPrimaryKeyProperties();
        var efProperties = efEntityType.GetEfProperties();
        var efAutoGeneratedProperties = efKeyProperties
            .Where(property => property.ValueGenerated != ValueGenerated.Never)
            .ToArray();

        // Keep order of properties specified (e.g. to match indices)
        var efPropertiesToMatch = optionsPropertiesToMatch.Count > 0
            ? optionsPropertiesToMatch
                .Select(propertyToMatch => efProperties.First(efProperty => efProperty.Name == propertyToMatch.Key))
                .ToArray()
            : [.. efKeyProperties];

        var efPropertiesToInsert = optionsPropertiesToInsert.Count > 0
            ? efProperties.Where(property => optionsPropertiesToInsert.ContainsKey(property.Name)).ToArray()
            : [.. efProperties];

        var efPropertiesToUpdate = optionsPropertiesToUpdate.Count > 0
            ? efProperties.Where(property => optionsPropertiesToUpdate.ContainsKey(property.Name)).ToArray()
            : [.. efProperties];

        // Do not insert auto-generated properties
        efPropertiesToInsert = [.. efPropertiesToInsert
            .Where(property => !efAutoGeneratedProperties.Any(efAutoGeneratedProperty => efAutoGeneratedProperty.PropertyInfo == property.PropertyInfo))
        ];

        // Do not update PK properties
        efPropertiesToUpdate = [.. efPropertiesToUpdate
            .Where(property => !efKeyProperties.Any(efKeyProperty => efKeyProperty.PropertyInfo == property.PropertyInfo))
        ];

        var hasInsert = efPropertiesToInsert.Length > 0;
        var hasUpdate = efPropertiesToUpdate.Length > 0;
        if (!hasInsert && !hasUpdate)
        {
            throw new InvalidOperationException($"No insert or update properties configured for '{typeof(TEntity).FullName}'.");
        }

        var propertiesForTempTable = efPropertiesToInsert
            .Union(efPropertiesToUpdate)
            .Union(efPropertiesToMatch)
            .Distinct()
            .ToArray();

        var hasOutputColumns = hasInsert && efAutoGeneratedProperties.Length > 0;
        var tableName = efEntityType.GetSchemaQualifiedTableName()!;
        using (var sourceTable = GetDataTable(propertiesForTempTable, entities, addIndexColumn: hasOutputColumns))
        await using (var sqlConnectionWrapper = await SqlConnectionWrapper.EnsureConnectAsync(dbContext, cancellationToken))
        {
            var tempTableName = await sqlConnectionWrapper.Transaction.CreateTempTableAsync(sourceTable, cancellationToken);
            var joinCondition = string.Join(" AND ", efPropertiesToMatch.Select(property =>
            {
                var columnName = property.GetColumnName();
                return $"Target.[{columnName}] = Source.[{columnName}]";
            }));

            var setClause = hasUpdate
                ? string.Join(", ", efPropertiesToUpdate.Select(property =>
                {
                    var columnName = property.GetColumnName();
                    return $"Target.[{columnName}] = Source.[{columnName}]";
                })) : string.Empty;

            var insertColumns = hasInsert
                ? string.Join(", ", efPropertiesToInsert.Select(property => $"[{property.GetColumnName()}]"))
                : string.Empty;

            var insertValues = hasInsert
                ? string.Join(", ", efPropertiesToInsert.Select(property => $"Source.[{property.GetColumnName()}]"))
                : string.Empty;

            var outputClause = string.Empty;
            if (hasOutputColumns)
            {
                var outputSelectColumns = new List<string>
                {
                    $"Source.[{IndexColumnName}]",
                    $"$action AS [{ActionColumnName}]"
                };

                outputSelectColumns.AddRange(efAutoGeneratedProperties.Select(property => $"INSERTED.[{property.GetColumnName()}]"));
                outputClause = $"OUTPUT {string.Join(", ", outputSelectColumns)}";
            }

            var whenMatchedClause = hasUpdate
                ? $"""
                WHEN MATCHED THEN 
                UPDATE SET {setClause}
                """
                : string.Empty;

            var whenNotMatchedClause = hasInsert
                ? $"""
                WHEN NOT MATCHED BY TARGET THEN
                INSERT ({insertColumns})
                VALUES ({insertValues})
                """ : string.Empty;

            var mergeSql = $"""
                MERGE INTO {tableName} AS Target
                USING {tempTableName} AS Source
                ON {joinCondition}
                {whenMatchedClause}
                {whenNotMatchedClause}
                {outputClause};
                """;

            await using var mergeCommand = new SqlCommand(mergeSql, sqlConnectionWrapper.Connection, sqlConnectionWrapper.Transaction);

            if (hasOutputColumns)
            {
                using var mergeReader = await mergeCommand.ExecuteReaderAsync(cancellationToken);
                while (await mergeReader.ReadAsync(cancellationToken))
                {
                    var action = mergeReader[ActionColumnName].ToString();
                    if (string.Equals(action, "INSERT", StringComparison.OrdinalIgnoreCase))
                    {
                        var insertedIndex = (int)mergeReader[IndexColumnName];
                        var entity = entities[insertedIndex];
                        foreach (var property in efAutoGeneratedProperties)
                        {
                            var value = mergeReader[property.GetColumnName()];
                            property.PropertyInfo!.SetValue(entity, value);
                        }
                    }
                }
            }
            else
            {
                await mergeCommand.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        dbContext.ChangeTracker.Clear();
    }

    private static IEntityType GetEfEntityType<TEntity>(this DbContext dbContext)
        where TEntity : class
        => dbContext.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"The entity type {typeof(TEntity)} is not found in '{dbContext.GetType()}'.");

    private static IEnumerable<IProperty> GetEfPrimaryKeyProperties(this IEntityType entityType)
        => entityType
            .FindPrimaryKey()?
            .Properties ?? throw new InvalidOperationException($"The entity type '{entityType.Name}' does not have a primary key defined.");

    private static IEnumerable<IProperty> GetEfProperties(this IEntityType entityType)
        => [.. entityType
            .GetProperties()
            .Where(property => !property.IsShadowProperty())
        ];

    private static DataTable GetDataTable<TEntity>(IProperty[] properties, TEntity[] entities, bool addIndexColumn = false)
    {
        var dataTable = new DataTable();
        foreach (var property in properties)
        {
            var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
            dataTable.Columns.Add(property.GetColumnName(), propertyType);
        }

        if (addIndexColumn)
        {
            dataTable.Columns.Add(IndexColumnName, typeof(int));
        }

        for (var i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            var row = dataTable.NewRow();

            foreach (var property in properties)
            {
                row[property.GetColumnName()] = property.PropertyInfo!.GetValue(entity) ?? DBNull.Value;
            }

            if (addIndexColumn)
            {
                row[IndexColumnName] = i;
            }

            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private static DataTable GetDataTable(IProperty[] properties, object?[][] values)
    {
        var dataTable = new DataTable();
        foreach (var property in properties)
        {
            var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
            dataTable.Columns.Add(property.GetColumnName(), propertyType);
        }

        for (var rowIndex = 0; rowIndex < values.Length; rowIndex++)
        {
            var rowValues = values[rowIndex];
            if (rowValues.Length != properties.Length)
            {
                throw new InvalidOperationException($"The number of values in row {rowIndex} does not match the number of properties ({properties.Length}).");
            }

            for (var valueIndex = 0; valueIndex < properties.Length; valueIndex++)
            {
                rowValues[valueIndex] ??= DBNull.Value;
            }

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    private static IReadOnlyDictionary<string, PropertyInfo> ToDictionary<TEntity, TPropertyType>(
        IEnumerable<Expression<Func<TEntity, TPropertyType>>> propertySelectors
    ) where TEntity : class
        => propertySelectors
            .Select(expression => expression.GetProperty())
            .ToDictionary(property => property.Name, StringComparer.OrdinalIgnoreCase);
}
