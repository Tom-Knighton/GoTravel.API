namespace GoTravel.API.Domain.Models.Lib;

public enum AddPointsReasonType {
    Travel,
    Correction,
    Punishment
}

public class AddPointsMessage
{
    public string UserId { get; set; }
    public double Points { get; set; }
    public string Message { get; set; }
    public AddPointsReasonType ReasonType { get; set; }
}