using System;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a best-deals city within Pegasus API.
    /// </summary>
    public class BestDealsCity
    {
        /// <summary>
        /// The city title.
        /// </summary>
        public string Title { get; }
        
        /// <summary>
        /// The city code.
        /// <remarks>It is a 3-letter code assigned by IATA.</remarks>
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.BestDealsCity" /> class.
        /// </summary>
        /// <param name="title">The city name.</param>
        /// <param name="code">The city code.</param>
        private BestDealsCity(string title, string code)
        {
            Title = title;
            Code = code;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a best-deals city to its <see cref="T:Pegasustan.Domain.BestDealsCity"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a best-deals city data to convert.</param>
        /// <returns>An object that is equivalent to the best-deals city data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid best-deals city data.</exception>
        public static BestDealsCity Parse(JsonNode node)
        {
            var title = (string)node["title"];
            var code = (string)node["code"];
            
            if (title is null || code is null)
            {
                throw new ArgumentException("JSON node does not provide proper best-deals city data.");
            }
            
            return new BestDealsCity(title, code);
        }
    }
}
