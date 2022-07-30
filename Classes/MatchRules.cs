using Newtonsoft.Json;
using System.Text.Json;
using Tiwaz.Server.Api.DtoModel;

namespace Tiwaz.Server.Classes
{
    /// <summary>
    /// Loads and handles the match rules from the match rule definition file
    /// </summary>
    public static class MatchRules
    {
        
        /// <summary>
        /// The rules of the loaded game type
        /// </summary>
        public static MatchRuleSet? Rules { get; set; }

        public async static Task LoadRules(string gameName)
        {
            System.IO.StreamReader sR = new StreamReader(SystemSettings.RuleFilePath);
            var rulesJson = await sR.ReadToEndAsync();
            var ruleList = JsonConvert.DeserializeObject<DtoRuleBody>(rulesJson, Helper.GetJsonSerializer());

            Rules = ruleList.Rules.SingleOrDefault(x => x.GameName == gameName);
        }
    }
}
