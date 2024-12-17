# Cheap flight offer finder

This is a ASP.NET application with a Blazor frontend. It can be used to access flight offer data from the Amadeus API through an UI.

The application is hosted on Azure here : 
https://cheapfligths-g9hyhqdte5edh5ds.westeurope-01.azurewebsites.net/

## For Users

### Search parameters
 - Origin and destination airport IATA codes
 - Start and return date
 - Number of passengers (Adults, child, infant)
 - Currency
 - One way / Return voyage

### Offer data
 - Origin and destination airport IATA codes
 - Start and return date and time
 - Number of travelers
 - Total price in selected currency
 - Number of stops to and from destination

## For Developers

### Responsibility chart

![Responsibility chart](https://imgur.com/OjbjXf8.png)
### Local caching
If the API call successfully returned data, it is mapped to the search parameters and stored in the memory space of the process that is hosting the application (server RAM). The key to access the data is a string which is made from the search parameter values. The cached data has an expiration time of x minutes.

### Dependencies
-  **amadeus-dotnet**: A .NET client for the Amadeus API, used for accessing flight offers and related data.
