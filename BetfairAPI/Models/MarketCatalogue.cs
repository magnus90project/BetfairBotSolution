// Models/MarketCatalogue.cs
using System;

namespace BetfairAPI.Models
{
    public class MarketCatalogue
    {
        public string MarketId { get; set; }
        public string MarketName { get; set; }
        public DateTime MarketStartTime { get; set; }
        public decimal TotalMatched { get; set; }
     
    }
}
