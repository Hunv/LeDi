using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblGameRule
    {
        /// <summary>
        /// Creates a new instance of TblGameRules
        /// </summary>
        public TblGameRule()
        {
        }

        /// <summary>
        /// Unique Id of Game Type
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The name of the Game
        /// </summary>
        public string Sport { get; set; } = string.Empty;

        /// <summary>
        /// how many periods this game type has?
        /// </summary>
        public int RulePeriodCount { get; set; }

        /// <summary>
        /// how long (seconds) are the periods of this game
        /// </summary>
        public int RulePeriodLength { get; set; }

        /// <summary>
        /// has this type of game an overtime or does it end exactly when the time is over?
        /// </summary>
        public bool RulePeriodOvertime { get; set; }

        /// <summary>
        /// Does the match continues with an addtional period if there is no winner at the end of the last regular period?
        /// </summary>
        public bool RuleMatchExtensionOnDraw { get; set; }

        /// <summary>
        /// does this type of game pauses if within the last RulePeriodLastPauseTimeOnEventSeconds seconds of the last period a break of the game happens (except on goals)?
        /// </summary>
        public bool RulePeriodLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If RulePeriodLastPauseTimeOnEvent is true, how many seconds before the last period of a match ends the time will be paused if a game is stopped because of fouls or other interruptions?
        /// </summary>
        public int RulePeriodLastPauseTimeOnEventSeconds { get; set; }

        /// <summary>
        /// List of penalties for this game type
        /// </summary>
        public List<TblGameRulePenalty> RulePenaltyList { get; set; } = new List<TblGameRulePenalty>();


    }
}
