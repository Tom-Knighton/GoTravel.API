namespace GoTravel.API.Domain.Exceptions;

public class NoStopPointException: Exception
{
    public NoStopPointException(): base() {}
    public NoStopPointException(string stopPointId) : base($"No stop point exists '{stopPointId}'") {}
}