namespace easyLib.IO;

public interface IBufferWriter : IRandomAccessWriter
{
    int Capacity { get; }
}
