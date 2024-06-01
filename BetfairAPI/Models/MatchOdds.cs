// Models/MatchOdds.cs
using System;

namespace BetfairAPI.Models
{
    public class MatchOdds
    {
        public string MatchId { get; set; }
        public string MarketId { get; set; }
        public decimal Odds { get; set; }
        public DateTime Timestamp { get; set; }
    }
}