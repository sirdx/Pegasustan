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
        /// The port GUID.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The language GUID.
        /// </summary>
        public Guid LanguageId { get; }

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
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Port" /> class.
        /// </summary>
        /// <param name="id">The port GUID.</param>
        /// <param name="country">The port country.</param>
        /// <param name="languageId">The language GUID.</param>
        /// <param name="name">The port name.</param>
        /// <param name="code">The port code.</param>
        /// <param name="cityName">The port city name.</param>
        /// <param name="domestic">Is the port domestic.</param>
        /// <param name="filters">The port filters.</param>
        /// <param name="createdAt">The port entry creation date.</param>
        /// <param name="modifiedAt">The port entry modification date.</param>
        private Port(Guid id, Guid languageId, Country country, string name, string code, string cityName, bool domestic, string[] filters, DateTime createdAt, DateTime modifiedAt)
        {
            Id = id;
            LanguageId = languageId;
            Country = country;
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
        /// <param name="country">The country in which the port is located.</param>
        /// <returns>An object that is equivalent to the port data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid port data.</exception>
        public static Port Parse(JsonNode node, Country country)
        {
            Guid.TryParse((string)node["id"] ?? string.Empty, out var id);
            Guid.TryParse((string)node["languageId"] ?? string.Empty, out var languageId);
            Guid.TryParse((string)node["portMatrixId"] ?? string.Empty, out var countryId);
            var name = (string)node["portName"];
            var code = (string)node["portCode"];
            var cityName = (string)node["cityName"];
            var domestic = (bool)node["domestic"];
            DateTime.TryParse((string)node["createdDate"] ?? string.Empty, out var createdAt);
            DateTime.TryParse((string)node["modifiedDate"] ?? string.Empty, out var modifiedAt);
            
            if (id == Guid.Empty || languageId == Guid.Empty || name is null || code is null || cityName is null || createdAt == DateTime.MinValue || modifiedAt == DateTime.MinValue)
            {
                throw new ArgumentException("JSON node does not provide proper port data.");
            }
            
            var portFilters = node["filter"]?.AsArray();

            if (portFilters is null)
            {
                throw new ArgumentException("JSON node does not provide proper port data.");
            }
            
            var portFiltersArr = portFilters.GetValues<string>().ToArray();
            var port = new Port(id, languageId, country, name, code, cityName, domestic, portFiltersArr, createdAt, modifiedAt);
            return port;
        }
    }
}
