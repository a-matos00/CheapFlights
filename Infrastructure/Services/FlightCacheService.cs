using Microsoft.Extensions.Caching.Memory;
using System.Text;
using CheapFlights.Infrastructure.Interfaces;
using CheapFlights.Domain.Entities;
using CheapFlights.Application.DTOs;

namespace CheapFlights.Infrastructure.Services
{
    public class FlightCacheService : IFlightOfferCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpirationMinutes;
        private const string CACHE_KEY_PREFIX = "flight_search_";
        private const string NO_RETURN = "no_return";
        private const int CACHE_EXPIRATION_MINUTES = 5;

        public FlightCacheService(IMemoryCache cache, TimeSpan? cacheExpirationMinutes = null)
        {
            _cache = cache;
            _cacheExpirationMinutes = cacheExpirationMinutes ?? TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES);
        }

        /// <summary>
        /// Generates a unique cache key based on search parameters
        /// </summary>
        /// <param name="searchParams">The search parameters to generate the key from</param>
        /// <returns>A unique string key for caching</returns>
        /// <exception cref="ArgumentNullException">Thrown when searchParams is null</exception>
        public string GenerateCacheKey(SearchParameters searchParams)
        {
            if (searchParams == null)
            {
                throw new ArgumentNullException(nameof(searchParams));
            }

            return new StringBuilder(CACHE_KEY_PREFIX)
                .Append(searchParams.OriginLocationCode).Append('_')
                .Append(searchParams.DestinationLocationCode).Append('_')
                .Append(searchParams.DepartureDate.ToString("yyyy-MM-dd")).Append('_')
                .Append(searchParams.ReturnDate?.ToString("yyyy-MM-dd") ?? NO_RETURN)
                .Append('_')
                .Append(searchParams.Passengers.AdultsCount).Append('_')
                .Append(searchParams.Passengers.InfantCount).Append('_')
                .Append(searchParams.Passengers.ChildCount).Append('_')
                .Append(searchParams.Currency).Append('_')
                .Append(searchParams.IsOneWay)
                .ToString();
        }

        public void CacheFlightOffers(string key, List<FlightOfferDto> results)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (results == null)
                throw new ArgumentNullException(nameof(results));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheExpirationMinutes);

            _cache.Set(key, results, cacheEntryOptions);
        }
        public bool TryGetValue(string key, out List<FlightOfferDto> cachedResults)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.TryGetValue(key, out cachedResults);
        }

        internal void Clear()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }

        internal void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
