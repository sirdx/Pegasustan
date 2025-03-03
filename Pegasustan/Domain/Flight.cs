using System;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a flight within Pegasus API.
    /// </summary>
    public class Flight
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
        /// The flight fare currency.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Creates a new empty instance of the <see cref="T:Pegasustan.Domain.Flight" /> class.
        /// </summary>
        private Flight()
        {
            
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Flight" /> class.
        /// </summary>
        /// <param name="date">The flight date.</param>
        /// <param name="campaignFare">Is the flight fare campaign.</param>
        /// <param name="amount">The flight fare amount.</param>
        /// <param name="currency">The flight fare currency.</param>
        private Flight(DateTime date, bool campaignFare, decimal amount, Currency currency)
        {
            Date = date;
            CampaignFare = campaignFare;
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a flight to its <see cref="T:Pegasustan.Domain.Flight"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a flight data to convert.</param>
        /// <param name="currency">The currency in which the fare is provided.</param>
        /// <returns>An object that is equivalent to the flight data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid flight data.</exception>
        public static Flight Parse(JsonNode node, Currency currency)
        {
            var flightMessage = node["availFlightMessage"];
                    
            if (flightMessage != null && (string)flightMessage == "NO_FARE")
            {
                return NoFlight;
            }
                    
            DateTime.TryParse((string)node["flightDate"] ?? string.Empty, out var date);
            var campaignFare = (bool)node["campaignFare"];
            var cheapFareNode = node["cheapFare"];

            if (cheapFareNode is null || date == DateTime.MinValue)
            {
                throw new ArgumentException("JSON node does not provide proper flight data.");
            }
            
            var amount = (decimal)cheapFareNode["amount"];

            var flight = new Flight(date, campaignFare, amount, currency);
            return flight;
        }
    }
}
