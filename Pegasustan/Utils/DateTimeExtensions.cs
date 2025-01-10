using System;

namespace Pegasustan.Utils
{
    /// <summary>
    /// Provides <see cref="T:System.DateTime"/> extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a <see cref="T:System.DateTime"/> to a <see cref="T:Pegasustan.Utils.YearMonth" />.
        /// </summary>
        /// <returns>A new <see cref="T:Pegasustan.Utils.YearMonth" /> object containing the year and the month.</returns>
        public static YearMonth ToYearMonth(this DateTime dateTime)
        {
            return new YearMonth(dateTime.Year, dateTime.Month);
        }
    }
}
