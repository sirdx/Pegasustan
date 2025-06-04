using System;
using System.Linq;
using System.Text.Json.Nodes;
using Pegasustan.Utils;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents one of the best deals within Pegasus API.
    /// </summary>
    public class BestDeal
    {
        /// <summary>
        /// The departure best-deals city.
        /// </summary>
        public BestDealsCity DepartureCity { get; }
        
        /// <summary>
        /// The arrival city name.
        /// </summary>
        public string ArrivalCityName { get; }
        
        /// <summary>
        /// The arrival port code.
        /// <remarks>It is a 3-letter code assigned by IATA.</remarks>
        /// </summary>
        public string ArrivalPortCode { get; }
        
        /// <summary>
        /// The flight fare amount provided in the currency.
        /// </summary>
        public decimal Amount { get; }
        
        /// <summary>
        /// The flight fare currency.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// The flight date (the first date of <c>Dates</c>).
        /// <remarks>Date only - no time provided.</remarks>
        /// </summary>
        public DateTime? Date => Dates.FirstOrDefault();
        
        /// <summary>
        /// The flight dates that are considered best-deal (including the <c>Date</c> one).
        /// <remarks>Dates only - no time provided.</remarks>
        /// </summary>
        public DateTime[] Dates { get; }
        
        /// <summary>
        /// The year and the month.
        /// </summary>
        public YearMonth YearMonth => Date?.ToYearMonth() ?? new YearMonth(0, 1);
        
        /// <summary>
        /// The image URL.
        /// </summary>
        public string ImageUrl { get; }
        
        /// <summary>
        /// Is the deal a promotion.
        /// </summary>
        public bool Promotion { get; }
        
        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.BestDeal" /> class.
        /// </summary>
        /// <param name="departureCity">The departure best-deals city.</param>
        /// <param name="currency">The flight fare currency.</param>
        /// <param name="arrivalCityName">The arrival city name.</param>
        /// <param name="arrivalPortCode">The arrival port code.</param>
        /// <param name="amount">The flight fare amount.</param>
        /// <param name="dates">The flight dates that are considered best-deal.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="promotion">Is the deal a promotion.</param>
        private BestDeal(
            BestDealsCity departureCity, 
            Currency currency, 
            string arrivalCityName, 
            string arrivalPortCode, 
            decimal amount, 
            DateTime[] dates, 
            string imageUrl, 
            bool promotion)
        {
            DepartureCity = departureCity;
            Currency = currency;
            ArrivalCityName = arrivalCityName;
            ArrivalPortCode = arrivalPortCode;
            Amount = amount;
            Dates = dates;
            ImageUrl = imageUrl;
            Promotion = promotion;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of one of the best deals to its <see cref="T:Pegasustan.Domain.BestDeal"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains one of the best deals data to convert.</param>
        /// <param name="departureCity">The departure best-deals city.</param>
        /// <param name="currency">The currency in which the fare is provided.</param>
        /// <returns>An object that is equivalent to one of the best deals data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid one of the best deals data.</exception>
        public static BestDeal Parse(JsonNode node, BestDealsCity departureCity, Currency currency)
        {
            var arrivalCityName = (string)node["arrCityName"];
            var arrivalPortCode = (string)node["arrPort"];
            var imageUrl = (string)node["imagePath"];
            var promotion = (bool)node["promotion"];
            
            if (arrivalCityName is null || arrivalPortCode is null || imageUrl is null)
            {
                throw new ArgumentException("JSON node does not provide proper one of the best deals data.");
            }
            
            var bestDealNode = node["bestDeal"];
            var dates = node["bestDealsDays"]?.AsArray();
            
            if (bestDealNode is null || dates is null)
            {
                throw new ArgumentException("JSON node does not provide proper one of the best deals data.");
            }
            
            var amount = (decimal)bestDealNode["amount"];
            
            var datesArr = dates.GetValues<string>().Select(DateTime.Parse).ToArray();
            return new BestDeal(departureCity, currency, arrivalCityName, arrivalPortCode, amount, datesArr, imageUrl, promotion);
        }
    }
}
