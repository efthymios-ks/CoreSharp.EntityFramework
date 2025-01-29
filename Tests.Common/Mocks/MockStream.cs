namespace Tests.Common.Mocks;

public sealed class MockStream(Stream innerStream) : Stream
{
    private readonly Stream _innerStream = innerStream;

    public MockStream(byte[] buffer)
        : this(new MemoryStream(buffer))
    {
    }

    public MockStream()
        : this(new MemoryStream())
    {
    }

    public bool? CanReadOverride { get; set; }

    public override bool CanRead
        => CanReadOverride ?? _innerStream.CanRead;

    public override bool CanWrite
        => _innerStream.CanWrite;

    public override bool CanSeek
        => _innerStream.CanSeek;

    public override long Length
        => _innerStream.Length;

    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public override void Write(byte[] buffer, int offset, int count)
        => _innerStream.Write(buffer, offset, count);

    public override int Read(byte[] buffer, int offset, int count)
        => _innerStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin)
        => _innerStream.Seek(offset, origin);

    public override void SetLength(long value)
        => _innerStream.SetLength(value);

    public override void Flush()
        => _innerStream.Flush();
}
