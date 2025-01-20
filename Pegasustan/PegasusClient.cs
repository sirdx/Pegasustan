using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Pegasustan.Domain;
using Pegasustan.Utils;

namespace Pegasustan
{
    /// <summary>
    /// Provides a class for sending and receiving unofficial Pegasus API requests and responses.
    /// </summary>
    public class PegasusClient
    {
        /// <summary>
        /// Represents the response caching mode used throughout the <see cref="T:Pegasustan.PegasusClient"/> while sending requests to the API.
        /// <remarks>Works only for the requests that can or should be cached.</remarks>
        /// </summary>
        public enum CachingMode
        {
            /// <summary>
            /// Do not cache responses at all.
            /// </summary>
            None,
            
            /// <summary>
            /// Cache some of the responses according to their probable lifetime/freshness.
            /// </summary>
            Smart,
            
            /// <summary>
            /// Cache all the responses that can be cached and do not update them at any point.
            /// </summary>
            Forced
        }
        
        // www.flypgs.com API
        protected const string BaseApiAddress = "https://www.flypgs.com/apint/";
        protected const string DepartureCountryPortsEndpoint = "pm/dep";
        protected const string ArrivalCountryPortsEndpoint = "pm/arr";
        protected const string FaresEndpoint = "cheapfare/flight-calender-prices";
        protected const string CitiesForBestDealsEndpoint = "BestDeals/GetCitiesForBestDeals";
        protected const string BestDealsEndpoint = "BestDeals/GetBestDeals";
        
        // web.flypgs.com API
        protected const string BaseWebApiAddress = "https://web.flypgs.com/pegasus/";
        protected const string StatusEndpoint = "cheapest-fare/status";
        protected const string LanguagesEndpoint = "common/languages";
        protected const string PortMatrixEndpoint = "port-matrix";
        protected const string CurrenciesEndpoint = "common/currency-codes";
        protected const string FareCurrenciesEndpoint = "cheapest-fare/currency-codes";

        // JSON nodes
        protected const string LanguagesNode = "languageList";
        protected const string CountriesNode = "list";
        protected const string FaresMonthsNode = "cheapFareFlightCalenderModelList"; // Yes, there is a typo in the API
        protected const string PortMatrixRowsNode = "destinationList";
        protected const string BestDealsCitiesNode = "cities";
        protected const string BestDealsNode = "data";
        protected const string CurrenciesNode = "codeList";

        // Website constants
        protected const string DefaultLanguageCode = "en";

