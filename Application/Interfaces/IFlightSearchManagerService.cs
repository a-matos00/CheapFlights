using CheapFlights.Application.DTOs;
using CheapFlights.Domain.Entities;

namespace CheapFlights.Application.Interfaces
{
    public interface IFlightSearchManagerService
    {
        Task<List<FlightOfferDto>> Search(SearchParameters searchParams);
    }
}
