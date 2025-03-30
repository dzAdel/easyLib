namespace easyLib.IO;

public sealed class RandomAccessStreamWriter : BinaryStreamWriter, IRandomAccessWriter
{
    public RandomAccessStreamWriter(Stream stream, ByteOrder endianness = ByteOrder.System) :
        base(stream, endianness)
    {
        require(stream != null);
        require(stream.CanWrite);
        require(stream.CanSeek);
        require(Enum.IsDefined(endianness));
    }

    public long Position
    {
        get => OutputStream.Position;
        set
        {
            require(value >= 0);
            require(value <= Length);

            OutputStream.Position = value;
        }
    }

    public long Length => OutputStream.Length;
}
