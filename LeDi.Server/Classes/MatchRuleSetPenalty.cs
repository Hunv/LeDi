namespace LeDi.Server.Classes
{
    public class MatchRuleSetPenalty
    {
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string? Name { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("penalty_seconds")]
        public int PenaltySeconds { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("total_dismissal")] 
        public bool TotalDismissal { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("display")]
        public List<MatchRuleSetDisplayText> Display { get; set; } = new List<MatchRuleSetDisplayText>();
    }
}