using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Provides <see cref="T:Pegasustan.Domain.Currency"/> extensions.
    /// </summary>
    public static class CurrencyExtensions
    {
        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.Currency" /> by its code.
        /// </summary>
        /// <param name="currencies">A source of countries.</param>
        /// <param name="code">The currency code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.Currency" /> reference if it is found, otherwise default value - null.</returns>
        public static Currency FindByCode(this IEnumerable<Currency> currencies, string code)
        {
            return currencies.SingleOrDefault(currency => 
                currency.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
