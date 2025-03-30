namespace easyLib.Debug.Exceptions;

public sealed class AssertionFailedException : Exception
{
    public AssertionFailedException(string msg) :
       base(msg)
    { }
}
