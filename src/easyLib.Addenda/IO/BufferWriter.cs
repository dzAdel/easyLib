using easyLib.IO;

namespace easyLib.IO;

public interface IBufferWriter : ISeekableWriter
{
    int Capacity { get; }
}
