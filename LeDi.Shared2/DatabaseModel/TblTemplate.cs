using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblTemplate
    {
        /// <summary>
        /// Creates a new instance of Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public TblTemplate()
        {
        }

        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>        
        public string TemplateName{ get; set; }


        /// <summary>
        /// How many periods a match has
        /// Example: 2 at soccer or underwaterhockey, 3 at basketball
        /// </summary>
        public int PeriodCount { get; set; }

        /// <summary>
        /// How long (in seconds) a period is
        /// Example: 27000 (45min * 60sec) in soccer, 900 (15min * 60sec) at underwaterhockey
        /// </summary>
        public int PeriodLength { get; set; }

        /// <summary>
        /// Does the template has an extension of the match on a draw?
        /// Example: If it is a KO-Modus of a tournament
        /// </summary>
        public bool HasExtension { get; set; }

        /// <summary>
        /// Does this templates as overtime (game continues after time is over until the referee stops the period)
        /// Example: true for soccer, false for underwaterhockey
        /// </summary>
        public bool HasOvertime { get; set; }

        /// <summary>
        /// Does the template pauses the time, if there is a break (except goal) PauseNearEndSeconds seconds to the end of the last period?
        /// Example: false for soccer, true for underwaterhockey
        /// </summary>
        public bool HasPauseNearEnd { get; set; }

        /// <summary>
        /// The number of seconds to the end of the last period the template pauses if HasPauseNearEnd is enabled
        /// Example: 120 for underwaterhockey
        /// </summary>
        public int PauseNearEndSeconds { get; set; }

        /// <summary>
        /// Maximum number of players for a team. 0 = unlimited
        /// Example: 15 for soccer, 10 for underwaterhockey
        /// </summary>
        public int MaxNumberOfTeamPlayers { get; set; }

        /// <summary>
        /// Maximum number of players for a team on the field. 0 = unlimited
        /// Example: 9 for soccer, 6 for underwaterhockey
        /// </summary>
        public int MaxNumberOfTeamPlayersOnField { get; set; }

    }
}
