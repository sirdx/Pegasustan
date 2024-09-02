using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a country within Pegasus API.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// The country name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The country code.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// The country ports.
        /// </summary>
        public Port[] Ports { get; private set; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Country" /> class.
        /// </summary>
        /// <param name="name">The country name.</param>
        /// <param name="code">The country code.</param>
        /// <param name="ports">The country ports.</param>
        private Country(string name, string code, Port[] ports)
        {
            Name = name;
            Code = code;
            Ports = ports;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a country to its <see cref="T:Pegasustan.Domain.Country"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a country data to convert.</param>
        /// <returns>An object that is equivalent to the country data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid country data.</exception>
        public static Country Parse(JsonNode node)
        {
            var name = (string)node["countryName"];
            var code = (string)node["countryCode"];
            
            if (name is null || code is null)
            {
                throw new ArgumentException("JSON node does not provide proper country data.");
            }

            var country = new Country(name, code, Array.Empty<Port>());
            var portsNode = node["ports"]?.AsArray();

            if (portsNode is null)
            {
                throw new ArgumentException("JSON node does not provide proper country data.");
            }
            
            var ports = portsNode.Select(portNode => Port.Parse(portNode, country)).ToArray();
            country.Ports = ports;
            return country;
        }

        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.Port" /> by its code.
        /// </summary>
        /// <param name="code">The port code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.Port" /> reference if it is found, otherwise default value - null.</returns>
        public Port FindPortByCode(string code)
        {
            return Ports.SingleOrDefault(port => port.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
