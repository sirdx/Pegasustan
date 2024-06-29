using System;
using System.Linq;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Provides Country extensions.
    /// </summary>
    public static class CountryExtensions
    {
        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.Country" /> by its code.
        /// </summary>
        /// <param name="countries">A source of countries.</param>
        /// <param name="code">The country code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.Country" /> reference if it is found, otherwise default value - null.</returns>
        public static Country FindCountryByCode(this Country[] countries, string code)
        {
            return countries.SingleOrDefault(country => country.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
