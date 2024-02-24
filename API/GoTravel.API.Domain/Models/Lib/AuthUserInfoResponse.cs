namespace GoTravel.API.Domain.Models.Lib;

public class AuthUserInfoResponse
{
    public string sub { get; set; }
    public string nickname { get; set;  }
    public string name { get; set; }
    public string picture { get; set; }
    public DateTime? updated_at { get; set; }
}