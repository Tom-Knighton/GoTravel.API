using System.Text;
using System.Text.Json;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Singletons;
using GoTravel.Standard.Models.Journeys;

namespace GoTravel.API.Services.Services;

public class JourneyService: IJourneyService
{
    private ILineModeService _lineModeService;
    private HttpClient _api;

    public JourneyService(ILineModeService lineModeService, IHttpClientFactory httpFactory)
    {
        _lineModeService = lineModeService;
        _api = httpFactory.CreateClient("GTCON");
    }
    
    
    public async Task<JourneyOptionsResultDto> GetJourneys(JourneyRequest journeyRequest, CancellationToken ct = default)
    {
        try
        {
            var body = JsonSerializer.Serialize(journeyRequest);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "/Journey/Options")
            {
                Content = content
            };

            var response = await _api.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(ct);
            var journeys = await JsonSerializer.DeserializeAsync<ICollection<Journey>>(stream, JsonSingleton.Options, cancellationToken: ct) ?? new List<Journey>();

            var lineIds = journeys
                .SelectMany(j => j.JourneyLegs?.SelectMany(l => l.LegDetails.LineIds) ?? Array.Empty<string>())
                .ToList();

            var lineModes = await _lineModeService.ListFromLineIdsAsync(lineIds, ct);
            
            var dto = new JourneyOptionsResultDto
            {
                JourneyOptions = journeys,
                LineModes = lineModes.ToList()
            };

            return dto;
        }
        catch (Exception ex)
        {
            //TODO: Log
            return new JourneyOptionsResultDto();
        }
    }
}