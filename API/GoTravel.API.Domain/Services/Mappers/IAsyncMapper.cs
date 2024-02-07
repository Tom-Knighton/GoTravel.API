namespace GoTravel.API.Domain.Services.Mappers;

public interface IAsyncMapper<in TSrc, TDest> where TSrc: class where TDest: class
{
    Task<TDest> MapAsync(TSrc source);   
}