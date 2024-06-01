// Models/MatchDetails.cs (tidigare MatchData)
using System;
using System.Collections.Generic;

namespace DataHandling.Models
{
    public class MatchDetails
    {
        public string MatchId { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public DateTime MatchDate { get; set; }
        public string League { get; set; }
        public string Country { get; set; }
        public string Stadium { get; set; }
        public List<PlayerStatus> InjuredPlayers { get; set; }
        public List<PlayerStatus> SuspendedPlayers { get; set; }
        public int CurrentSeasonMatchNumber { get; set; } // Current match number in the season
        public int TotalSeasonMatches { get; set; } // Total number of matches in the season
    }
}

