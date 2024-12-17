using System.ComponentModel.DataAnnotations;
using CheapFlights.Domain.Enums;
using CheapFlights.Domain.Validation;

namespace CheapFlights.Domain.Entities
{
    public class SearchParameters
    {
        [Required(ErrorMessage = "Origin airport code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Airport code must be exactly 3 characters")]
        [CustomValidation(typeof(SearchParametersValidator), nameof(SearchParametersValidator.ValidateIataCode))]
        public string OriginLocationCode { get; set; }

        [Required(ErrorMessage = "Destination airport code is required")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Airport code must be exactly 3 characters")]
        [CustomValidation(typeof(SearchParametersValidator), nameof(SearchParametersValidator.ValidateIataCode))]
        public string DestinationLocationCode { get; set; }

        [Required(ErrorMessage = "Departure date is required")]
        [CustomValidation(typeof(SearchParametersValidator), nameof(SearchParametersValidator.ValidateDepartureDate))]
        public DateTime DepartureDate { get; set; }

        [Required(ErrorMessage = "Return date is required")]
        [CustomValidation(typeof(SearchParametersValidator), nameof(SearchParametersValidator.ValidateReturnDate))]
        public DateTime? ReturnDate { get; set; }

        [CustomValidation(typeof(SearchParametersValidator), nameof(SearchParametersValidator.ValidatePassengerCount))]
        public Passengers Passengers { get; set; }

        [Required(ErrorMessage = "Currency is required")]
        [EnumDataType(typeof(CurrencyCode), ErrorMessage = "Invalid currency code")]
        public CurrencyCode Currency { get; set; }

        public bool IsOneWay { get; set; }

        public SearchParameters()
        {
            OriginLocationCode = "SPU";
            DestinationLocationCode = "ZAG";
            DepartureDate = DateTime.Now;
            ReturnDate = DateTime.Now.AddDays(3);
            Passengers = new Passengers(1, 0, 0);
            Currency = CurrencyCode.EUR;
        }
    }
}
