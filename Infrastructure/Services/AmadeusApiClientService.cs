using amadeus;
using amadeus.exceptions;
using amadeus.resources;
using Newtonsoft.Json;
using CheapFlights.Infrastructure.Extensions;
using CheapFlights.Infrastructure.Interfaces;
using CheapFlights.Domain.Entities;
using CheapFlights.Application.DTOs;
using CheapFlights.Domain.Exceptions;

namespace CheapFlights.Infrastructure.Services;

public class AmadeusApiClientService : IFlightOfferApiService
{
    #region Constants

    const uint REQUEST_TIMEOUT_SECONDS = 60;
    const string FLIGHT_OFFERS_ENDPOINT = "/v2/shopping/flight-offers";

    #endregion Constants

    #region Private fields

    private readonly Amadeus _amadeusApiClient;

    #endregion

    #region Constructors
    public AmadeusApiClientService(Amadeus amadeusClient)
    {
        _amadeusApiClient = amadeusClient;
    }

    #endregion

    #region Public Methods

    public async Task<List<FlightOfferDto>> SearchFlightOffers(SearchParameters parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }
        var paramsBuilder = Params
            .with("originLocationCode", parameters.OriginLocationCode)
            .and("currencyCode", parameters.Currency.ToString())
            .and("destinationLocationCode", parameters.DestinationLocationCode)
            .and("departureDate", parameters.DepartureDate.ToString("yyyy-MM-dd"))
            .and("infants", parameters.Passengers.InfantCount.ToString())
            .and("children", parameters.Passengers.ChildCount.ToString())
            .and("adults", parameters.Passengers.AdultsCount.ToString());

        if (!parameters.IsOneWay)
        {
            if (parameters.ReturnDate.HasValue)
            {
                paramsBuilder = paramsBuilder.and("returnDate", parameters.ReturnDate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                throw new ArgumentException("Return date must be specified for round-trip flights", nameof(parameters));
            }
        }

        var flightOfferList = await FetchRawFlightOfferData(paramsBuilder);

        return flightOfferList.ToFlightOfferDto();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// This method is a workaround for the amadeus.shopping.flightOffers.get() method<br></br>
    /// because the original uses v1 API which has probles with API client authentication.
    /// </summary>
    private async Task<Response> GetFlightOffersV2Api(Params searchParameters)
    {
        var responseTask = await Task.Run(() =>
        {
            return _amadeusApiClient.get(FLIGHT_OFFERS_ENDPOINT, searchParameters);
        });

        return responseTask;
    }

    private async Task<List<FlightOffer>> FetchRawFlightOfferData(Params searchParameters)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS));

        try
        {
            var response = await GetFlightOffersV2Api(searchParameters)
                .WaitAsync(cts.Token);

            return DeserializeFlightOffers(response);
        }
        catch (TaskCanceledException ex)
        {
            throw new FlightOfferFetchException("Search timed out after no response from API", ex);
        }
        catch (ResponseException ex)
        {
            throw new FlightOfferFetchException($"An error was encountered while fetching data: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new FlightOfferFetchException("An unexpected error occurred while fetching flight offers.", ex);
        }
    }

    private List<FlightOffer> DeserializeFlightOffers(Response response)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<FlightOffer>>(response.dataString);
        }
        catch (Exception ex)
        {
            throw new FlightOfferFetchException("Failed to deserialize flight offers.", ex);
        }
    }

    #endregion
}
