namespace GoTravel.API.Domain.Models.DTOs;

public class StopPointInformationDto
{
    public bool HasWifi { get; set; }
    public ICollection<StopPointToiletInformationDto> ToiletsInfo { get; set; }
    public StopPointAccessibleInfoDto? AccessibleInfo { get; set; }
    public StopPointAddressInfoDto? AddressInfo { get; set; }
}

public class StopPointToiletInformationDto
{
    public string Type { get; set; }
    public bool Free { get; set; }
    public bool Accessible { get; set; }
    public bool HasBabyGate { get; set; }
    public string? Info { get; set; }
}

public class StopPointAccessibleInfoDto
{
    public bool ViaLift { get; set; }
    public string? Info { get; set; }
}

public class StopPointAddressInfoDto
{
    public string? Phone { get; set; }
    public string? Address { get; set; }
}