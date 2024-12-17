using CheapFlights.Presentation;
using amadeus;
using Microsoft.Extensions.Caching.Memory;
using CheapFlights.Presentation;
using CheapFlights.Application;
using CheapFlights.Infrastructure.Interfaces;
using CheapFlights.Application.Interfaces;
using CheapFlights.Infrastructure.Services;
using CheapFlights.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var configuration = builder.Configuration;
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FlightCacheService>(sp =>
{
    var cache = sp.GetRequiredService<IMemoryCache>();
    return new FlightCacheService(cache, TimeSpan.FromMinutes(30));
});

builder.Services.AddScoped<IFlightSearchManagerService, FlightSearchManagerService>(sp =>
{
    var cacheService = sp.GetRequiredService<FlightCacheService>();
    var amadeusService = sp.GetRequiredService<IFlightOfferApiService>();
    return new FlightSearchManagerService(cacheService, amadeusService);
});

builder.Services.AddScoped<PresentationService>();

builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiKey = configuration["AMADEUS_API_KEY"];
    var apiSecret = configuration["AMADEUS_API_SECRET"];
    
    return Amadeus.builder(apiKey, apiSecret).build();
});

builder.Services.AddScoped<IFlightOfferApiService, AmadeusApiClientService>(sp =>
{   
    var amadeus = sp.GetRequiredService<Amadeus>();
    return new AmadeusApiClientService(amadeus);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
