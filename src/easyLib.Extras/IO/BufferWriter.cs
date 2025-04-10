using easyLib.IO;

namespace easyLib.Extras.IO;

public interface IBufferWriter : IRandomAccessWriter
{
    int Capacity { get; }
}
