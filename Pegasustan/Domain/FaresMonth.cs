using System;
using System.Linq;
using System.Text.Json.Nodes;
using Pegasustan.Utils;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a fares month within Pegasus API.
    /// </summary>
    public class FaresMonth
    {
        /// <summary>
        /// The departure port.
        /// </summary>
        public Port DeparturePort { get; }
        
        /// <summary>
        /// The arrival port.
        /// </summary>
        public Port ArrivalPort { get; }
        
        /// <summary>
        /// The year and the month.
        /// </summary>
        public YearMonth YearMonth { get; }
        
        /// <summary>
        /// The flights.
        /// </summary>
        public Flight[] Flights { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.FaresMonth" /> class.
        /// </summary>
        /// <param name="departurePort">The departure port.</param>
        /// <param name="arrivalPort">The arrival port.</param>
        /// <param name="yearMonth">The year and the month.</param>
        /// <param name="flights">The flights.</param>
        private FaresMonth(Port departurePort, Port arrivalPort, YearMonth yearMonth, Flight[] flights)
        {
            DeparturePort = departurePort;
            ArrivalPort = arrivalPort;
            YearMonth = yearMonth;
            Flights = flights;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a fares month to its <see cref="T:Pegasustan.Domain.FaresMonth"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a fares month data to convert.</param>
        /// <param name="departurePort">The departure port.</param>
        /// <param name="arrivalPort">The arrival port.</param>
        /// <param name="currency">The currency in which the fares are provided.</param>
        /// <returns>An object that is equivalent to the fares month data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid fares month data.</exception>
        public static FaresMonth Parse(JsonNode node, Port departurePort, Port arrivalPort, Currency currency)
        {
            var departurePortCode = (string)node["depPort"];
            var arrivalPortCode = (string)node["arrPort"];
            YearMonth.TryParse((string)node["month"] ?? string.Empty, out var yearMonth);
            
            if (departurePortCode is null || arrivalPortCode is null || yearMonth is null)
            {
                throw new ArgumentException("JSON node does not provide proper fares month data.");
            }
            
            var flightsNode = node["days"]?.AsArray();

            if (flightsNode is null)
            {
                throw new ArgumentException("JSON node does not provide proper fares month data.");
            }
            
            var flights = flightsNode
                .Select(flightNode => Flight.Parse(flightNode, currency))
                .Where(f => !f.Equals(Flight.NoFlight))
                .ToArray();

            var faresMonth = new FaresMonth(departurePort, arrivalPort, yearMonth, flights);
            return faresMonth;
        }
    }
}
