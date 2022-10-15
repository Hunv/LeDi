using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared.DtoModel
{
    public class DtoRule
    {
        /// <summary>
        /// The name of the game (i.e. soccer or underwaterhockey)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Gamename { get; set; }


        /// <summary>
        /// Number of "halftimes" in a match
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "The number of halftime must be greater than 0.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RuleHalftimeCount { get; set; }

        /// <summary>
        /// Length of the Halftime
        /// </summary>
        [Range(10, int.MaxValue, ErrorMessage = "Length of a halftime must be at least 10 seconds.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RuleHalftimeLength { get; set; }

        /// <summary>
        /// Does a halftime have (optional) overtime?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RuleHalftimeOvertime { get; set; }

        /// <summary>
        /// Does the time pauses in the last halftime in case an event happens, that breaks the game (except on goal)?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RuleHalftimeLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If the time pauses at the end of the last halftime, how much seconds until the end of the last halftime this rule applies?
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "The time the time pauses before the end of the match must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RuleHalftimeLastPauseTimeOnEventSeconds { get; set; }



        /// <summary>
        /// Does the match automatically extend if there is a draw at the end of the last regular halftime
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RuleMatchExtensionOnDraw { get; set; }

        /// <summary>
        /// List of penalties the type of game has by default
        /// </summary>
        public List<DtoRulePenalty> RulePenaltyList { get; set; } = new List<DtoRulePenalty>();

    }
}
