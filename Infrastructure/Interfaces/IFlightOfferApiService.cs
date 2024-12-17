using CheapFlights.Application.DTOs;
using CheapFlights.Domain.Entities;

namespace CheapFlights.Infrastructure.Interfaces
{
    public interface IFlightOfferApiService
    {
        Task<List<FlightOfferDto>> SearchFlightOffers(SearchParameters parameters);
    }
}
