namespace easyLib.Exceptions;

public class RuntimeException : Exception
{
    public RuntimeException(Exception? innerException = null) :
       base(null, innerException)
    { }
}
