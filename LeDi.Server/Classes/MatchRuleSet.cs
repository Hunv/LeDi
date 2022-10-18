namespace LeDi.Server.Classes
{
    public class MatchRuleSet
    {
        /// <summary>
        /// The name of the game (i.e. soccer or underwaterhockey)
        /// </summary>
        public string? GameName { get; set; }



        /// <summary>
        /// Number of "halftimes" in a match
        /// </summary>
        public int RulePeriodCount { get; set; }

        /// <summary>
        /// Length of the Halftime
        /// </summary>
        public int RulePeriodLength { get; set; }

        /// <summary>
        /// Does a halftime have (optional) overtime?
        /// </summary>
        public bool RulePeriodOvertime { get; set; }

        /// <summary>
        /// Does the time pauses in the last halftime in case an event happens, that breaks the game (except on goal)?
        /// </summary>
        public bool RulePeriodLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If the time pauses at the end of the last halftime, how much seconds until the end of the last halftime this rule applies?
        /// </summary>
        public int RulePeriodLastPauseTimeOnEventSeconds { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        public bool RuleMatchExtensionOnDraw { get; set; }


        /// <summary>
        /// The penalties that can be given in this game type to players and/or teams
        /// </summary>
        public List<MatchRuleSetPenalty> RulePenaltyList { get; set; } = new List<MatchRuleSetPenalty>();

    }
}
