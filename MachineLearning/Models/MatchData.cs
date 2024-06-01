using DataHandling.Models;
using System.Collections.Generic;

namespace MachineLearning.Models
{
    public class MatchData
    {
        public List<BookmakerOdds> OtherBookmakersOdds { get; set; } // List of odds from various bookmakers
        public MatchDetails MatchDetails { get; set; } // Match information
        public float HomeTeamInjurySuspensionImpact { get; set; } // Impact of injuries and suspensions for home team
        public float AwayTeamInjurySuspensionImpact { get; set; } // Impact of injuries and suspensions for away team
        public float BetfairHomeWinOdds { get; set; } // Betfair odds for home win
        public float BetfairDrawOdds { get; set; } // Betfair odds for draw
        public float BetfairAwayWinOdds { get; set; } // Betfair odds for away win
        public MatchDataPrediction Prediction { get; set; } // Predicted odds
    }
}

