using System;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a flight within Pegasus API.
    /// </summary>
    public struct Flight
    {
        /// <summary>
        /// The "no flight" object.
        /// <remarks>It's used by the Pegasus API mainly to mark a no flight day.</remarks>
        /// </summary>
        public static Flight NoFlight { get; } = new Flight();
        
        /// <summary>
        /// The flight date.
        /// <remarks>Date only - no time provided.</remarks>
        /// </summary>
        public DateTime Date { get; }
        
        /// <summary>
        /// Is the flight fare campaign.
        /// <remarks>That is only a guess. An example have not been found yet.</remarks>
        /// </summary>
        public bool CampaignFare { get; }
        
        /// <summary>
        /// The flight fare amount provided in the currency.
        /// </summary>
        public decimal Amount { get; }
        
        /// <summary>
        /// The flight fare currency code.
        /// </summary>
        public string CurrencyCode { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Flight" /> struct.
        /// </summary>
        /// <param name="date">The flight date.</param>
        /// <param name="campaignFare">Is the flight fare campaign.</param>
        /// <param name="amount">The flight fare amount.</param>
        /// <param name="currencyCode">The flight fare currency code.</param>
        private Flight(DateTime date, bool campaignFare, decimal amount, string currencyCode)
        {
            Date = date;
            CampaignFare = campaignFare;
            Amount = amount;
            CurrencyCode = currencyCode;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a flight to its <see cref="T:Pegasustan.Domain.Flight"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a flight data to convert.</param>
        /// <returns>An object that is equivalent to the flight data contained in <paramref name="node"/>.</returns>
        public static Flight Parse(JsonNode node)
        {
            var flightMessage = node["availFlightMessage"];
                    
            if (flightMessage != null && (string)flightMessage == "NO_FLIGHT")
            {
                return NoFlight;
            }
                    
            var date = DateTime.Parse((string)node["flightDate"]);
            var campaignFare = (bool)node["campaignFare"];
            
            var cheapFareNode = node["cheapFare"];
            var amount = (decimal)cheapFareNode["amount"];
            var currencyCode = (string)cheapFareNode["currency"];

            var flight = new Flight(date, campaignFare, amount, currencyCode);
            return flight;
        }
    }
}
