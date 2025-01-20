# <img alt="✈️" src="./Pegasustan/icon.png" width="28"/> Pegasustan
[![dotnet](https://img.shields.io/badge/-.NET%20Standard%202.0-5C2D91?style=for-the-badge)](https://dotnet.microsoft.com/) [![latest version](https://img.shields.io/nuget/v/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan) [![downloads](https://img.shields.io/nuget/dt/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan)

[Pegasus](https://www.flypgs.com/) Unofficial API client written in .NET Standard 2.0.

It provides a simple way to fetch ticket fares for a specified route. More functionality perhaps will be added later.

> [!Warning]
> This project is not (and will not be) related to [Pegasus NDC API](https://devportal.flypgs.com/).

> [!Note]
> This project is in no way affiliated with Pegasus Airlines. This is just a hobby project created for fun.

## Installation
Pegasustan is available on [NuGet](https://www.nuget.org/packages/Pegasustan).

## Usage
The following code demonstrates basic usage of Pegasustan.

> [!Warning]
> Pegasus API is unstable! Some functionalities can be suddenly gone or do not work as expected.

```csharp
try 
{
    // Create and initialize PegasusClient
    PegasusClient client = await PegasusClient.CreateAsync();
    
    // Fetch departure countries
    Country[] countries = await client.GetDepartureCountriesAsync();
    
    // Pick a departure port
    Port ankaraPort = countries.FindPortByCountryAndPortCode("TR", "ESB");
    
    // Fetch arrival countries
    Country[] arrivalCountries = await client.GetArrivalCountriesAsync(ankaraPort);
    
    // Pick an arrival port
    Port berlinPort = arrivalCountries.FindPortByCountryAndPortCode("DE", "BER");
    
    // Pick a currency for the request
    Currency usd = (await client.GetCurrenciesAsync()).FindByCode("USD");
    
    // Fetch ticket fares months counting from today in USD
    FaresMonth[] faresMonths = await client.GetFaresMonthsAsync(
        ankaraPort, 
        berlinPort, 
        DateTime.Today.ToUniversalTime(), 
        usd);
}
catch (PegasusException ex)
{
    // The API is likely unavailable or changed...
}
```

### Language selection
The following code demonstrates how to change the API response language.

```csharp
client.ChangeLanguage("tr");
```

Currently, there are 8 available languages (according to [Pegasus](https://www.flypgs.com/) website):
- en (English, default)
- tr (Turkish)
- de (German)
- fr (French)
- ru (Russian)
- it (Italian)
- es (Spanish)
- ar (Arabic)

### Caching
`PegasusClient` supports 3 modes of response caching:
- **None** - Do not cache responses at all.
- **_Smart_** - Cache some of the responses according to their probable lifetime/freshness. _Set by default._
- **Forced** - Cache all the responses that can be cached and do not update them at any point.

You can change the mode using the `Caching` property in the `PegasusClient`.

```csharp
client.Caching = PegasusClient.CachingMode.None;
```

### Port matrix
The following code demonstrates how to get and iterate through all of the airport combinations.

```csharp
// Fetch port matrix
PortMatrixRow[] portMatrix = await client.GetPortMatrixAsync();

// Iterate through the rows
foreach (PortMatrixRow row in portMatrix)
{
    PortMatrixItem departurePort = row.Departure;

    foreach (PortMatrixItem arrivalPort in row.Arrivals)
    {
        // Print all combinations like: ESB -> BER
        Console.WriteLine($"{departurePort.Code} -> {arrivalPort.Code}");
    }
}
```

### Best deals
The following code demonstrates how to get the best deals for a certain city.

```csharp
// Fetch cities with the best deals
BestDealsCity[] cities = await client.GetCitiesForBestDealsAsync();

// Pick a city
BestDealsCity amsterdamCity = cities.FindByCode("AMS");

// Fetch best deals in USD
BestDeal[] bestDeals = await client.GetBestDealsAsync(amsterdamCity, Currency.Dollar);
```

## Contributing
There is a lot of to discover and add.

I really welcome community pull requests for bug fixes, enhancements, and documentation.

## License
[GNU GPLv3](LICENSE.txt)
