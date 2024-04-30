using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using Tests.Internal.Database.Models;

namespace Tests.Extensions;

[TestFixture]
public sealed class PropertyBuilderExtensionsTests : DummyDbContextTestsBase
{
    [Test]
    public void HasUtcConversion_WhenCalled_ShouldAddUtcDateTimeValueConverterToProperty()
    {
        var modelBuilder = DbContext.ModelBuilder;
        var dummyEntityType = modelBuilder.Model.FindEntityType(typeof(DummyEntity));
        var builderProperty = dummyEntityType
            .GetType()
            .GetProperty("Microsoft.EntityFrameworkCore.Metadata.IConventionAnnotatable.Builder", BindingFlags.NonPublic | BindingFlags.Instance);
        var entityTypeBuilder = (InternalEntityTypeBuilder)builderProperty.GetValue(dummyEntityType);

        var entityTypeBuilder2 = modelBuilder.Entity(dummyEntityType.Name);
    }
}