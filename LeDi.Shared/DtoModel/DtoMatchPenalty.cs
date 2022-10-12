using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoMatchPenalty
    {
        /// <summary>
        /// The unique Id for the Database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The PlayerId in case the player is maintained in database
        /// </summary>
        public int? PlayerId { get; set; }

        /// <summary>
        /// The Playernumber
        /// </summary>
        public int PlayerNumber { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public string? PlayerName { get; set; }

        /// <summary>
        /// A note for the penalty
        /// </summary>
        public string Note { get; set; } = "";

        /// <summary>
        /// The source of the penalty
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// The timestamp when the penalty occured
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The point of time the penalty was made. This are the seconds the game already runs.
        /// </summary>
        public int PenaltyTimeStart { get; set; }

        /// <summary>
        /// Number of seconds the penalty lasts. 0 if it has no ending.
        /// </summary>
        public int PenaltyTime { get; set; }

    }
}

