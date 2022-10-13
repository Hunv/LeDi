namespace LeDi.Server.Classes
{
    public class MatchRuleSetPenalty
    {
        /// <summary>
        /// Internal ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the Penalty
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The number of seconds a player or team has for the penalty
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("penalty_seconds")]
        public int PenaltySeconds { get; set; }

        /// <summary>
        /// Is this penalty a total dismissal of a player or team?
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("total_dismissal")] 
        public bool TotalDismissal { get; set; }

        /// <summary>
        /// The display text for this penalty
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("display")]
        public List<MatchRuleSetDisplayText> Display { get; set; } = new List<MatchRuleSetDisplayText>();
    }
}