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
        /// The country GUID.
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// The data language GUID.
        /// </summary>
        public Guid LanguageId { get; }
        
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
        /// The country entry creation date.
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        /// The country entry modification date.
        /// </summary>
        public DateTime ModifiedAt { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Country" /> class.
        /// </summary>
        /// <param name="id">The country GUID.</param>
        /// <param name="languageId">The data language GUID.</param>
        /// <param name="name">The country name.</param>
        /// <param name="code">The country code.</param>
        /// <param name="ports">The country ports.</param>
        /// <param name="createdAt">The country entry creation date.</param>
        /// <param name="modifiedAt">The country entry modification date.</param>
        private Country(Guid id, Guid languageId, string name, string code, Port[] ports, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            LanguageId = languageId;
            Name = name;
            Code = code;
            Ports = ports;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a country to its <see cref="T:Pegasustan.Domain.Country"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a country data to convert.</param>
        /// <returns>An object that is equivalent to the country data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid country data.</exception>
        public static Country Parse(JsonNode node)
        {
            Guid.TryParse((string)node["id"] ?? string.Empty, out var id);
            Guid.TryParse((string)node["languageId"] ?? string.Empty, out var languageId);
            var name = (string)node["countryName"];
            var code = (string)node["countryCode"];
            DateTime.TryParse((string)node["createdDate"] ?? string.Empty, out var createdAt);
            DateTime.TryParse((string)node["modifiedDate"] ?? string.Empty, out var modifiedAt);
            
            if (id == Guid.Empty || languageId == Guid.Empty || name is null || code is null || createdAt == DateTime.MinValue || modifiedAt == DateTime.MinValue)
            {
                throw new ArgumentException("JSON node does not provide proper country data.");
            }

            var country = new Country(id, languageId, name, code, Array.Empty<Port>(), createdAt, modifiedAt);
            var portsNode = node["portMatrixPorts"]?.AsArray();

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
