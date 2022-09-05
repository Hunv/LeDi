using Tiwaz.Shared.Enum;

namespace Tiwaz.Server.Classes
{
    /// <summary>
    /// This class is for extenting the MatchEventEnum to have more details about the events
    /// </summary>
    public class MatchEventItem
    {
        /// <summary>
        /// The Event Enum this EventItem belongs to
        /// </summary>
        public MatchEventEnum EventName { get; set; }

        /// <summary>
        /// Stops the match time when occuring
        /// </summary>
        public bool StopTime { get; set; }

        /// <summary>
        /// Start/Continue match time when occuring
        /// </summary>
        public bool StartTime { get; set; }

        /// <summary>
        /// Stops the time when occuring at the end of game, if that gamename uses this
        /// </summary>
        public bool StopTimeOnMatchEndEvents { get; set; }

        /// <summary>
        /// This event indicates that a match ended
        /// </summary>
        public bool EndsMatch { get; set; }

        /// <summary>
        /// The timestamp when this event happend
        /// </summary>
        public DateTime TimeOfEvent { get; set; }

        /// <summary>
        /// A comment to the event
        /// </summary>
        public string? EventComment { get; set; }
    }
}
