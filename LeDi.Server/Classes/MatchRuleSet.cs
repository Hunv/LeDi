namespace LeDi.Server.Classes
{
    public class MatchRuleSet
    {
        /// <summary>
        /// The name of the game (i.e. soccer or underwaterhockey)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("gamename")]
        public string? GameName { get; set; }



        /// <summary>
        /// Number of "halftimes" in a match
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_count")]
        public int HalftimeCount { get; set; }

        /// <summary>
        /// Length of the Halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_length")]
        public int HalftimeLength { get; set; }

        /// <summary>
        /// Does a halftime have (optional) overtime?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_overtime")]
        public bool HalftimeOvertime { get; set; }

        /// <summary>
        /// Does the time pauses in the last halftime in case an event happens, that breaks the game (except on goal)?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_last_pause_time_on_event")]
        public bool HalftimeLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If the time pauses at the end of the last halftime, how much seconds until the end of the last halftime this rule applies?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_last_pause_time_on_event_last_seconds")]
        public int HalftimeLastPauseTimeOnEventSeconds { get; set; }



        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("match_extension_on_draw")]
        public bool MatchExtensionOnDraw { get; set; }


        /// <summary>
        /// The penalties that can be given in this game type to players and/or teams
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("penalties")]
        public List<MatchRuleSetPenalty> MatchPenaltyList { get; set; } = new List<MatchRuleSetPenalty>();

    }
}
