namespace GoTravel.API.Domain.Exceptions;

public class WinNotFoundException(string winId) : Exception($"Scoreboard Win '{winId}' was not found");