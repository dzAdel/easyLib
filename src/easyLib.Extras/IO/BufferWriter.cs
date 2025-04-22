using easyLib.IO;

namespace easyLib.Extras.IO;

public interface IBufferWriter : ISeekableWriter
{
    int Capacity { get; }
}
