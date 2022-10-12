using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared.DtoModel
{
    public class DtoRule
    {
        /// <summary>
        /// The name of the game (i.e. soccer or underwaterhockey)
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("gamename")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? GameName { get; set; }


        /// <summary>
        /// Number of "halftimes" in a match
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_count")]
        [Range(1, int.MaxValue, ErrorMessage = "The number of halftime must be greater than 0.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int HalftimeCount { get; set; }

        /// <summary>
        /// Length of the Halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_length")]
        [Range(10, int.MaxValue, ErrorMessage = "Length of a halftime must be larger at least 10 seconds.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int HalftimeLength { get; set; }

        /// <summary>
        /// Does a halftime have (optional) overtime?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_overtime")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HalftimeOvertime { get; set; }

        /// <summary>
        /// Does the time pauses in the last halftime in case an event happens, that breaks the game (except on goal)?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_last_pause_time_on_event")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HalftimeLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If the time pauses at the end of the last halftime, how much seconds until the end of the last halftime this rule applies?
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("halftime_last_pause_time_on_event_last_seconds")]
        [Range(0, int.MaxValue, ErrorMessage = "The time the time pauses before the end of the match must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int HalftimeLastPauseTimeOnEventSeconds { get; set; }



        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("match_extension_on_draw")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool MatchExtensionOnDraw { get; set; }



        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulPenaltyTime1 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_1_seconds")]
        [Range(0, int.MaxValue, ErrorMessage = "The time for the first penalty must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int FoulPenaltyTime1Seconds { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_2")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulPenaltyTime2 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_2_seconds")]
        [Range(0, int.MaxValue, ErrorMessage = "The time for the second penalty must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int FoulPenaltyTime2Seconds { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_3")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulPenaltyTime3 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_penaltytime_3_seconds")]
        [Range(0, int.MaxValue, ErrorMessage = "The time for the third penalty must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int FoulPenaltyTime3Seconds { get; set; }


        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_1")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulWarning1 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_1_alias")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? FoulWarning1Alias { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_2")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulWarning2 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_2_alias")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? FoulWarning2Alias { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_3")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool FoulWarning3 { get; set; }

        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("foul_warning_3_alias")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? FoulWarning3Alias { get; set; }
    }
}
