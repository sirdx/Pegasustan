using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Provides <see cref="T:Pegasustan.Domain.BestDealsCity"/> extensions.
    /// </summary>
    public static class BestDealsCityExtensions
    {
        /// <summary>
        /// Finds a <see cref="T:Pegasustan.Domain.BestDealsCity" /> by its code.
        /// </summary>
        /// <param name="bestDealsCities">A source of best-deals cities.</param>
        /// <param name="code">The city code.</param>
        /// <returns>A <see cref="T:Pegasustan.Domain.BestDealsCity" /> reference if it is found, otherwise default value - null.</returns>
        public static BestDealsCity FindByCode(this IEnumerable<BestDealsCity> bestDealsCities, string code)
        {
            return bestDealsCities.SingleOrDefault(city => 
                city.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }
    }
}
