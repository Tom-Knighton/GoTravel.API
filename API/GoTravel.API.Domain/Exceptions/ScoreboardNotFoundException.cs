namespace GoTravel.API.Domain.Exceptions;

public class ScoreboardNotFoundException(string id) : Exception($"Could not find scoreboard with identifier '{id}'");