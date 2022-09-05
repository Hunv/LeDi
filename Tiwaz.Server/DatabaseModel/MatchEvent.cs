﻿using System.ComponentModel.DataAnnotations;
using Tiwaz.Server.Classes;
using Tiwaz.Shared.Enum;


namespace Tiwaz.Server.DatabaseModel
{
    public class MatchEvent
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
        public Match? Match { get; set; }
    }
}
