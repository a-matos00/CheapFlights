using CheapFlights.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using CheapFlights.DataTransferObjects;

namespace CheapFlights.Services
{
    public class FlightCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpirationMinutes;
        private const string CACHE_KEY_PREFIX = "flight_search_";
        private const string NO_RETURN = "no_return";

        public FlightCacheService(IMemoryCache cache, TimeSpan? cacheExpirationMinutes = null)
        {
            _cache = cache;
            _cacheExpirationMinutes = cacheExpirationMinutes ?? TimeSpan.FromMinutes(30); // Default to 30 minutes if not specified
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

        public bool TryGetValue(string key, out List<FlightOfferDto> cachedResults)
        {
            return _cache.TryGetValue(key, out cachedResults);
        }

        public void Set(string key, List<FlightOfferDto> results, MemoryCacheEntryOptions options)
        {
            _cache.Set(key, results, options);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
            }
        }

        public void CacheFlightOffers(string key, List<FlightOfferDto> results)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(_cacheExpirationMinutes);

            Set(key, results, cacheEntryOptions);
        }
    }
}
