using System.ComponentModel.DataAnnotations;

namespace LeDi.Server2.DatabaseModel
{
    public class TblTournament
    {
        /// <summary>
        /// The unique ID for the tournament
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Displayname for the tournament
        /// </summary>
        public string Name { get; set; } = "New Tournament";

        /// <summary>
        /// The Start date of the tournament
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// The End date of the tournament
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Today;

        /// <summary>
        /// The default name of the first team for this tournament
        /// </summary>
        public string DefaultTeam1Name { get; set; } = "Team1";

        /// <summary>
        /// The default name of the second team for this tournament
        /// </summary>
        public string DefaultTeam2Name { get; set; } = "Team2";

        /// <summary>
        /// List of matches of this Tournament
        /// </summary>
        public List<TblMatch> Matches { get; set; } = new List<TblMatch>();

        /// <summary>
        /// List of default devices, that should show the current status of a match
        /// </summary>
        public List<TblDevice2Tournament> Devices { get; set; } = new List<TblDevice2Tournament> { };



        // ################################
        // ################################
        // Properties, that are also in the TblGameRule table
        // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        

        /// <summary>
        /// The name of the Game
        /// </summary>
        public string Sport { get; set; } = string.Empty;

        /// <summary>
        /// how many periods this game type has?
        /// </summary>
        public int DefaultPeriodCount { get; set; } = 1;

        /// <summary>
        /// how long (seconds) are the periods of this game
        /// </summary>
        public int DefaultRulePeriodLength { get; set; } = 600;

        /// <summary>
        /// has this type of game an overtime or does it end exactly when the time is over?
        /// </summary>
        public bool DefaultRulePeriodOvertime { get; set; }

        /// <summary>
        /// Does the match continues with an addtional period if there is no winner at the end of the last regular period?
        /// </summary>
        public bool DefaultRuleMatchExtensionOnDraw { get; set; }

        /// <summary>
        /// does this type of game pauses if within the last RulePeriodLastPauseTimeOnEventSeconds seconds of the last period a break of the game happens (except on goals)?
        /// </summary>
        public bool DefaultRulePeriodLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If RulePeriodLastPauseTimeOnEvent is true, how many seconds before the last period of a match ends the time will be paused if a game is stopped because of fouls or other interruptions?
        /// </summary>
        public int DefaultRulePeriodLastPauseTimeOnEventSeconds { get; set; } = 120;

        /// <summary>
        /// List of penalties for this game type
        /// </summary>
        public List<TblGameRulePenalty> DefaultRulePenaltyList { get; set; } = new List<TblGameRulePenalty>();
    }
}
