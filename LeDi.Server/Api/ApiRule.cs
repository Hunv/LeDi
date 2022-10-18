using Newtonsoft.Json;
using LeDi.Shared.DtoModel;
using LeDi.Shared;

namespace LeDi.Server.Api
{
    public static class ApiRule
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the rules of the gamerules.json file
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetRules()
        {
            Logger.Trace("GetRules executed...");

            StreamReader sR = new StreamReader("gamerules.json");
            var rulesJson = await sR.ReadToEndAsync();
            var ruleList = JsonConvert.DeserializeObject<DtoRuleBody>(rulesJson, Helper.GetJsonSerializer());

            var json = JsonConvert.SerializeObject(ruleList, Helper.GetJsonSerializer());
            Logger.Debug("GetRules returns the following result: {0}", json);
            return json;
        }
    }
}
