using CheapFlights.DataTransferObjects;
using CheapFlights.Model;

namespace CheapFlights.Interfaces
{
    public interface IFlightSearchManager
    {
        Task<List<FlightOfferDto>> Search(SearchParameters searchParams);
    }
}
