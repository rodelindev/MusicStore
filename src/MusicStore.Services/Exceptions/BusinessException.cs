namespace MusicStore.Services.Exceptions;

public sealed class BusinessException(string message) : Exception(message);

/*public sealed class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
}*/
