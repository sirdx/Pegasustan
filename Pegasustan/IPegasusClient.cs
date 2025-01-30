using System;
using System.Threading.Tasks;
using Pegasustan.Domain;

namespace Pegasustan
{
    /// <summary>
    /// Provides an interface for sending and receiving unofficial Pegasus API requests and responses.
    /// </summary>
    public interface IPegasusClient
    { 
        /// <summary>
        /// The default API response language.
        /// <remarks>It should be initialized by default to English.</remarks>
        /// </summary>
        Language DefaultLanguage { get; set; }

        /// <summary>
        /// Changes the API response language.
        /// </summary>
        /// <param name="code">The language code.</param>
        void ChangeLanguage(string code);

        /// <summary>
        /// Fetches API status.
        /// </summary>
        /// <returns>Boolean representing the service availability.</returns>
        Task<bool> GetStatusAsync();
        
        /// <summary>
        /// Fetches available API response languages.
        /// </summary>
        /// <returns>An array of available API response languages.</returns>
        Task<Language[]> GetLanguagesAsync();
        
        /// <summary>
        /// Fetches available API response currencies.
        /// </summary>
        /// <returns>An array of available API response currencies.</returns>
        Task<Currency[]> GetCurrenciesAsync();
        
        /// <summary>
        /// Fetches airport matrix aka destination list.
        /// <remarks>This request can download a lot of data (a few megabytes). Be careful!</remarks>
        /// </summary>
        /// <param name="lastUpdatedTimestamp">The timestamp when the port matrix was last updated (default - 0 - assures the latest data).
        /// This parameter is not guaranteed to work properly. Even if you pass a fresh timestamp, the response will be the whole matrix.</param>
        /// <returns>An array of port matrix rows.</returns>
        Task<PortMatrixRow[]> GetPortMatrixAsync(long lastUpdatedTimestamp = 0L);
        
        /// <summary>
        /// Fetches departure countries.
        /// </summary>
        /// <returns>An array of departure countries.</returns>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">The request was not successful.</exception>
        Task<Country[]> GetDepartureCountriesAsync();
        
        /// <summary>
        /// Fetches arrival countries based on the provided departure port.
        /// </summary>
        /// <param name="departurePort">The port from which the flight begins.</param>
        /// <returns>An array of possible arrival countries.</returns>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">The request was not successful.</exception>
        Task<Country[]> GetArrivalCountriesAsync(Port departurePort);
        
        /// <summary>
        /// Fetches fares months for the given route and currency.
        /// </summary>
        /// <param name="departurePort">The port from which the flight begins.</param>
        /// <param name="arrivalPort">The port at which the flight ends.</param>
        /// <param name="flightDate">The UTC based date from which the fares months should be counted.</param>
        /// <param name="currency">The currency in which the fares should be presented (only the ones that support cheapest-fare requests).</param>
        /// <returns>An array of fares months.</returns>
        /// <exception cref="ArgumentException">Passed currency does not support cheapest-fare requests.</exception>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">The request was not successful.</exception>
        Task<FaresMonth[]> GetFaresMonthsAsync(Port departurePort, Port arrivalPort, DateTime flightDate, Currency currency);
        
        /// <summary>
        /// Fetches cities for best-deals.
        /// </summary>
        /// <returns>An array of cities.</returns>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">The request was not successful.</exception>
        Task<BestDealsCity[]> GetCitiesForBestDealsAsync();
        
        /// <summary>
        /// Fetches best deals for the given city and currency.
        /// <remarks>Supports pagination.</remarks>
        /// </summary>
        /// <param name="departureCity">The best-deals city from which the flights begin.</param>
        /// <param name="currency">The currency in which the fares should be presented.</param>
        /// <param name="page">The page of deals (each contains at most 10 elements, set to 0 by default).</param>
        /// <returns>An array of best deals.</returns>
        /// <exception cref="T:Pegasustan.Utils.PegasusException">The request was not successful.</exception>
        Task<BestDeal[]> GetBestDealsAsync(BestDealsCity departureCity, Currency currency, uint page = 0U);
    }
}
