# <img alt="✈️" src="./Pegasustan/icon.png" width="28"/> Pegasustan
[![dotnet](https://img.shields.io/badge/-.NET%20Standard%202.0-5C2D91?style=for-the-badge)](https://dotnet.microsoft.com/) [![latest version](https://img.shields.io/nuget/v/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan) [![downloads](https://img.shields.io/nuget/dt/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan)

Unofficial [Pegasus](https://www.flypgs.com/) API written in .NET Standard 2.0.

It provides a simple way to fetch ticket fares for a specified route. More functionality perhaps will be added later.

> [!Warning]
> This project is not (and will not be) related to [Pegasus NDC API](https://devportal.flypgs.com/)

> [!Note]
> This project is in no way affiliated with Pegasus Airlines. This is just a hobby project created for fun.

## Installation
Pegasustan is available on [NuGet](https://www.nuget.org/packages/Pegasustan).

## Basic usage
The following code demonstrates basic usage of Pegasustan.

```csharp
// Create and initialize PegasusClient
PegasusClient client = await PegasusClient.CreateAsync();

// Fetch departure countries
Country[] countries = await client.FetchDepartureCountriesAsync();

// Pick a departure port
Port ankaraPort = countries.FindPortByCountryAndPortCode("TR", "ESB");

// Fetch arrival countries
Country[] arrivalCountries = await client.FetchArrivalCountriesAsync(ankaraPort);

// Pick an arrival port
Port berlinPort = arrivalCountries.FindPortByCountryAndPortCode("DE", "BER");

// Fetch ticket fares months counting from today in USD
FaresMonth[] faresMonths = await client.FetchFaresMonthsAsync(ankaraPort, berlinPort, DateTime.Today, Currency.Dollar);
```

### Language selection
The following code demonstrates how to change the API response language.

```csharp
client.ChangeLanguage("tr");
```

Currently, there are 7 available languages (according to [Pegasus](https://www.flypgs.com/) website):
- en (English, default)
- tr (Turkish)
- de (German)
- fr (French)
- ru (Russian)
- it (Italian)
- es (Spanish)

## Contributing
There is a lot of to discover and add.

I really welcome community pull requests for bug fixes, enhancements, and documentation.

## License
[GNU GPLv3](LICENSE.txt)
