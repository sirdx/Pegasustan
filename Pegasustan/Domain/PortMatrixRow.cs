using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a port matrix row within Pegasus API.
    /// </summary>
    public class PortMatrixRow
    {
        /// <summary>
        /// The departure port.
        /// </summary>
        public PortMatrixItem Departure { get; }
        
        /// <summary>
        /// The arrival ports.
        /// </summary>
        public PortMatrixItem[] Arrivals { get; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.PortMatrixRow" /> class.
        /// </summary>
        /// <param name="departure">The departure port.</param>
        /// <param name="arrivals">The arrival ports.</param>
        private PortMatrixRow(PortMatrixItem departure, PortMatrixItem[] arrivals)
        {
            Departure = departure;
            Arrivals = arrivals;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a port to its <see cref="T:Pegasustan.Domain.PortMatrixRow"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a port matrix row data to convert.</param>
        /// <returns>An object that is equivalent to the port matrix row data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid port matrix row data.</exception>
        public static PortMatrixRow Parse(JsonNode node)
        {
            var departureNode = node["departure"];
            var arrivalsNode = node["arrivalList"]?.AsArray();
            
            if (departureNode is null || arrivalsNode is null)
            {
                throw new ArgumentException("JSON node does not provide proper port matrix row data.");
            }
            
            var departure = PortMatrixItem.Parse(departureNode);
            var arrivals = arrivalsNode.Select(PortMatrixItem.Parse).ToArray();
            
            return new PortMatrixRow(departure, arrivals);
        }
    }
}
