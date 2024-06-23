using System;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a language compatible with Pegasus API.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// The language code.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// The language name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Language" /> class.
        /// </summary>
        /// <param name="code">The language code.</param>
        /// <param name="name">The language name.</param>
        public Language(string code, string name)
        {
            Code = code;
            Name = name;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a language to its <see cref="T:Pegasustan.Domain.Language"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a language data to convert.</param>
        /// <returns>An object that is equivalent to the language data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid language data.</exception>
        public static Language Parse(JsonNode node)
        {
            var code = (string)node["code"];
            var name = (string)node["name"];

            if (code is null || name is null)
            {
                throw new ArgumentException("JSON node does not provide proper language data.");
            }

            var language = new Language(code, name);
            return language;
        }
    }
}