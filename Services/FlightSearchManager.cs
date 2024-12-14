using CheapFlights.DataTransferObjects;
using CheapFlights.Interfaces;
using CheapFlights.Model;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;

namespace CheapFlights.Services
{
    public class FlightSearchManager : IFlightSearchManager
    {
        private readonly FlightCacheService _cacheService;
        private readonly IFlightOfferApiService _flightOfferService;

        public FlightSearchManager(FlightCacheService cacheService, IFlightOfferApiService flightOfferService)
        {
            _cacheService = cacheService;
            _flightOfferService = flightOfferService;
        }
        
        public async Task<List<FlightOfferDto>> Search(SearchParameters searchParams)
        {
            await ValidateSearchParameters(searchParams);
            var results = await GetFlightOffers(searchParams);
            return results;
        }

        private async Task ValidateSearchParameters(SearchParameters searchParams)
        {
            var validationContext = new ValidationContext(searchParams);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(searchParams, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessage = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"{errorMessage}");
            }
        }

        private async Task<List<FlightOfferDto>> GetFlightOffers(SearchParameters searchParams)
        {
            string cacheKey = _cacheService.GenerateCacheKey(searchParams);

            if (_cacheService.TryGetValue(cacheKey, out List<FlightOfferDto> cachedResults))
                return cachedResults;

            var results = await _flightOfferService.SearchFlightOffers(searchParams);

            _cacheService.CacheFlightOffers(cacheKey, results);

            return results;
        }
    }
}
