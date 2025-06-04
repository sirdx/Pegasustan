using System;
using System.Text.RegularExpressions;

namespace Pegasustan.Utils
{
    /// <summary>
    /// Represents a year and a month.
    /// </summary>
    public class YearMonth
    {
        /// <summary>
        /// The 'yyyy-MM' RegEx pattern.
        /// </summary>
        private const string Pattern = @"^[\d]{4}-(0[1-9]|1[0-2])$";
        
        /// <summary>
        /// Gets the year component.
        /// </summary>
        /// <returns>The year, between 0 and 9999.</returns>
        public int Year { get; }
        
        /// <summary>
        /// Gets the month component.
        /// </summary>
        /// <returns>The month component, expressed as a value between 1 and 12.</returns>
        public int Month { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Utils.YearMonth" /> class.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        public YearMonth(int year, int month)
        {
            if (year < 0 || year > 9999)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Year is out of range.");
            }

            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month is out of range.");
            }
            
            Year = year;
            Month = month;
        }

        /// <summary>
        /// Converts the string representation of a year and a month to its <see cref="T:Pegasustan.Utils.YearMonth"/> equivalent.
        /// </summary>
        /// <param name="text">A string in format 'yyyy-MM' that contains a year and a month to convert.</param>
        /// <returns>An object that is equivalent to the year and month contained in <paramref name="text"/>.</returns>
        /// <exception cref="ArgumentException">Invalid text format.</exception>
        public static YearMonth Parse(string text)
        {
            if (!Regex.IsMatch(text, Pattern))
            {
                throw new ArgumentException("The text is not in 'yyyy-MM' format.");
            }

            return ParseUnsafe(text.AsSpan());
        }

        /// <summary>
        /// Converts the string representation of a year and a month to its <see cref="T:Pegasustan.Utils.YearMonth"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string in format 'yyyy-MM' that contains a year and a month to convert.</param>
        /// <param name="yearMonth">When this method returns, contains the <see cref="T:Pegasustan.Utils.YearMonth" /> value equivalent to the year and month contained in <paramref name="text" />, if the conversion succeeded, or null if the conversion failed. The conversion fails if the <paramref name="text" /> parameter is not in the 'yyyy-MM' format. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true" /> if the <paramref name="text" /> parameter was converted successfully; otherwise, <see langword="false" />.</returns>
        public static bool TryParse(string text, out YearMonth yearMonth)
        {
            if (!Regex.IsMatch(text, Pattern))
            {
                yearMonth = null;
                return false;
            }
            
            yearMonth = ParseUnsafe(text.AsSpan());
            return true;
        }

        /// <summary>
        /// Converts the string representation of a year and a month to its <see cref="T:Pegasustan.Utils.YearMonth"/> equivalent without any checks.
        /// </summary>
        /// <param name="text">A string in format 'yyyy-MM' that contains a year and a month to convert.</param>
        /// <returns>An object that is equivalent to the year and month contained in <paramref name="text"/>.</returns>
        private static YearMonth ParseUnsafe(ReadOnlySpan<char> text)
        {
            var year = int.Parse(text.Slice(0, 4).ToString());
            var month = int.Parse(text.Slice(5, 2).ToString());
            return new YearMonth(year, month);
        }
    }
}