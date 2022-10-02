using Newtonsoft.Json;
using LeDi.Shared.DtoModel;
using LeDi.Shared;

namespace LeDi.Server.Classes
{
    /// <summary>
    /// Loads and handles the match rules from the match rule definition file
    /// </summary>
    public static class MatchRules
    {

        /// <summary>
        /// The rules of the loaded game type
        /// </summary>
        public static MatchRuleSet Rules { get; set; } = new MatchRuleSet();

        /// <summary>
        /// Load the rules for a gametype from rule definition file
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public async static Task LoadRules(string gameName)
        {
            StreamReader sR = new(SystemSettings.RuleFilePath);
            var rulesJson = await sR.ReadToEndAsync();
            var ruleList = JsonConvert.DeserializeObject<DtoRuleBody>(rulesJson, Helper.GetJsonSerializer());

            if (ruleList != null && ruleList.Rules != null)
            {
                var rl = ruleList.Rules.SingleOrDefault(x => x.GameName == gameName);
                if (rl == null)
                    return;

                Rules.FoulPenaltyTime1 = rl.FoulPenaltyTime1;
                Rules.FoulPenaltyTime1Seconds = rl.FoulPenaltyTime1Seconds;
                Rules.FoulPenaltyTime2 = rl.FoulPenaltyTime2;
                Rules.FoulPenaltyTime2Seconds = rl.FoulPenaltyTime2Seconds;
                Rules.FoulPenaltyTime3 = rl.FoulPenaltyTime3;
                Rules.FoulPenaltyTime3Seconds = rl.FoulPenaltyTime3Seconds;
                Rules.FoulWarning1 = rl.FoulWarning1;
                Rules.FoulWarning1Alias = rl.FoulWarning1Alias;
                Rules.FoulWarning2 = rl.FoulWarning2;
                Rules.FoulWarning2Alias = rl.FoulWarning2Alias;
                Rules.FoulWarning3 = rl.FoulWarning3;
                Rules.FoulWarning3Alias = rl.FoulWarning3Alias;
                Rules.GameName = rl.GameName;
                Rules.HalftimeCount = rl.HalftimeCount;
                Rules.HalftimeLastPauseTimeOnEvent = rl.HalftimeLastPauseTimeOnEvent;
                Rules.HalftimeLastPauseTimeOnEventSeconds = rl.HalftimeLastPauseTimeOnEventSeconds;
                Rules.HalftimeLength = rl.HalftimeLength;
                Rules.HalftimeOvertime = rl.HalftimeOvertime;
                Rules.MatchExtensionOnDraw = rl.MatchExtensionOnDraw;
            }
        }
    }
}
