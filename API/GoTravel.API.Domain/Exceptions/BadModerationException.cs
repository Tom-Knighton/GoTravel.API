namespace GoTravel.API.Domain.Exceptions;

public class BadModerationException: Exception
{
    public BadModerationException(): base() {}

    public BadModerationException(string text, string userId, string entityId) : base($"Bad moderation for crowdsource text: {text}, from: {userId}, for: {entityId}") {}
}