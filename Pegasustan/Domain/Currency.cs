using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a currency compatible with Pegasus API.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Turkish lira (TRY).
        /// </summary>
        public static Currency Lira { get; } = new Currency("TRY", true);

        /// <summary>
        /// The currency code.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// Does support cheapest-fare requests.
        /// </summary>
        public bool SupportsCheapestFare { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Currency" /> class.
        /// </summary>
        /// <param name="code">The currency code.</param>
        /// <param name="supportsCheapestFare">Does support cheapest-fare requests.</param>
        public Currency(string code, bool supportsCheapestFare)
        {
            Code = code;
            SupportsCheapestFare = supportsCheapestFare;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a language to its <see cref="T:Pegasustan.Domain.Currency"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a currency data to convert.</param>
        /// <param name="fareSupportingCurrencies">A collection of currencies supporting cheapest-fare requests.</param>
        /// <returns>An object that is equivalent to the currency data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid currency data.</exception>
        public static Currency Parse(JsonNode node, IEnumerable<string> fareSupportingCurrencies)
        {
            var code = (string)node.AsValue();

            if (code is null)
            {
                throw new ArgumentException("JSON node does not provide proper currency data.");
            }

            if (string.Equals(code, "TL", StringComparison.OrdinalIgnoreCase))
            {
                return Lira;
            }
            
            var currency = new Currency(code, fareSupportingCurrencies.Contains(code));
            return currency;
        }
    }
}
