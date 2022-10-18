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
        /// Number of periods in a match
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "The number of period must be greater than 0.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RulePeriodCount { get; set; }

        /// <summary>
        /// Length of the periods
        /// </summary>
        [Range(10, int.MaxValue, ErrorMessage = "Length of a period must be at least 10 seconds.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RulePeriodLength { get; set; }

        /// <summary>
        /// Does a periods have (optional) overtime?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RulePeriodOvertime { get; set; }

        /// <summary>
        /// Does the time pauses in the last period in case an event happens, that breaks the game (except on goal)?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool RulePeriodLastPauseTimeOnEvent { get; set; }

        /// <summary>
        /// If the time pauses at the end of the last period, how much seconds until the end of the last period this rule applies?
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "The time the time pauses before the end of the match must be 0 or positive.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public int? RulePeriodLastPauseTimeOnEventSeconds { get; set; }



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
