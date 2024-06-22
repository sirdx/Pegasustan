# Pegasustan
[![dotnet](https://img.shields.io/badge/-.NET%20Standard%202.0-5C2D91?style=for-the-badge)](https://dotnet.microsoft.com/) [![latest version](https://img.shields.io/nuget/v/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan) [![downloads](https://img.shields.io/nuget/dt/Pegasustan?style=for-the-badge)](https://www.nuget.org/packages/Pegasustan)

[Pegasus](https://www.flypgs.com/) API written in .NET Standard 2.0.

It provides a simple way to fetch ticket fares for a specified route. More functionality perhaps will be added later.

## Installation
Pegasustan **will be** available on [NuGet](https://www.nuget.org/packages/Pegasustan).<br/>
For the moment a few more things are needed to publish the first build.

## Basic usage
The following code demonstrates basic usage of Pegasustan.

```csharp
// Create and initialize PegasusClient
PegasusClient client = new PegasusClient();
await client.InitializeAsync();

// Fetch departure countries
Country[] countries = await client.FetchDepartureCountriesAsync();

// Pick the departure country and port
Country turkiye = countries.Single(c => c.Code == "TR");
Port ankaraPort = turkiye.Ports.Single(p => p.Code == "ESB");

// Fetch arrival countries
Country[] arrivalCountries = await client.FetchArrivalCountriesAsync(ankaraPort);

// Pick the arrival country and port
Country germany = arrivalCountries.Single(c => c.Code == "DE");
Port berlinPort = germany.Ports.Single(p => p.Code == "BER");

// Fetch ticket fares months counting from today in USD
FaresMonth[] faresMonths = await client.FetchFaresMonthsAsync(ankaraPort, berlinPort, DateTime.Today, Currency.Dollar);
```

## Notice
This project is in no way affiliated with Pegasus Airlines. This is just a hobby project created for fun.

## Contributing
There is a lot of to discover and add.<br/>
I really welcome community pull requests for bug fixes, enhancements, and documentation.

## License
[GNU GPLv3](LICENSE.txt)
