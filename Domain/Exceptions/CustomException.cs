namespace Domain.Exceptions;

public interface ICustomException;

public class CustomException : Exception, ICustomException
{
    public CustomException() { }

    public CustomException(string message)
        : base(message)
    {

    }
    public CustomException(string message, Exception inner)
        : base(message, inner)
    {

    }
}
