namespace CheapFlights.Services.Exceptions
{
    public class FlightOfferDataParseException : Exception
    {
        public FlightOfferDataParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
