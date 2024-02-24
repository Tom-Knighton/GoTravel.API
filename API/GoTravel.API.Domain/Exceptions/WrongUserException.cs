namespace GoTravel.API.Domain.Exceptions;

public class WrongUserException(string identifyingAgainst) : Exception($"User '{identifyingAgainst}' does not have access to operate on other users");