        protected readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None
        });
        
        /// <summary>
        /// The cached API response languages. 
        /// </summary>
        protected Language[] Languages = Array.Empty<Language>();
        protected DateTimeOffset LanguagesLastCacheTime = DateTimeOffset.MinValue;
        protected readonly TimeSpan LanguagesCacheTime = new TimeSpan(12, 0, 0); 
        
        /// <summary>
        /// The cached API response currencies. 
        /// </summary>
        protected Currency[] Currencies = Array.Empty<Currency>();
        protected DateTimeOffset CurrenciesLastCacheTime = DateTimeOffset.MinValue;
        protected readonly TimeSpan CurrenciesCacheTime = new TimeSpan(0, 30, 0); 
        
        /// <summary>
        /// The cached port matrix. 
        /// </summary>
        protected PortMatrixRow[] PortMatrix = Array.Empty<PortMatrixRow>();
        protected DateTimeOffset PortMatrixLastCacheTime = DateTimeOffset.MinValue;
        protected readonly TimeSpan PortMatrixCacheTime = new TimeSpan(6, 0, 0);                
        
        /// <summary>
        /// The cached departure countries. 
        /// </summary>
        protected Country[] DepartureCountries = Array.Empty<Country>();
        protected DateTimeOffset DepartureCountriesLastCacheTime = DateTimeOffset.MinValue;
        protected readonly TimeSpan DepartureCountriesCacheTime = new TimeSpan(6, 0, 0);           
        
        /// <summary>
        /// The cached cities for best-deals. 
        /// </summary>
        protected BestDealsCity[] CitiesForBestDeals = Array.Empty<BestDealsCity>();
        protected DateTimeOffset CitiesForBestDealsLastCacheTime = DateTimeOffset.MinValue;
        protected readonly TimeSpan CitiesForBestDealsCacheTime = new TimeSpan(2, 0, 0);        

        /// <summary>
        /// The API response caching mode (for some of the requests that can or should be cached).
        /// <remarks><c>Smart</c> mode should be enough for most of the users, so it is set by default.</remarks>
        /// </summary>
        public CachingMode Caching { get; set; } = CachingMode.Smart;
        
        /// <summary>
        /// The default API response language.
        /// <remarks>It should be initialized by default to English.</remarks>
        /// </summary>
        public Language DefaultLanguage { get; set; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.PegasusClient" /> class.
        /// </summary>
        protected PegasusClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:125.0) Gecko/20100101 Firefox/125.0");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Platform", "web");
            Client.DefaultRequestHeaders.Accept.ParseAdd("*/*");
        }

        /// <summary>
        /// Creates and initializes a new instance of the <see cref="T:Pegasustan.PegasusClient" /> class.
        /// </summary>
        /// <param name="statusCheck">Should the status of the API be checked (true by default).</param>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">Thrown when <paramref name="statusCheck"/> is true and the Pegasus API is unavailable.</exception>
        /// <returns>An initialized <see cref="T:Pegasustan.PegasusClient" /> object.</returns>
        public static async Task<PegasusClient> CreateAsync(bool statusCheck = true)
        {
            var client = new PegasusClient();

            if (statusCheck)
            {
                if (!await client.GetStatusAsync())
                {
                    throw new PegasusException("The Pegasus API is currently unavailable.");
                }
            }

            await client.InitializeAsync();
            return client;
        }
        
        /// <summary>
        /// Initializes the <see cref="T:Pegasustan.PegasusClient" /> asynchronously.
        /// <remarks>The available API result languages and currencies are cached.</remarks>
        /// </summary>
        protected async Task InitializeAsync()
        {
            await GetLanguagesAsync();
            ChangeLanguage(DefaultLanguageCode);

            await GetCurrenciesAsync();
        }

        /// <summary>
        /// Changes the API response language.
        /// </summary>
        /// <param name="code">The language code.</param>
        public void ChangeLanguage(string code)
        {
            DefaultLanguage = Languages.FindByCode(code);
        }
        
        /// <summary>
        /// Fetches API status.
        /// </summary>
        /// <returns>Boolean representing the service availability.</returns>
        public async Task<bool> GetStatusAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{StatusEndpoint}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream); 
                content.TryGetPropertyValue("status", out var statusNode);
                return (bool)statusNode;
            }
        }

        /// <summary>
        /// Fetches available API response languages.
        /// </summary>
        /// <returns>An array of available API response languages.</returns>
        public async Task<Language[]> GetLanguagesAsync()
        {
            if (Languages.Length != 0)
            {
                switch (Caching)
                {
                    case CachingMode.Forced:
                    case CachingMode.Smart when DateTimeOffset.UtcNow.Subtract(LanguagesLastCacheTime).Ticks < LanguagesCacheTime.Ticks:
                        return Languages;
                }
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{LanguagesEndpoint}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                var languages = ParseLanguages(content);

                Languages = Caching != CachingMode.None ? languages : Array.Empty<Language>();
                LanguagesLastCacheTime = Caching != CachingMode.None ? DateTimeOffset.UtcNow : DateTimeOffset.MinValue;
                return languages;
            }
        }
        
        /// <summary>
        /// Fetches available API response currencies.
        /// </summary>
        /// <returns>An array of available API response currencies.</returns>
        public async Task<Currency[]> GetCurrenciesAsync()
        {
            if (Currencies.Length != 0)
            {
                switch (Caching)
                {
                    case CachingMode.Forced:
                    case CachingMode.Smart when DateTimeOffset.UtcNow.Subtract(CurrenciesLastCacheTime).Ticks < CurrenciesCacheTime.Ticks:
                        return Currencies;
                }
            }
            
            var fareRequest = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{FareCurrenciesEndpoint}");
            PrepareWebApiRequest(fareRequest);
            
            var fareResponse = await Client.SendAsync(fareRequest);
            IEnumerable<string> fareSupportingCurrencies;
            
            using (var stream = await fareResponse.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                var currenciesNode = content[CurrenciesNode].AsArray();
                fareSupportingCurrencies = currenciesNode.Select(currency => (string)currency);
            }
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{CurrenciesEndpoint}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                var currencies = ParseCurrencies(content, fareSupportingCurrencies);
                
                Currencies = Caching != CachingMode.None ? currencies : Array.Empty<Currency>();
                CurrenciesLastCacheTime = Caching != CachingMode.None ? DateTimeOffset.UtcNow : DateTimeOffset.MinValue;
                return currencies;
            }
        }
        
        /// <summary>
        /// Fetches airport matrix aka destination list.
        /// <remarks>This request can download a lot of data (a few megabytes). Be careful!</remarks>
        /// </summary>
        /// <param name="lastUpdatedTimestamp">The timestamp when the port matrix was last updated (default - 0 - assures the latest data).
        /// This parameter is not guaranteed to work properly. Even if you pass a fresh timestamp, the response will be the whole matrix.</param>
        /// <returns>An array of port matrix rows.</returns>
        public async Task<PortMatrixRow[]> GetPortMatrixAsync(long lastUpdatedTimestamp = 0L)
        {
            if (PortMatrix.Length != 0)
            {
                switch (Caching)
                {
                    case CachingMode.Forced:
                    case CachingMode.Smart when DateTimeOffset.UtcNow.Subtract(PortMatrixLastCacheTime).Ticks < PortMatrixCacheTime.Ticks:
                        return PortMatrix;
                }
            }
            
            var queryParams = await ParamsToStringAsync(new Dictionary<string, string> { { "lastUpdatedTimestamp", lastUpdatedTimestamp.ToString() } });
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{PortMatrixEndpoint}?{queryParams}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                var portMatrix = ParsePortMatrix(content);
                
                PortMatrix = Caching != CachingMode.None ? portMatrix : Array.Empty<PortMatrixRow>();
                PortMatrixLastCacheTime = Caching != CachingMode.None ? DateTimeOffset.UtcNow : DateTimeOffset.MinValue;
                return portMatrix;
            }
        }

        /// <summary>
        /// Fetches departure countries.
        /// </summary>
        /// <returns>An array of departure countries.</returns>
        public async Task<Country[]> GetDepartureCountriesAsync()
        {
            if (DepartureCountries.Length != 0)
            {
                switch (Caching)
                {
                    case CachingMode.Forced:
                    case CachingMode.Smart when DateTimeOffset.UtcNow.Subtract(DepartureCountriesLastCacheTime).Ticks < DepartureCountriesCacheTime.Ticks:
                        return DepartureCountries;
                }
            }
            
            var langCode = DefaultLanguage.Code.ToLower();
            var response = await Client.GetAsync($"{BaseApiAddress}{DepartureCountryPortsEndpoint}/{langCode}");
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                var countries = ParseCountries(content);

                DepartureCountries = Caching != CachingMode.None ? countries : Array.Empty<Country>();
                DepartureCountriesLastCacheTime = Caching != CachingMode.None ? DateTimeOffset.UtcNow : DateTimeOffset.MinValue;
                return countries;
            }
        }
        
        /// <summary>
        /// Fetches arrival countries based on the provided departure port.
        /// </summary>
        /// <param name="departurePort">The port from which the flight begins.</param>
        /// <returns>An array of possible arrival countries.</returns>
        public async Task<Country[]> GetArrivalCountriesAsync(Port departurePort)
        {
            var langCode = DefaultLanguage.Code.ToLower();
            var response = await Client.GetAsync($"{BaseApiAddress}{ArrivalCountryPortsEndpoint}/{langCode}/{departurePort.Code}");
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                return ParseCountries(content);
            }
        }
        
        /// <summary>
        /// Fetches fares months for the given route and currency.
        /// </summary>
        /// <param name="departurePort">The port from which the flight begins.</param>
        /// <param name="arrivalPort">The port at which the flight ends.</param>
        /// <param name="flightDate">The UTC based date from which the fares months should be counted.</param>
        /// <param name="currency">The currency in which the fares should be presented (only the ones that support cheapest-fare requests).</param>
        /// <returns>An array of fares months.</returns>
        /// <exception cref="ArgumentException">Passed currency does not support cheapest-fare requests.</exception>
        public async Task<FaresMonth[]> GetFaresMonthsAsync(Port departurePort, Port arrivalPort, DateTime flightDate, Currency currency)
        {
            if (!currency.SupportsCheapestFare)
            {
                throw new ArgumentException("Currency does not support cheapest fare requests.");
            }
            
            var localFlightDate = ConvertToTurkeyTimeZone(flightDate);
            
            var payload = new
            {
                depPort = departurePort.Code,
                arrPort = arrivalPort.Code,
                flightDate = localFlightDate.ToString("yyyy-MM-dd"),
                currency = currency.Code
            };
            
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync($"{BaseApiAddress}{FaresEndpoint}", content);
            response.EnsureSuccessStatusCode();
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var responseContent = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                return ParseFaresMonths(responseContent, departurePort, arrivalPort, currency);
            }
        }

        /// <summary>
        /// Fetches cities for best-deals.
        /// </summary>
        /// <returns>An array of cities.</returns>
        public async Task<BestDealsCity[]> GetCitiesForBestDealsAsync()
        {
            if (CitiesForBestDeals.Length != 0)
            {
                switch (Caching)
                {
                    case CachingMode.Forced:
                    case CachingMode.Smart when DateTimeOffset.UtcNow.Subtract(CitiesForBestDealsLastCacheTime).Ticks < CitiesForBestDealsCacheTime.Ticks:
                        return CitiesForBestDeals;
                }
            }
            
            var langCode = DefaultLanguage.Code.ToLower();
            var queryParams = await ParamsToStringAsync(new Dictionary<string, string> { { "language", langCode } });
            var response = await Client.GetAsync($"{BaseApiAddress}{CitiesForBestDealsEndpoint}?{queryParams}");
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                var cities = ParseBestDealsCities(content);

                CitiesForBestDeals = Caching != CachingMode.None ? cities : Array.Empty<BestDealsCity>();
                CitiesForBestDealsLastCacheTime = Caching != CachingMode.None ? DateTimeOffset.UtcNow : DateTimeOffset.MinValue;
                return cities;
            }
        }
        
        /// <summary>
        /// Fetches best deals for the given city and currency.
        /// <remarks>Supports pagination.</remarks>
        /// </summary>
        /// <param name="departureCity">The best-deals city from which the flights begin.</param>
        /// <param name="currency">The currency in which the fares should be presented.</param>
        /// <param name="page">The page of deals (each contains at most 10 elements, set to 0 by default).</param>
        /// <returns>An array of best deals.</returns>
        public async Task<BestDeal[]> GetBestDealsAsync(BestDealsCity departureCity, Currency currency, uint page = 0U)
        {
            var payload = new
            {
                channel = "WEB",
                currency = currency.Code,
                domestic = "tr" == DefaultLanguage.Code.ToLower(),
                languageCode = DefaultLanguage.Code,
                depPort = departureCity.Code,
                page
            };
            
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var queryParams = await ParamsToStringAsync(new Dictionary<string, string> { { "v", timestamp.ToString() } });
            
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync($"{BaseApiAddress}{BestDealsEndpoint}?{queryParams}", content);
            response.EnsureSuccessStatusCode();
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var responseContent = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                return ParseBestDeals(responseContent, departureCity, currency);
            }
        }

        protected static void PrepareWebApiRequest(HttpRequestMessage request)
        {
            // Web API for some reason requires 'Content-Type: application/json' in a GET request
            // As a solution an empty JSON is passed, setting a request header manually does not work
            request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        }

        protected static Language[] ParseLanguages(JsonObject jsonObject)
        {
            var languagesNode = jsonObject[LanguagesNode].AsArray();
            return languagesNode.Select(Language.Parse).ToArray();
        }
        
        protected Currency[] ParseCurrencies(JsonObject content, IEnumerable<string> fareSupportingCurrencies)
        {
            var currenciesNode = content[CurrenciesNode].AsArray();
            return currenciesNode.Select(node => Currency.Parse(node, fareSupportingCurrencies)).ToArray();
        }
        
        protected static Country[] ParseCountries(JsonObject jsonObject)
        {
            var countriesNode = jsonObject[CountriesNode].AsArray();
            return countriesNode.Select(Country.Parse).ToArray();
        }

        protected static FaresMonth[] ParseFaresMonths(JsonObject jsonObject, Port departurePort, Port arrivalPort, Currency currency)
        {
            var faresMonthsNode = jsonObject[FaresMonthsNode].AsArray();
            return faresMonthsNode.Select(node => FaresMonth.Parse(node, departurePort, arrivalPort, currency)).ToArray();
        }
        
        protected PortMatrixRow[] ParsePortMatrix(JsonObject content)
        {
            var portMatrixRowsNode = content[PortMatrixRowsNode].AsArray();
            return portMatrixRowsNode.Select(PortMatrixRow.Parse).ToArray();
        }
        
        protected BestDealsCity[] ParseBestDealsCities(JsonObject content)
        {
            var citiesNode = content[BestDealsCitiesNode].AsArray();
            return citiesNode.Select(BestDealsCity.Parse).ToArray();
        }
        
        protected BestDeal[] ParseBestDeals(JsonObject content, BestDealsCity departureCity, Currency currency)
        {
            var dealsNode = content[BestDealsNode].AsArray();
            return dealsNode.Select(node => BestDeal.Parse(node, departureCity, currency)).ToArray();
        }
        
        protected static async Task<string> ParamsToStringAsync(Dictionary<string, string> urlParams)
        {
            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
        
        protected static DateTime ConvertToTurkeyTimeZone(DateTime dateTime)
        {
            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time"); 
            var turkeyDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, turkeyTimeZone);
            
            return turkeyDateTime;
        }
    }
}
