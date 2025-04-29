using easyLib.IO;

namespace easyLib.IO;

public interface IReadOnlyBuffer : IReadOnlyList<byte>
{
    bool IsEmpty { get; }
    IBufferReader GetReader(int offset = 0, ByteOrder endianness = ByteOrder.System);
    IBufferReader GetReader(int offset, int len, ByteOrder endianness);
    ReadOnlySpan<byte> AsReadOnlySpan();
    ReadOnlySpan<byte> AsReadOnlySpan(int count, int ndxStart = 0);
    void CopyTo(Stream dest, int count, int ndxFrom = 0);
    void CopyTo(Span<byte> dest, int ndxFrom = 0);
}
