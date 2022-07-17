using Newtonsoft.Json;

namespace Tiwaz.Server.Api.DtoModel
{
    public class DtoRule
    {
        /// <summary>
        /// The identifier name of this game.
        /// </summary>
        [JsonProperty("gamename")]
        public string? Gamename { get; set; }


        /// <summary>
        /// How many halftimes are played in a regular match (excluding extention)
        /// </summary>
        [JsonProperty("halftime_count")]
        public int HalftimeCount { get; set; }
        /// <summary>
        /// What is the regular length of a halftime
        /// </summary>
        [JsonProperty("halftime_length")]
        public int HalftimeLength { get; set;}
        /// <summary>
        /// Is there an overtime? The time will not automatically stop if the match time is over.
        /// </summary>
        [JsonProperty("overtime", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Overtime { get; set; }

        /// <summary>
        /// The time will stop on the set number of seconds before a match ends and a event occures (except goal and timeout). i.e. foul.
        /// </summary>
        [JsonProperty("halftime_laust_pause_time_on_event_seconds")]
        public int HalftimeLastPauseTimeOnEventSeconds { get; set; }


        /// <summary>
        /// Does the match will have an extension of the match is a draw after the regular match length?
        /// </summary>
        [JsonProperty("match_extension_on_draw", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool MatchExtensionOnDraw { get; set; }


        /// <summary>
        /// How many seconds a player will get a time penalty on first warning?
        /// 0 = no time penalty for players
        /// >0 = number of seconds of the penalty
        /// </summary>
        [JsonProperty("foul_penalty_time_1_seconds")]
        public int FoulPenaltyTime1Seconds { get; set; }

        /// <summary>
        /// How many seconds a player will get a time penalty on first warning?
        /// 0 = no time penalty for players
        /// >0 = number of seconds of the penalty        
        /// </summary>
        [JsonProperty("foul_penalty_time_2_seconds")]
        public int FoulPenaltyTime2Seconds { get; set; }

        /// <summary>
        /// How many seconds a player will get a time penalty on first warning?
        /// 0 = no time penalty for players
        /// >0 = number of seconds of the penalty        
        /// </summary>
        [JsonProperty("foul_penalty_time_3_seconds")]
        public int FoulPenaltyTime3Seconds { get; set; }

        /// <summary>
        /// The name of the first warning for players (i.e. yellow card)
        /// Leave empty if now warning exists
        /// </summary>
        [JsonProperty("foul_warning_1_alias")]
        public string? FoulWarning1 { get; set; }

        /// <summary>
        /// The name of the second warning for players (i.e. yellow card)
        /// Leave empty if now warning exists
        /// </summary>
        [JsonProperty("foul_warning_2_alias")]
        public string? FoulWarning2 { get; set; }

        /// <summary>
        /// The name of the thrid warning for players (i.e. red card)
        /// Leave empty if now warning exists
        /// </summary>
        [JsonProperty("foul_warning_3_alias")]
        public string? FoulWarning3 { get; set; }
    }
}
