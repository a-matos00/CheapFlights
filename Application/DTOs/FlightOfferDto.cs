namespace CheapFlights.Application.DTOs
{
    public class FlightOfferDto
    {
        public string OriginAirport { get; set; }
        public string DestinationAirport { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDepartureDate { get; set; }
        public DateTime? ReturnArrivalDate { get; set; }
        public int StopsInDepartureJourney { get; set; }
        public int StopsInReturnJourney { get; set; }
        public int NumberOfAvailableSeats { get; set; }
        public string Currency { get; set; }
        public double TotalPrice { get; set; }
    }
}