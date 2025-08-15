using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a port within Pegasus API.
    /// </summary>
    public class Port
    {
        /// <summary>
        /// The port country.
        /// </summary>
        public Country Country { get; }

        /// <summary>
        /// The port name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The port code.
        /// <remarks>It is a 3-letter code assigned by IATA.</remarks>
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// The port city name.
        /// <remarks>It sometimes does not contain any data.</remarks>
        /// </summary>
        public string CityName { get; } = null;
        
        /// <summary>
        /// Is the port domestic.
        /// <remarks>Since Pegasus is a Turkish airline, only Turkish ports are marked as domestic.</remarks>
        /// </summary>
        public bool Domestic { get; }

        /// <summary>
        /// Does the port have a direct flight from the departure port.
        /// <remarks>Only meaningful for arrival ports.</remarks>
        /// </summary>
        public bool IsDirectFlight { get; }
        
        /// <summary>
        /// The port keywords.
        /// <remarks>Keywords that are used to quickly search for a port.</remarks>
        /// </summary>
        public string[] Keywords { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Port" /> class.
        /// </summary>
        /// <param name="country">The port country.</param>
        /// <param name="name">The port name.</param>
        /// <param name="code">The port code.</param>
        /// <param name="cityName">The port city name.</param>
        /// <param name="domestic">Is the port domestic.</param>
        /// <param name="isDirectFlight">Does the port have a direct flight.</param>
        /// <param name="keywords">The port keywords.</param>
        private Port(Country country, string name, string code, string cityName, bool domestic, bool isDirectFlight, string[] keywords)
        {
            Country = country;
            Name = name;
            Code = code;
            CityName = cityName;
            Domestic = domestic;
            IsDirectFlight = isDirectFlight;
            Keywords = keywords;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a port to its <see cref="T:Pegasustan.Domain.Port"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a port data to convert.</param>
        /// <param name="country">The country in which the port is located.</param>
        /// <returns>An object that is equivalent to the port data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid port data.</exception>
        public static Port Parse(JsonNode node, Country country)
        {
            var name = (string)node["portName"];
            var code = (string)node["portCode"];
            var cityName = (string)node["cityName"];
            var domestic = (bool)node["domestic"];
            var isDirectFlight = (bool)node["isDirectFlight"];
            
            if (name is null || code is null)
            {
                throw new ArgumentException("JSON node does not provide proper port data.");
            }
            
            var portKeywords = node["keywords"]?.AsArray();

            if (portKeywords is null)
            {
                throw new ArgumentException("JSON node does not provide proper port data.");
            }
            
            var portKeywordsArr = portKeywords.GetValues<string>().ToArray();
            return new Port(country, name, code, cityName, domestic, isDirectFlight, portKeywordsArr);
        }
    }
}
