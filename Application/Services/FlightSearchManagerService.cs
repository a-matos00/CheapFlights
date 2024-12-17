using CheapFlights.Application.DTOs;
using CheapFlights.Application.Interfaces;
using CheapFlights.Domain.Entities;
using CheapFlights.Infrastructure.Interfaces;
using CheapFlights.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;

namespace CheapFlights.Application.Services
{
    public class FlightSearchManagerService : IFlightSearchManagerService
    {
        private readonly FlightCacheService _cacheService;
        private readonly IFlightOfferApiService _flightOfferApiService;

        public FlightSearchManagerService(FlightCacheService cacheService, IFlightOfferApiService flightOfferService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _flightOfferApiService = flightOfferService ?? throw new ArgumentNullException(nameof(flightOfferService));
        }

        public async Task<List<FlightOfferDto>> Search(SearchParameters searchParams)
        {
            if (searchParams == null)
                throw new ArgumentNullException(nameof(searchParams));

            ValidateSearchParameters(searchParams);
            var results = await GetFlightOffers(searchParams);

            return results ?? new List<FlightOfferDto>();
        }

        private void ValidateSearchParameters(SearchParameters searchParams)
        {
            var validationContext = new ValidationContext(searchParams);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(searchParams, validationContext, validationResults, validateAllProperties: true))
            {
                var errorMessage = string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException(errorMessage);
            }
        }

        private async Task<List<FlightOfferDto>> GetFlightOffers(SearchParameters searchParams)
        {
            var cacheKey = _cacheService.GenerateCacheKey(searchParams);

            if (_cacheService.TryGetValue(cacheKey, out List<FlightOfferDto> cachedResults))
                return cachedResults;

            var results = await _flightOfferApiService.SearchFlightOffers(searchParams);

            _cacheService.CacheFlightOffers(cacheKey, results);

            return results;
        }
    }
}
