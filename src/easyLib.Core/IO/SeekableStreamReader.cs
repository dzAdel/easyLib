namespace easyLib.IO;

public sealed class SeekableStreamReader : BinaryStreamReader, ISeekableReader
{
    public SeekableStreamReader(Stream stream, ByteOrder endianness = ByteOrder.System) :
        base(stream, endianness)
    {
        require(stream != null);
        require(stream.CanRead);
        require(stream.CanSeek);
        require(Enum.IsDefined(endianness));
    }

    public long Position
    {
        get => InputStream.Position;
        set
        {
            require(value >= 0);
            require(value <= Length);

            InputStream.Position = value;
        }
    }

    public long Length => InputStream.Length;
    public bool IsExhausted => Position >= Length;
}
