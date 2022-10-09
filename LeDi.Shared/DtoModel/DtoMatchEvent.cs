using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoMatchEvent
    {
        /// <summary>
        /// The ID of the match event
        /// </summary>
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
        public int Event { get; set; }

        /// <summary>
        /// A Description of the Event
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// The source of this log (System or name of referee)
        /// </summary>
        public string? Source { get; set; }
    }
}
