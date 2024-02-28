namespace GoTravel.API.Domain.Exceptions;

public class NoRelationshipException: Exception
{
    public NoRelationshipException(): base() {}

    public NoRelationshipException(string userId, string followsId) : base($"No relationship exists between users: {userId}, {followsId}") {}
}