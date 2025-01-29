using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Tests.Common.Mocks;

public sealed class MockLogger<TCategory> : ILogger<TCategory>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ICollection<MockLoggerEntry> _logs = [];

    public IDictionary<LogLevel, bool> LogLevelEnabledOverrides { get; } = new Dictionary<LogLevel, bool>();

    public IEnumerable<MockLoggerEntry> Logs
        => [.. _logs];

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var stateAsJson = JsonSerializer.Serialize(state);
        var stateAsDictionary = JsonSerializer
            .Deserialize<IDictionary<string, JsonElement>[]>(stateAsJson)!
            .Aggregate(
                new Dictionary<string, object?>(),
                (accumulatorDictionary, currentDictionary) =>
                {
                    var key = currentDictionary["Key"].GetString()!;
                    var valueAsJsonElemnent = currentDictionary["Value"];

                    var value = valueAsJsonElemnent.ValueKind switch
                    {
                        JsonValueKind.String => valueAsJsonElemnent.GetString(),
                        JsonValueKind.Number => GetNumber(valueAsJsonElemnent),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => valueAsJsonElemnent.GetRawText()
                    };

                    accumulatorDictionary[key] = value;
                    return accumulatorDictionary;
                });

        _logs.Add(new()
        {
            Level = logLevel,
            Values = stateAsDictionary,
        });

        static object? GetNumber(JsonElement jsonElement)
        {
            if (jsonElement.TryGetInt64(out var intValue))
            {
                return intValue;
            }

            if (jsonElement.TryGetDecimal(out var decimalValue))
            {
                return decimalValue;
            }

            return null;
        }
    }

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => null!;

    public bool IsEnabled(LogLevel logLevel)
        => LogLevelEnabledOverrides.TryGetValue(logLevel, out var isEnabled) && isEnabled;
}
