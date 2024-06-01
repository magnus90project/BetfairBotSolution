
public class PlayerStatus
{
    public string PlayerName { get; set; }
    public string Team { get; set; }
    public string Position { get; set; }
    public string Status { get; set; } // Injured, Suspended
    public int ImportanceRating { get; set; } // 0-10, where 10 is most important
    public int Age { get; set; }
    public int MatchesPlayed { get; set; }
    public int GoalsScored { get; set; }
    public int Assists { get; set; }
    public int MinutesPlayed { get; set; } // Total minutes played this season
    public int MatchesStarted { get; set; } // Number of matches started
    public int MatchesSubstituted { get; set; } // Number of matches played as a substitute
    public int MatchesPlayedAny { get; set; } // Total number of matches played
    public int MatchesNotPlayed { get; set; } // Total number of matches not played
}