namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a currency compatible with Pegasus API.
    /// </summary>
    public struct Currency
    {
        /// <summary>
        /// Turkish lira (TRY).
        /// </summary>
        public static Currency Lira { get; } = new Currency("TRY", '\u20ba');
        
        /// <summary>
        /// United States dollar (USD).
        /// </summary>
        public static Currency Dollar { get; } = new Currency( "USD", '$');
        
        /// <summary>
        /// Euro (EUR).
        /// </summary>
        public static Currency Euro { get; } = new Currency("EUR", '\u20ac');
        
        /// <summary>
        /// British pound sterling (GBP).
        /// </summary>
        public static Currency Pound { get; } = new Currency("GBP", '\u00a3');

        /// <summary>
        /// The currency code.
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// The currency symbol.
        /// </summary>
        public char Symbol { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.Currency" /> struct.
        /// </summary>
        /// <param name="code">The currency code.</param>
        /// <param name="symbol">The currency symbol.</param>
        public Currency(string code, char symbol)
        {
            Code = code;
            Symbol = symbol;
        }
    }
}
