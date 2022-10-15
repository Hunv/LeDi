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
        public string? Name { get; set; }

        /// <summary>
        /// The number of seconds a player or team has for the penalty. 0 = no Time Penalty, -1 = Until end of match
        /// </summary>
        public int PenaltySeconds { get; set; }

        /// <summary>
        /// Is this penalty a total dismissal of a player or team?
        /// </summary>
        public bool TotalDismissal { get; set; }

        /// <summary>
        /// The display text for this penalty
        /// </summary>

        public List<MatchRuleSetDisplayText> Display { get; set; } = new List<MatchRuleSetDisplayText>();
    }
}