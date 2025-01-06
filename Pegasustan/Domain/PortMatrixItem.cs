using System;
using System.Text.Json.Nodes;

namespace Pegasustan.Domain
{
    /// <summary>
    /// Represents a port matrix item within Pegasus API.
    /// </summary>
    public class PortMatrixItem
    {
        /// <summary>
        /// The port name.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// The port code.
        /// <remarks>It is a 3-letter code assigned by IATA.</remarks>
        /// </summary>
        public string Code { get; }
        
        /// <summary>
        /// The port country name.
        /// </summary>
        public string CountryName { get; }
        
                
        /// <summary>
        /// The port country code.
        /// </summary>
        public string CountryCode { get; }
        
        /// <summary>
        /// The port city name.
        /// </summary>
        public string CityName { get; }
        
        /// <summary>
        /// The port city code.
        /// </summary>
        public string CityCode { get; }
        
        /// <summary>
        /// Is soldier student eligible.
        /// <remarks>No idea what it means, it is mostly false.</remarks>
        /// </summary>
        public bool EligibleSoldierStudent { get; }
        
        /// <summary>
        /// Are there multiple ports.
        /// <remarks>For example, IST_SAW is a multiple port.</remarks>
        /// </summary>
        public bool MultiplePort { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Pegasustan.Domain.PortMatrixItem" /> class.
        /// </summary>
        /// <param name="name">The port name.</param>
        /// <param name="code">The port code.</param>
        /// <param name="countryName">The port country name.</param>
        /// <param name="countryCode">The port country code.</param>
        /// <param name="cityName">The port city name.</param>
        /// <param name="cityCode">The port city code.</param>
        /// <param name="eligibleSoldierStudent">Is soldier student eligible.</param>
        /// <param name="multiplePort">Are there multiple ports.</param>
        private PortMatrixItem(string name, string code, string countryName, string countryCode, string cityName, string cityCode, bool eligibleSoldierStudent, bool multiplePort)
        {
            Name = name;
            Code = code;
            CountryName = countryName;
            CountryCode = countryCode;
            CityName = cityName;
            CityCode = cityCode;
            EligibleSoldierStudent = eligibleSoldierStudent;
            MultiplePort = multiplePort;
        }

        /// <summary>
        /// Converts the <see cref="T:System.Text.Json.Nodes.JsonNode"/> representation of a port to its <see cref="T:Pegasustan.Domain.PortMatrixItem"/> equivalent.
        /// </summary>
        /// <param name="node">A JSON node that contains a port matrix item data to convert.</param>
        /// <returns>An object that is equivalent to the port matrix item data contained in <paramref name="node"/>.</returns>
        /// <exception cref="ArgumentException">Passed JSON node does not represent a valid port matrix item data.</exception>
        public static PortMatrixItem Parse(JsonNode node)
        {
            var name = (string)node["portName"];
            var code = (string)node["portCode"];
            var countryName = (string)node["countryName"];
            var countryCode = (string)node["countryCode"];
            var cityName = (string)node["cityName"];
            var cityCode = (string)node["cityCode"];
            var eligibleSoldierStudent = (bool)node["eligibleSoldierStudent"];
            var multiplePort = (bool)node["multiplePort"];
            
            if (name is null || code is null || countryName is null || countryCode is null || cityName is null || cityCode is null)
            {
                throw new ArgumentException("JSON node does not provide proper port matrix item data.");
            }
            
            return new PortMatrixItem(name, code, countryName, countryCode, cityName, cityCode, eligibleSoldierStudent, multiplePort);
        }
    }
}
