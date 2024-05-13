namespace Kolos2.Exceptions;

[Serializable]
public class WrongAgeException : Exception
{
    public WrongAgeException ()
    {}

    public WrongAgeException (string message) 
        : base(message)
    {}

    public WrongAgeException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}