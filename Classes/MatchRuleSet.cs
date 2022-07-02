namespace Tiwaz.Server.Classes
{
    public class MatchRuleSet
    {
        /// <summary>
        /// The name of the game (i.e. soccer or underwaterhockey)
        /// </summary>
        public string? GameName { get; set; }

        /// <summary>
        /// The list of rules for this game
        /// </summary>
        /// 
        [System.Text.Json.Serialization.JsonPropertyName("halftime_count")]
        public int HalftimeCount { get; set; }


        /// <summary>
        /// The list of rules for this game
        /// </summary>
        /// 
        [System.Text.Json.Serialization.JsonPropertyName("halftime_lenght")]
        public int HalftimeLenght { get; set; }


        /// <summary>
        /// The list of rules for this game
        /// </summary>
        /// 
        [System.Text.Json.Serialization.JsonPropertyName("halftime_overtime")]
        public bool HalftimeOvertime { get; set; }


        /// <summary>
        /// The list of rules for this game
        /// </summary>
        /// 
        [System.Text.Json.Serialization.JsonPropertyName("match_extension_on_draw")]
        public bool MatchExtensionOnDraw { get; set; }
        
    }
}
