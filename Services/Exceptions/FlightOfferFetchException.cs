﻿namespace CheapFlights.Services.Exceptions
{
    public class FlightOfferFetchException : Exception
    {
        public FlightOfferFetchException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}