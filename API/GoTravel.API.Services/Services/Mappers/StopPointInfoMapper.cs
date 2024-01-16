using GoTravel.API.Domain.Models.Database;
using GoTravel.API.Domain.Models.DTOs;
using GoTravel.API.Domain.Services.Mappers;
using GoTravel.Standard.Models;

namespace GoTravel.API.Services.Services.Mappers;

public class StopPointInfoMapper: IMapper<ICollection<GTStopPointInfoValue>, StopPointInformationDto>
{
    private const string YesIndicator = "Y";
    
    public StopPointInformationDto Map(ICollection<GTStopPointInfoValue> source)
    {
        var dto = new StopPointInformationDto
        {
            HasWifi = source.GetByKey(StopPointInfoKey.WiFi)?.Value == YesIndicator,
        };

        var toiletInfo = new List<StopPointToiletInformationDto>();
        if (source.ContainsKey(StopPointInfoKey.ToiletsMen))
        {
            var mensToilet = new StopPointToiletInformationDto
            {
                Type = "Men",
                Accessible = source.GetByKey(StopPointInfoKey.ToiletsMenAccessible)?.Value == YesIndicator,
                Free = source.GetByKey(StopPointInfoKey.ToiletsMenFree)?.Value == YesIndicator,
                HasBabyGate = source.GetByKey(StopPointInfoKey.ToiletsMenBaby)?.Value == YesIndicator,
                Info = source.GetByKey(StopPointInfoKey.ToiletsMenNote)?.Value
            };
            toiletInfo.Add(mensToilet);
        }
        
        if (source.ContainsKey(StopPointInfoKey.ToiletsWomen))
        {
            var womensToilet = new StopPointToiletInformationDto
            {
                Type = "Women",
                Accessible = source.GetByKey(StopPointInfoKey.ToiletsWomenAccessible)?.Value == YesIndicator,
                Free = source.GetByKey(StopPointInfoKey.ToiletsWomenFree)?.Value == YesIndicator,
                HasBabyGate = source.GetByKey(StopPointInfoKey.ToiletsWomenBaby)?.Value == YesIndicator,
                Info = source.GetByKey(StopPointInfoKey.ToiletsWomenNote)?.Value
            };
            toiletInfo.Add(womensToilet);
        }
        
        if (source.ContainsKey(StopPointInfoKey.ToiletsUnisex))
        {
            var unisexToilet = new StopPointToiletInformationDto
            {
                Type = "Unisex",
                Accessible = source.GetByKey(StopPointInfoKey.ToiletsUnisexAccessible)?.Value == YesIndicator,
                Free = source.GetByKey(StopPointInfoKey.ToiletsUnisexFree)?.Value == YesIndicator,
                HasBabyGate = source.GetByKey(StopPointInfoKey.ToiletsUnisexBaby)?.Value == YesIndicator,
                Info = source.GetByKey(StopPointInfoKey.ToiletsUnisexNote)?.Value
            };
            toiletInfo.Add(unisexToilet);
        }
        
        dto.ToiletsInfo = toiletInfo;
        dto.AccessibleInfo = new StopPointAccessibleInfoDto
        {
            ViaLift = source.GetByKey(StopPointInfoKey.AccessibilityViaLift)?.Value == YesIndicator,
            Info = source.GetByKey(StopPointInfoKey.AccessibilityViaLift)?.Value
        };

        dto.AddressInfo = new StopPointAddressInfoDto
        {
            Address = source.GetByKey(StopPointInfoKey.Address)?.Value,
            Phone = source.GetByKey(StopPointInfoKey.Phone)?.Value
        };

        return dto;
    }
}

public static class InfoCollectionExtensions
{

    public static bool ContainsKey(this IEnumerable<GTStopPointInfoValue> values, StopPointInfoKey key)
    {
        return values.Any(k => k.KeyId == key);
    }
    
    public static GTStopPointInfoValue? GetByKey(this IEnumerable<GTStopPointInfoValue> values, StopPointInfoKey key)
    {
        return values.FirstOrDefault(k => k.KeyId == key);
    }
}