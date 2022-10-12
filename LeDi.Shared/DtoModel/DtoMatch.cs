using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeDi.Shared.DtoModel 
{
    public class DtoMatch : DtoRule
    {
        /// <summary>
        /// The ID of the match
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Scores of Team1
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? Team1Score { get; set; }

        /// <summary>
        /// Scores of Team2
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? Team2Score { get; set; }

        /// <summary>
        /// Name of Team1
        /// </summary>
        [MaxLength(256, ErrorMessage = "Der Name kann höchstens aus 256 Zeichen bestehen.")]
        [MinLength(2, ErrorMessage = "Der Name muss mindestens aus zwei Zeichen bestehen.")]
        [RegularExpression(@"^[a-zA-Z0-9\s-äüößÄÜÖ\.]*$", ErrorMessage = "Der Name kann nur aus Buchstaben, Zahlen, Bindestrichen, Punkten und Leerzeichen bestehen.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Team1Name { get; set; }

        /// <summary>
        /// Name of Team2
        /// </summary>
        [MaxLength(256, ErrorMessage = "Der Name kann höchstens aus 256 Zeichen bestehen.")]
        [MinLength(2, ErrorMessage = "Der Name muss mindestens aus zwei Zeichen bestehen.")]
        [RegularExpression(@"^[a-zA-Z0-9\s-äüößÄÜÖ\.]*$", ErrorMessage = "Der Name kann nur aus Buchstaben, Zahlen, Bindestrichen, Punkten und Leerzeichen bestehen.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Team2Name { get; set; }

        /// <summary>
        /// Time left
        /// </summary>
        [Range(0,int.MaxValue,ErrorMessage = "Die Zeit muss größer als 0 Sekunden sein.")]
        public int TimeLeftSeconds { get; set; }

        /// <summary>
        /// Only for Livematches: The current status of the Match (see EnumMatchStatus for ID resolution)
        /// </summary>
        public int MatchStatus { get; set; }

        /// <summary>
        /// The Scheduled time when the match should start
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ScheduledTime { get; set; }

        /// <summary>
        /// The PlayerIds of Team1
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<int>? Team1PlayerIds { get; set; }

        /// <summary>
        /// The PlayerIds of Team2
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<int>? Team2PlayerIds { get; set; }

        /// <summary>
        /// Number of Halftimes
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "The number of halftimes must be 1 or larger")]
        public int HalfTimeCount { get; set; }

        /// <summary>
        /// Current number of Halftime
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "The current halftime must be 0 or more")]
        public int HalfTimeCurrent { get; set; }

        /// <summary>
        /// List of referees for this match
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<DtoMatchReferee> Referees { get; set; } = new List<DtoMatchReferee>();
    }
}
