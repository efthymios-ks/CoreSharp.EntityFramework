using System.Data;

namespace CoreSharp.EntityFramework.Tests.Internal;

internal static class AssertUtils
{
    public static void Equivalent(DataTable expected, DataTable actual)
    {
        Assert.True(
            expected.Columns.Count == actual.Columns.Count,
            $"Column count does not match: " +
            $"Expected {expected.Columns.Count}, " +
            $"Actual {actual.Columns.Count}"
        );
        for (var columnindex = 0; columnindex < expected.Columns.Count; columnindex++)
        {
            Assert.True(
                expected.Columns[columnindex].ColumnName == actual.Columns[columnindex].ColumnName,
                $"Column names do not match at index {columnindex}: " +
                $"Expected '{expected.Columns[columnindex].ColumnName}', " +
                $"Actual '{actual.Columns[columnindex].ColumnName}'"
            );

            Assert.True(
                expected.Columns[columnindex].DataType == actual.Columns[columnindex].DataType,
                $"Data types do not match at index {columnindex}: " +
                $"Expected '{expected.Columns[columnindex].DataType}', " +
                $"Actual '{actual.Columns[columnindex].DataType}'"
            );
        }

        Assert.True(
            expected.Rows.Count == actual.Rows.Count,
            $"Row count does not match: " +
            $"Expected {expected.Rows.Count}, " +
            $"Actual {actual.Rows.Count}"
        );
        for (var rowIndex = 0; rowIndex < expected.Rows.Count; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < expected.Columns.Count; columnIndex++)
            {
                Assert.True(
                    Equals(expected.Rows[rowIndex][columnIndex], actual.Rows[rowIndex][columnIndex]),
                    $"Values do not match at row {rowIndex}, column {columnIndex}: " +
                    $"Expected '{expected.Rows[rowIndex][columnIndex]}', " +
                    $"Actual '{actual.Rows[rowIndex][columnIndex]}'"
                );
            }
        }
    }
}
