using System;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a port within Pegasus API.
    /// </summary>
    public struct Port
    {
        /// <summary>
        /// The port GUID.
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// The country GUID.
        /// </summary>
        public Guid CountryId { get; }
        
        /// <summary>
        /// The language GUID.
        /// </summary>
        public Guid LanguageId { get; }
        
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
        /// </summary>
        public string CityName { get; }
        
        /// <summary>
        /// Is the port domestic.
        /// <remarks>Since Pegasus is a Turkish airline, only Turkish ports are marked as domestic.</remarks>
        /// </summary>
        public bool Domestic { get; }
        
        /// <summary>
        /// The port filters.
        /// <remarks>They are used to quickly search for a port.</remarks>
        /// </summary>
        public string[] Filters { get; }
        
        /// <summary>
        /// The port entry creation date.
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        /// The port entry modification date.
        /// </summary>
        public DateTime ModifiedAt { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Port" /> struct.
        /// </summary>
        /// <param name="id">The port GUID.</param>
        /// <param name="countryId">The country GUID.</param>
        /// <param name="languageId">The language GUID.</param>
        /// <param name="name">The port name.</param>
        /// <param name="code">The port code.</param>
        /// <param name="cityName">The port city name.</param>
        /// <param name="domestic">Is the port domestic.</param>
        /// <param name="filters">The port filters.</param>
        /// <param name="createdAt">The port entry creation date.</param>
        /// <param name="modifiedAt">The port entry modification date.</param>
        private Port(Guid id, Guid countryId, Guid languageId, string name, string code, string cityName, bool domestic, string[] filters, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            CountryId = countryId;
            LanguageId = languageId;
            Name = name;
            Code = code;
            CityName = cityName;
            Domestic = domestic;
            Filters = filters;
            CreatedAt = createdAt;
            ModifiedAt = modifiedAt;
        }
        
        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a port to its <see cref="T:Pegasustan.Domain.Port"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a port data to convert.</param>
        /// <returns>An object that is equivalent to the port data contained in <paramref name="node"/>.</returns>
        public static Port Parse(JsonNode node)
        {
            var id = Guid.Parse((string)node["id"]);
            var countryId = Guid.Parse((string)node["portMatrixId"]);
            var languageId = Guid.Parse((string)node["languageId"]);
            var name = (string)node["portName"];
            var code = (string)node["portCode"];
            var cityName = (string)node["cityName"];
            var domestic = (bool)node["domestic"];
            var portFilters = node["filter"].AsArray().GetValues<string>().ToArray();
            var createdAt = DateTime.Parse((string)node["createdDate"]);
            var modifiedAt = DateTime.Parse((string)node["modifiedDate"]);

            var port = new Port(id, countryId, languageId, name, code, cityName, domestic, portFilters, createdAt, modifiedAt);
            return port;
        }
    }
}
