using CheapFlights.DataTransferObjects;
using CheapFlights.Model;

namespace CheapFlights.Interfaces
{
    public interface IFlightOfferApiService
    {
        Task<List<FlightOfferDto>> SearchFlightOffers(SearchParameters parameters);
    }
}
