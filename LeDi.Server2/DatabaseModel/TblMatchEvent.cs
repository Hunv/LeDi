using System.ComponentModel.DataAnnotations;
using LeDi.Server2.Enum;


namespace LeDi.Server2.DatabaseModel
{
    public class TblMatchEvent
    {
        /// <summary>
        /// The ID of the match event
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Timestamp of the Match Event
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Number of seconds after the match started when this event occured
        /// </summary>
        public int Matchtime { get; set; }

        /// <summary>
        /// The Type of the Event
        /// </summary>
        public MatchEventEnum Event { get; set; }

        /// <summary>
        /// A Description of the Event
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// The match this event belongs to
        /// </summary>
        public TblMatch? Match { get; set; }

        /// <summary>
        /// The MatchId this Event belongs to
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// The source of the log (system or referee)
        /// </summary>
        public string? Source { get; set; }

    }
}
