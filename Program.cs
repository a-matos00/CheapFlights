using CheapFlights.Components;
using CheapFlights.Interfaces;
using CheapFlights.Services;
using amadeus;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var configuration = builder.Configuration;
builder.Services.AddMemoryCache();
builder.Services.AddScoped<FlightCacheService>(sp =>
{
    var cache = sp.GetRequiredService<IMemoryCache>();
    return new FlightCacheService(cache, TimeSpan.FromMinutes(30));
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

builder.Services.AddScoped<FlightSearchManager>(sp =>
{
    var cacheService = sp.GetRequiredService<FlightCacheService>();
    var amadeusService = sp.GetRequiredService<IFlightOfferApiService>();
    return new FlightSearchManager(cacheService, amadeusService);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
