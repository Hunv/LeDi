using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tiwaz.Server.DatabaseModel
{
    public class Match
    {
        public Match(string team1Name, string team2Name) 
        {
            Team1Name = team1Name;
            Team2Name = team2Name;
        }

        /// <summary>
        /// The ID of the match
        /// </summary>
        [Key]
        public int Id { get; set; }


        /// <summary>
        /// The List of the Players for Team 1
        /// </summary>
        public Player[]? Team1Players { get; set; }

        /// <summary>
        /// The Ids of the Players for Team 1
        /// </summary>
        [NotMapped]
        public int[]? Team1PlayerIds { get; set; }

        /// <summary>
        /// The List of the Players for Team 2
        /// </summary>
        public Player[]? Team2Players { get; set; }

        /// <summary>
        /// The Ids of the Players for Team 2
        /// </summary>
        [NotMapped]
        public int[]? Team2PlayerIds { get; set; }

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
        public string Team1Name { get; set; }

        /// <summary>
        /// Name of Team2
        /// </summary>
        [Required]
        public string Team2Name { get; set; }

        /// <summary>
        /// Time left
        /// </summary>
        [Required]
        public int TimeLeftSeconds { get; set; }

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
        public MatchEvent[]? MatchEvents { get; set; }
    }
}
