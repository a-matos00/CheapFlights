using CheapFlights.Application.DTOs;
using CheapFlights.Domain.Entities;

namespace CheapFlights.Infrastructure.Interfaces
{
    public interface IFlightOfferCacheService
    {
        public string GenerateCacheKey(SearchParameters searchParams);
        public void CacheFlightOffers(string key, List<FlightOfferDto> results);
    }
}
