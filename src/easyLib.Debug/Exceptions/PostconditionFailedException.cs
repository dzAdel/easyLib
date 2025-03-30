namespace easyLib.Debug.Exceptions;

public sealed class PostconditionFailedException : Exception
{
    public PostconditionFailedException(string message) :
        base(message)
    { }
}
