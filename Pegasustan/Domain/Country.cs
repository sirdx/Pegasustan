using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a country within Pegasus API.
    /// </summary>
    public struct Country
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
        public Port[] Ports { get; }
        
        /// <summary>
        /// The country entry creation date.
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        /// The country entry modification date.
        /// </summary>
        public DateTime ModifiedAt { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Country" /> struct.
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
        public static Country Parse(JsonNode node)
        {
            var id = Guid.Parse((string)node["id"]);
            var languageId = Guid.Parse((string)node["languageId"]);
            var name = (string)node["countryName"];
            var code = (string)node["countryCode"];
            var createdAt = DateTime.Parse((string)node["createdDate"]);
            var modifiedAt = DateTime.Parse((string)node["modifiedDate"]);

            var portsNode = node["portMatrixPorts"].AsArray();
            var ports = portsNode.Select(Port.Parse).ToArray();

            var country = new Country(id, languageId, name, code, ports, createdAt, modifiedAt);
            return country;
        }
    }
}
