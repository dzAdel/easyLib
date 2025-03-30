namespace easyLib.IO;

public interface IBinaryWriter : IWriter
{
    ByteOrder ByteOrder { get; }
    void WriteBytes(ReadOnlySpan<byte> bytes);
    void WriteSBytes(ReadOnlySpan<sbyte> sbytes);
    void WriteBools(ReadOnlySpan<bool> bools);
    void WriteChars(ReadOnlySpan<char> chars);
    void WriteShorts(ReadOnlySpan<short> shorts);
    void WriteUShorts(ReadOnlySpan<ushort> ushorts);
    void WriteInts(ReadOnlySpan<int> ints);
    void WriteUInts(ReadOnlySpan<uint> uints);
    void WriteLongs(ReadOnlySpan<long> longs);
    void WriteULongs(ReadOnlySpan<ulong> ulongs);
    void WriteFloats(ReadOnlySpan<float> floats);
    void WriteDoubles(ReadOnlySpan<double> doubles);
    void WriteDecimals(ReadOnlySpan<decimal> decimals);
}
