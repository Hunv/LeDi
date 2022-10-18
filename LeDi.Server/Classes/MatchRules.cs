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
                var rl = ruleList.Rules.SingleOrDefault(x => x.Gamename == gameName);
                if (rl == null)
                    return;
                                
                Rules.GameName = rl.Gamename;
                Rules.RulePeriodCount = rl.RulePeriodCount ?? 2;
                Rules.RulePeriodLastPauseTimeOnEvent = rl.RulePeriodLastPauseTimeOnEvent;
                Rules.RulePeriodLastPauseTimeOnEventSeconds = rl.RulePeriodLastPauseTimeOnEventSeconds ?? 0;
                Rules.RulePeriodLength = rl.RulePeriodLength ?? 10;
                Rules.RulePeriodOvertime = rl.RulePeriodOvertime;
                Rules.RuleMatchExtensionOnDraw = rl.RuleMatchExtensionOnDraw;

                Rules.RulePenaltyList = new List<MatchRuleSetPenalty>();
                foreach(var aDtoPenalty in rl.RulePenaltyList)
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

                    Rules.RulePenaltyList.Add(aPenalty);
                }
            }
        }
    }
}
