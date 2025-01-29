using System.Diagnostics;
using System.Reflection;

namespace Tests.Common.Mocks;

public sealed class MockStackFrame : StackFrame
{
    public static readonly FieldInfo _methodField = null!;
    public static readonly FieldInfo _fileNameField = null!;
    public static readonly FieldInfo _lineNumberField = null!;

    static MockStackFrame()
    {
        var fields = typeof(StackFrame)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

        _methodField = fields.Single(field => field.Name is "_method");
        _fileNameField = fields.Single(field => field.Name is "_fileName");
        _lineNumberField = fields.Single(field => field.Name is "_lineNumber");
    }

    public MockStackFrame()
    {
    }

    public MockStackFrame(bool needFileInfo)
        : base(needFileInfo)
    {
    }

    public MockStackFrame(int skipFrames)
        : base(skipFrames)
    {
    }

    public MockStackFrame(int skipFrames, bool needFileInfo)
        : base(skipFrames, needFileInfo)
    {
    }

    public MockStackFrame(string? fileName, int lineNumber)
        : base(fileName, lineNumber)
    {
    }

    public MockStackFrame(string? fileName, int lineNumber, int colNumber)
        : base(fileName, lineNumber, colNumber)
    {
    }

    public void SetMethodBase(MethodBase? methodBase)
        => _methodField.SetValue(this, methodBase);

    public void SetFileName(string? fileName)
        => _fileNameField.SetValue(this, fileName);

    public void SetLineNumber(int lineNumber)
        => _lineNumberField.SetValue(this, lineNumber);
}
