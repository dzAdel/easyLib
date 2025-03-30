namespace easyLib.IO;

public interface IRandomAccessReader : IBinaryReader
{
    long Position { get; set; }
    long Length { get; }
    bool IsExhausted { get; }
}
