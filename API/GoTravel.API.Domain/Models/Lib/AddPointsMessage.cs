namespace GoTravel.API.Domain.Models.Lib;

public class AddPointsMessage
{
    public string UserId { get; set; }
    public double Points { get; set; }
    public string Message { get; set; }
}