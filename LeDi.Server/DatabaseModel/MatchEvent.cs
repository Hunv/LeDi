using System.ComponentModel.DataAnnotations;
using LeDi.Server.Classes;
using LeDi.Shared.DtoModel;
using LeDi.Shared.Enum;


namespace LeDi.Server.DatabaseModel
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
        public Match? Match { get; set; }

        /// <summary>
        /// The MatchId this Event belongs to
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// The source of the log (system or referee)
        /// </summary>
        public string? Source { get; set; }


        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoMatchEvent ToDto()
        {
            var dto = new DtoMatchEvent()
            {
                Id = Id,
                Timestamp = Timestamp,
                Matchtime = Matchtime,
                Event = (int)Event,
                Text = Text,
                Source = Source
            };

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoMatchEvent dto)
        {
            Id = dto.Id;
            Timestamp = dto.Timestamp;
            Event = (MatchEventEnum)dto.Event;
            Text = dto.Text;
            Source = dto.Source;
        }
    }
}
