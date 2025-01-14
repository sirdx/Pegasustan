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
    public class PegasusClient
    {
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

        // JSON nodes
        protected const string LanguagesNode = "languageList";
        protected const string CountriesNode = "list";
        protected const string FaresMonthsNode = "cheapFareFlightCalenderModelList"; // Yes, there is a typo in the API
        protected const string PortMatrixRowsNode = "destinationList";
        protected const string BestDealsCitiesNode = "cities";
        protected const string BestDealsNode = "data";

        // Website constants
        protected const string DefaultLanguageCode = "en";

        protected readonly HttpClient Client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None
        });

        /// <summary>
        /// The available API response languages. 
        /// </summary>
        public Language[] Languages { get; protected set; } = Array.Empty<Language>();
        
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
        /// <remarks>The available API result languages are fetched and saved in the <c>Languages</c> property.</remarks>
        /// </summary>
        protected async Task InitializeAsync()
        {
            Languages = await GetLanguagesAsync();
            ChangeLanguage(DefaultLanguageCode);
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{LanguagesEndpoint}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                return ParseLanguages(content);
            }
        }
        
        /// <summary>
        /// Fetches airport matrix aka destination list.
        /// <remarks>This request can download a lot of data (a few megabytes). Be careful!</remarks>
        /// </summary>
        /// <param name="lastUpdatedTimestamp">The timestamp when the port matrix was last updated (default - 0 - assures the latest data).</param>
        /// <returns>An array of port matrix rows.</returns>
        public async Task<PortMatrixRow[]> GetPortMatrixAsync(long lastUpdatedTimestamp = 0L)
        {
            var queryParams = await ParamsToStringAsync(new Dictionary<string, string> { { "lastUpdatedTimestamp", lastUpdatedTimestamp.ToString() } });
            var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseWebApiAddress}{PortMatrixEndpoint}?{queryParams}");
            PrepareWebApiRequest(request);
            
            var response = await Client.SendAsync(request);
            
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(stream);
                return ParsePortMatrix(content);
            }
        }

        /// <summary>
        /// Fetches departure countries.
        /// </summary>
        /// <returns>An array of departure countries.</returns>
        public async Task<Country[]> GetDepartureCountriesAsync()
        {
            var langCode = DefaultLanguage.Code.ToLower();
            var response = await Client.GetAsync($"{BaseApiAddress}{DepartureCountryPortsEndpoint}/{langCode}");
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                return ParseCountries(content);
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
        /// <param name="currency">The currency in which the fares should be presented.</param>
        /// <returns>An array of fares months.</returns>
        public async Task<FaresMonth[]> GetFaresMonthsAsync(Port departurePort, Port arrivalPort, DateTime flightDate, Currency currency)
        {
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
        /// Fetches cities for best deals.
        /// </summary>
        /// <returns>An array of cities.</returns>
        public async Task<BestDealsCity[]> GetCitiesForBestDealsAsync()
        {
            var langCode = DefaultLanguage.Code.ToLower();
            var queryParams = await ParamsToStringAsync(new Dictionary<string, string> { { "language", langCode } });
            var response = await Client.GetAsync($"{BaseApiAddress}{CitiesForBestDealsEndpoint}?{queryParams}");
            
            using (var jsonStream = await response.Content.ReadAsStreamAsync())
            {
                var content = await JsonSerializer.DeserializeAsync<JsonObject>(jsonStream);
                return ParseBestDealsCities(content);
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
        
        private PortMatrixRow[] ParsePortMatrix(JsonObject content)
        {
            var portMatrixRowsNode = content[PortMatrixRowsNode].AsArray();
            return portMatrixRowsNode.Select(PortMatrixRow.Parse).ToArray();
        }
        
        private BestDealsCity[] ParseBestDealsCities(JsonObject content)
        {
            var citiesNode = content[BestDealsCitiesNode].AsArray();
            return citiesNode.Select(BestDealsCity.Parse).ToArray();
        }
        
        private BestDeal[] ParseBestDeals(JsonObject content, BestDealsCity departureCity, Currency currency)
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
