namespace GoTravel.API.Domain.Exceptions;

public class NoCrowdsourceException: Exception
{
    public NoCrowdsourceException(): base() {}

    public NoCrowdsourceException(string crowdsourceId) : base($"No crowdsource found with id {crowdsourceId}") {}

}