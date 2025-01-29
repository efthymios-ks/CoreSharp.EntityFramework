using Microsoft.Extensions.Logging;

namespace Tests.Common.Mocks;

public sealed class MockLoggerEntry
{
    public LogLevel Level { get; init; }

    public IReadOnlyDictionary<string, object?> Values { get; init; } = null!;
}
