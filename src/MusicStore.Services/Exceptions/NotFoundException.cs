namespace MusicStore.Services.Exceptions;

public sealed class NotFoundException(string message) : Exception(message);

/*public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}*/
