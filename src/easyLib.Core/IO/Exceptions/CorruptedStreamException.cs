using easyLib.Exceptions;

namespace easyLib.IO.Exceptions;

public class CorruptedStreamException : RuntimeException
{
    public CorruptedStreamException(Exception? innerException = null) :
        base(innerException)
    { }
}
