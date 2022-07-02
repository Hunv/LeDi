using System.Text.Json;

namespace Tiwaz.Server.Classes
{
    /// <summary>
    /// Loads and handles the match rules from the match rule definition file
    /// </summary>
    public static class MatchRules
    {
        /// <summary>
        /// Path the the game rule definition file
        /// </summary>
        private static readonly string ruleFilePath = "gamerules.json";
        
        /// <summary>
        /// The rules of the loaded game type
        /// </summary>
        public static MatchRuleSet? Rules { get; set; }

        public static void LoadRules(string gameName)
        {
            // Load config:
            List<MatchRuleSet> allRules = new();

            var jsonString = File.ReadAllText(ruleFilePath);
            allRules = JsonSerializer.Deserialize<List<MatchRuleSet>>(jsonString);

            Rules = allRules.SingleOrDefault(x => x.GameName == gameName);
        }
    }
}
