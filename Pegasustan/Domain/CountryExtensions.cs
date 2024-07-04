using System;
using System.Collections.Generic;
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
        public static Country FindByCode(this IEnumerable<Country> countries, string code)
        {
            return countries.SingleOrDefault(country => 
                country.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.Port" /> by its country's and own code.
        /// </summary>
        /// <param name="countries">A source of countries.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="portCode">The port code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.Port" /> reference if it is found, otherwise default value - null.</returns>
        public static Port FindPortByCountryAndPortCode(this IEnumerable<Country> countries, string countryCode, string portCode)
        {
            var country = countries.FindByCode(countryCode);
            return country?.FindPortByCode(portCode);
        }
    }
}
