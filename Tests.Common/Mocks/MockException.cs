using System.Reflection;

namespace Tests.Common.Mocks;

public class MockException : Exception
{
    private static readonly FieldInfo _stackTraceField = typeof(Exception)
        .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic)!;

    public MockException(string? message)
        : base(message)
    {
    }

    public MockException()
    {
    }

    public void SetStackTrace(string? stackTrace)
        => _stackTraceField.SetValue(this, stackTrace);
}
