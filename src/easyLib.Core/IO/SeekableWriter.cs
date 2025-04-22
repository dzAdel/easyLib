namespace easyLib.IO;

public interface ISeekableWriter : IBinaryWriter
{
    long Position { get; set; }
    long Length { get; }
}
