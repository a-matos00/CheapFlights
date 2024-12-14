using CheapFlights.DataTransferObjects;
using amadeus;
using amadeus.exceptions;
using amadeus.resources;
using Newtonsoft.Json;
using CheapFlights.Services.Exceptions;
using CheapFlights.Model;
using CheapFlights.Extensions;
using CheapFlights.Interfaces;

namespace CheapFlights.Services;

public class AmadeusApiClientService : IFlightOfferApiService
{
    #region Constants

    const uint REQUEST_TIMEOUT_SECONDS = 10;

    #endregion Constants

    #region Private fields

    private readonly Amadeus _amadeusApiClient;

    #endregion


    #region Constructors
    public AmadeusApiClientService(Amadeus amadeusClient)
    {
        _amadeusApiClient = amadeusClient;
    }

    #endregion Constructors

    #region Public Methods

    public async Task<List<FlightOfferDto>> SearchFlightOffers(SearchParameters parameters)
    {
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
            paramsBuilder = paramsBuilder.and("returnDate", parameters.ReturnDate.Value.ToString("yyyy-MM-dd"));
        }

        var flightOfferList = await FetchRawFlightOfferData(paramsBuilder);
        return ParseRawFlightOffers(flightOfferList);
    }

    #endregion

    #region Private methods

    private List<FlightOfferDto> ParseRawFlightOffers(List<FlightOffer> flightOffers)
    {
        return flightOffers.ToFlightOfferData();
    }


    /// <summary>
    /// This method is a workaround for the amadeus.shopping.flightOffers.get() method<br></br>
    /// because the original uses v1 API which has probles with API client authentication.
    /// </summary>
    private async Task<Response> GetWrapper(Params searchParameters)
    {
        var responseTask = await Task.Run(() =>
        {
            return _amadeusApiClient.get("/v2/shopping/flight-offers", searchParameters);
        });

        return responseTask;
    }

    private async Task<List<FlightOffer>> FetchRawFlightOfferData(Params searchParameters)
    {
        try
        {
            var timeout = (int)TimeSpan.FromSeconds(REQUEST_TIMEOUT_SECONDS).TotalMilliseconds;
            var responseTask = GetWrapper(searchParameters);

            if (await Task.WhenAny(responseTask, Task.Delay(timeout)) == responseTask)
            {
                var response = responseTask;
                return DeserializeFlightOffers(response.Result);
            }
            else
            {
                throw new TimeoutException($"The request to Amadeus API timed out after {REQUEST_TIMEOUT_SECONDS} seconds.");
            }
        }
        catch (TimeoutException ex)
        {
            throw new FlightOfferFetchException(ex.Message, ex);
        }
        catch (ResponseException ex)
        {
            throw new FlightOfferFetchException(ex.Message, ex);
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
            throw new ApplicationException("Failed to deserialize flight offers.", ex);
        }
    }

    #endregion
}
