namespace GoTravel.API.Domain.Exceptions;

public class UserNotFoundException(string identifier) : Exception($"Could not find user with identifier '{identifier}'");