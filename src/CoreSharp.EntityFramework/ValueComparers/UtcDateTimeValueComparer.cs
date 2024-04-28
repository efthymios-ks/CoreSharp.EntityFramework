using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq.Expressions;

namespace CoreSharp.EntityFramework.ValueComparers;

public sealed class UtcDateTimeValueComparer : ValueComparer<DateTime?>
{
    private static UtcDateTimeValueComparer _instance;

    public UtcDateTimeValueComparer()
        : base(
            equalsExpression: (left, right) => object.Equals(
                left == null ? null : left.Value.ToUniversalTime(),
                right == null ? null : right.Value.ToUniversalTime()
            ),
            hashCodeExpression: value => value.GetHashCode(),
            snapshotExpression: value => value) // DateTime is a struct, so an assignment will copy the value. 
    {
    }

    public static UtcDateTimeValueComparer Instance
        => _instance ??= new();
}