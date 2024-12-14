using CheapFlights.DataTransferObjects;
using amadeus.resources;
using System.Globalization;
using CheapFlights.Services.Exceptions;

namespace CheapFlights.Extensions;

public static class FlightOfferExtensions
{
    public static List<FlightOfferDto> ToFlightOfferData(this List<FlightOffer> flightOffers)
    {
        List<FlightOfferDto> offers = new List<FlightOfferDto>();

        foreach (var offer in flightOffers)
        {
            try
            {
                var stops = offer.itineraries[0].segments.Count - 1;
                int returnStops;
                if (offer.itineraries.Count > 1)
                {
                    returnStops = offer.itineraries[1].segments.Count - 1;
                }
                else
                {
                    returnStops = 0;
                }
                var offerData = new FlightOfferDto
                {
                    StopsInDepartureJourney = stops,
                    StopsInReturnJourney = returnStops,
                    OriginAirport = offer.itineraries[0].segments[0].departure.iataCode,
                    DestinationAirport = offer.itineraries[0].segments[stops].arrival.iataCode,
                    DepartureDate = DateTime.Parse(offer.itineraries[0].segments[0].departure.at),
                    ArrivalDate = DateTime.Parse(offer.itineraries[0].segments[stops].arrival.at),
                    ReturnDepartureDate = offer.itineraries.Count > 1 ? DateTime.Parse(offer.itineraries[1].segments[0].departure.at) : null,
                    ReturnArrivalDate = offer.itineraries.Count > 1 
                        ? DateTime.Parse(offer.itineraries[1].segments[returnStops].arrival.at) 
                        : null,
                    NumberOfAvailableSeats = offer.numberOfBookableSeats,
                    Currency = offer.price.currency,
                    TotalPrice = double.Parse(offer.price.grandTotal, CultureInfo.InvariantCulture)
                };

                offers.Add(offerData);
            }
            catch (Exception ex)
            {
                throw new FlightOfferDataParseException(ex.Message, ex);
            }
        }

        return offers;
    }
}