namespace easyLib.IO;

public interface ISeekableReader : IBinaryReader
{
    long Position { get; set; }
    long Length { get; }
    bool IsExhausted { get; }
}
