using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeDi.Server2.DatabaseModel
{
    public class TblMatch
    {

        /// <summary>
        /// The ID of the match
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The List of the Players for Team 1
        /// </summary>
        public List<TblPlayer>? Team1Players { get; set; }

        /// <summary>
        /// The List of the Players for Team 2
        /// </summary>
        public List<TblPlayer>? Team2Players { get; set; }

        /// <summary>
        /// Score of Team1
        /// </summary>
        [Required]
        public int Team1Score { get; set; }

        /// <summary>
        /// Score of Team2
        /// </summary>
        [Required]
        public int Team2Score { get; set; }

        /// <summary>
        /// Name of Team1
        /// </summary>
        [Required]
        public string? Team1Name { get; set; }

        /// <summary>
        /// Name of Team2
        /// </summary>
        [Required]
        public string? Team2Name { get; set; }

        /// <summary>
        /// Time left in seconds
        /// </summary>
        [Required]
        public int CurrentTimeLeft { get; set; }

        /// <summary>
        /// The current period; 0 = match not started
        /// </summary>
        [Required]
        public int CurrentPeriod { get; set; }

        /// <summary>
        /// The Status of the Match
        /// </summary>
        public int MatchStatus { get; set; }

        /// <summary>
        /// The Scheduled time when the match should start
        /// </summary>
        public DateTime? ScheduledTime { get; set; }

        /// <summary>
        /// The timestamps of matchevents
        /// </summary>
        public ICollection<TblMatchEvent> MatchEvents { get; set; } = new List<TblMatchEvent>();

        /// <summary>
        /// The referees of the event
        /// </summary>
        public List<TblMatchReferee> MatchReferees { get; set; } = new List<TblMatchReferee>();

        /// <summary>
        /// The Penalties that were raised in this match
        /// </summary>
        public List<TblMatchPenalty> MatchPenalties { get; set; } = new List<TblMatchPenalty>();

        /// <summary>
        /// The Devices, that will show this match by default
        /// </summary>
        public List<TblDevice2Match> Devices { get; set; } = new List<TblDevice2Match>();

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
        public List<TblGameRulePenalty> RulePenaltyList { get; set; } = new List<TblGameRulePenalty>();

        /// <summary>
        /// The tournament this match belongs to. If not belongs to a tournament, set this to null.
        /// </summary>
        public TblTournament? Tournament { get; set; }



        // ########################################
        // ########################################
        // Some helper methods and properties, that do not contain data and are not present in the database
        // ########################################
        // ########################################

        /// <summary>
        /// Get the time left in full minutes. Mintes are always rounded to the next lower number (i.e. 4:59 Min left = 4 Min).
        /// </summary>
        /// <returns>Full minutes of time left</returns>
        public int GetTimeLeftMinutes()
        {
            return (int)(CurrentTimeLeft / 60);         
        }

        /// <summary>
        /// Get the time left in seconds without full minutes. i.e. 4:59 Min left = 59 Sec
        /// </summary>
        /// <returns>Seconds of time left without full minutes</returns>
        public int GetTimeLeftSeconds()
        {
            return (int)(CurrentTimeLeft % 60);
        }

        /// <summary>
        /// Returns the number of seconds played since the start of the match (without overtime).
        /// </summary>
        /// <returns></returns>
        public int GetMatchTime()
        {
            var timeSinceStart = RulePeriodLength - CurrentTimeLeft + (CurrentPeriod - 1) * RulePeriodLength;
            if (timeSinceStart < 0)
                timeSinceStart = 0;

            return timeSinceStart;
        }
    }
}
