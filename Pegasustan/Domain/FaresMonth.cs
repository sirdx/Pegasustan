using System.Linq;
using System.Text.Json.Nodes;
using Pegasustan.Utils;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a fares month within Pegasus API.
    /// </summary>
    public struct FaresMonth
    {
        /// <summary>
        /// The departure port code.
        /// </summary>
        public string DeparturePortCode { get; }
        
        /// <summary>
        /// The arrival port code.
        /// </summary>
        public string ArrivalPortCode { get; }
        
        /// <summary>
        /// The year and the month.
        /// </summary>
        public YearMonth YearMonth { get; }
        
        /// <summary>
        /// The flights.
        /// </summary>
        public Flight[] Flights { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.FaresMonth" /> struct.
        /// </summary>
        /// <param name="departurePortCode">The departure port code.</param>
        /// <param name="arrivalPortCode">The arrival port code.</param>
        /// <param name="yearMonth">The year and the month.</param>
        /// <param name="flights">The flights.</param>
        private FaresMonth(string departurePortCode, string arrivalPortCode, YearMonth yearMonth, Flight[] flights)
        {
            DeparturePortCode = departurePortCode;
            ArrivalPortCode = arrivalPortCode;
            YearMonth = yearMonth;
            Flights = flights;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a fares month to its <see cref="T:Pegasustan.Domain.FaresMonth"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a fares month data to convert.</param>
        /// <returns>An object that is equivalent to the fares month data contained in <paramref name="node"/>.</returns>
        public static FaresMonth Parse(JsonNode node)
        {
            var departurePortCode = (string)node["depPort"];
            var arrivalPortCode = (string)node["arrPort"];
            var yearMonth = YearMonth.Parse((string)node["month"]);
            
            var flightsNode = node["days"].AsArray();
            var flights = flightsNode
                .Select(Flight.Parse)
                .Where(f => !f.Equals(Flight.NoFlight))
                .ToArray();

            var faresMonth = new FaresMonth(departurePortCode, arrivalPortCode, yearMonth, flights);
            return faresMonth;
        }
    }
}
