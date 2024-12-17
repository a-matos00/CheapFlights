namespace CheapFlights.Infrastructure.Exceptions
{
    public class FlightOfferDataParseException : Exception
    {
        public FlightOfferDataParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
