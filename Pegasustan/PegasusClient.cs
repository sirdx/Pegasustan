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

namespace Pegasustan
{
    public class PegasusClient
    {
        // www.flypgs.com API
        protected const string BaseApiAddress = "https://www.flypgs.com/apint/";
        protected const string DepartureCountryPortsEndpoint = "pm/dep";
        protected const string ArrivalCountryPortsEndpoint = "pm/arr";
        protected const string FaresEndpoint = "cheapfare/flight-calender-prices";
        
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
        /// <remarks>It automatically</remarks>
        /// </summary>
        /// <returns>An initialized <see cref="T:Pegasustan.PegasusClient" /> object.</returns>
        public static async Task<PegasusClient> CreateAsync()
        {
            var client = new PegasusClient();
            await client.InitializeAsync();
            
            return client;
        }
        
        /// <summary>
        /// Initializes the <see cref="T:Pegasustan.PegasusClient" /> asynchronously.
        /// <remarks>The available API result languages are fetched and saved in the <c>Languages</c> property.</remarks>
        /// </summary>
        protected async Task InitializeAsync()
        {
            Languages = await FetchLanguagesAsync();
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
        public async Task<bool> FetchStatusAsync()
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
        public async Task<Language[]> FetchLanguagesAsync()
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
        public async Task<PortMatrixRow[]> FetchPortMatrixAsync(long lastUpdatedTimestamp = 0L)
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
        public async Task<Country[]> FetchDepartureCountriesAsync()
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
        public async Task<Country[]> FetchArrivalCountriesAsync(Port departurePort)
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
        /// <param name="flightDate">The date from which the fares months should be counted.</param>
        /// <param name="currency">The currency in which the fares should be presented.</param>
        /// <returns>An array of fares months.</returns>
        public async Task<FaresMonth[]> FetchFaresMonthsAsync(Port departurePort, Port arrivalPort, DateTime flightDate, Currency currency)
        {
            var payload = new
            {
                depPort = departurePort.Code,
                arrPort = arrivalPort.Code,
                flightDate = flightDate.ToString("yyyy-MM-dd"),
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

        protected static async Task<string> ParamsToStringAsync(Dictionary<string, string> urlParams)
        {
            using (HttpContent content = new FormUrlEncodedContent(urlParams))
            {
                return await content.ReadAsStringAsync();
            }
        }
    }
}
