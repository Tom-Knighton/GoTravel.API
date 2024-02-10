using NetTopologySuite.Geometries;

namespace GoTravel.API.Domain.Models.DTOs;

public class JourneyDto
{
    public DateTime BeginJourneyAt { get; set; }
    public DateTime EndJourneyAt { get; set; }
    public int TotalDuration { get; set; }
    
    public ICollection<JourneyLegDto> JourneyLegs { get; set; }
}

public class JourneyLegDto
{
    public DateTime BeginLegAt { get; set; }
    public DateTime EndLegAt { get; set; }
    public int LegDuration { get; set; }
    public string? StartAtName { get; set; }
    public string? EndAtName { get; set; }
    
    public JourneyLegStopPointDto? StartAtStop { get; set; }
    public JourneyLegStopPointDto? EndAtStop { get; set; }
    
    public JourneyLegDetailsDto LegDetails { get; set; }
}

public class JourneyLegDetailsDto
{
    public string Summary { get; set; }
    public string? DetailedSummary { get; set; }
    
    public ICollection<JourneyLegStepDto> LegSteps { get; set; }
    
    public LineString? LineString { get; set; }
    public LineModeDto? LineMode { get; set; }
    public ICollection<JourneyLegStopPointDto> StopPoints { get; set; }
}

public class JourneyLegStepDto
{
    public string Summary { get; set; }
    public string? Direction { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int DistanceOfStep { get; set; }
}

public class JourneyLegStopPointDto
{
    public string StopPointId { get; set; }
    public string StopPointName { get; set; }
    public Point StopCoordinate { get; set; }
}