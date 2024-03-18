namespace GoTravel.API.Domain.Exceptions;

public class NotAcceptingFollowersException: Exception
{
    public NotAcceptingFollowersException() : base() {}
    
    public NotAcceptingFollowersException(string userId, string followId): base($"{followId} is not accepting followers") {}
}