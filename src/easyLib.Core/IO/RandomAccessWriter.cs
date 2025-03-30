namespace easyLib.IO;

public interface IRandomAccessWriter : IBinaryWriter
{
    long Position { get; set; }
    long Length { get; }
}
