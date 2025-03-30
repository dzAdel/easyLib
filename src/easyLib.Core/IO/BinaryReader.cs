namespace easyLib.IO;

public interface IBinaryReader : IReader
{
    ByteOrder ByteOrder { get; }
    IEnumerable<byte> ReadBytes();
    void ReadBytes(Span<byte> bytes);
    void ReadSBytes(Span<sbyte> sbytes);
    void ReadBools(Span<bool> bools);
    void ReadChars(Span<char> chars);
    void ReadShorts(Span<short> shorts);
    void ReadUShorts(Span<ushort> ushorts);
    void ReadInts(Span<int> ints);
    void ReadUInts(Span<uint> uints);
    void ReadLongs(Span<long> longs);
    void ReadULongs(Span<ulong> ulongs);
    void ReadFloats(Span<float> floats);
    void ReadDoubles(Span<double> doubles);
    void ReadDecimals(Span<decimal> decimals);
}
