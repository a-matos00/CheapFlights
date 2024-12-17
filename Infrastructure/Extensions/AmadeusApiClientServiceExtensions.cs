using amadeus.resources;
using System.Globalization;
using CheapFlights.Infrastructure.Exceptions;
using CheapFlights.Application.DTOs;

namespace CheapFlights.Infrastructure.Extensions;

public static class FlightOfferExtensions
{
    const int DEPARTURE_ITINERARY_INDEX = 0;
    const int DEPARTURE_SEGMENT_INDEX = 0;
    const int RETURN_ITINERARY_INDEX = 1;

    public static List<FlightOfferDto> ToFlightOfferDto(this List<FlightOffer> flightOffers)
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
                    OriginAirport = offer.itineraries[DEPARTURE_ITINERARY_INDEX].segments[DEPARTURE_SEGMENT_INDEX].departure.iataCode,
                    DestinationAirport = offer.itineraries[DEPARTURE_ITINERARY_INDEX].segments[stops].arrival.iataCode,
                    DepartureDate = DateTime.Parse(offer.itineraries[DEPARTURE_ITINERARY_INDEX].segments[DEPARTURE_SEGMENT_INDEX].departure.at),
                    ArrivalDate = DateTime.Parse(offer.itineraries[DEPARTURE_ITINERARY_INDEX].segments[stops].arrival.at),
                    ReturnDepartureDate = offer.itineraries.Count > 1 ? DateTime.Parse(offer.itineraries[RETURN_ITINERARY_INDEX].segments[DEPARTURE_SEGMENT_INDEX].departure.at) : null,
                    ReturnArrivalDate = offer.itineraries.Count > 1
                        ? DateTime.Parse(offer.itineraries[RETURN_ITINERARY_INDEX].segments[returnStops].arrival.at)
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