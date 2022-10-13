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
                                
                Rules.GameName = rl.GameName;
                Rules.HalftimeCount = rl.HalftimeCount ?? 2;
                Rules.HalftimeLastPauseTimeOnEvent = rl.HalftimeLastPauseTimeOnEvent;
                Rules.HalftimeLastPauseTimeOnEventSeconds = rl.HalftimeLastPauseTimeOnEventSeconds ?? 0;
                Rules.HalftimeLength = rl.HalftimeLength ?? 10;
                Rules.HalftimeOvertime = rl.HalftimeOvertime;
                Rules.MatchExtensionOnDraw = rl.MatchExtensionOnDraw;

                Rules.MatchPenaltyList = new List<MatchRuleSetPenalty>();
                foreach(var aDtoPenalty in rl.PenaltyList)
                {
                    var aPenalty = new MatchRuleSetPenalty();
                    aPenalty.Name = aDtoPenalty.Name;
                    aPenalty.TotalDismissal = aDtoPenalty.TotalDismissal;
                    aPenalty.PenaltySeconds = aDtoPenalty.PenaltySeconds;
                    
                    aPenalty.Display = new List<MatchRuleSetDisplayText>();
                    foreach(var aDtoDisplay in aDtoPenalty.Display)
                    {
                        var aDisplay = new MatchRuleSetDisplayText();
                        aDisplay.Text = aDtoDisplay.Text;
                        aDisplay.Language = aDtoDisplay.Language;
                        aPenalty.Display.Add(aDisplay);
                    }

                    Rules.MatchPenaltyList.Add(aPenalty);
                }
            }
        }
    }
}
