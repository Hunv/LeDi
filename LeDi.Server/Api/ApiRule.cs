using Newtonsoft.Json;
using LeDi.Shared.DtoModel;
using LeDi.Shared;

namespace LeDi.Server.Api
{
    public static class ApiRule
    {
        /// <summary>
        /// Gets the rules of the gamerules.json file
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetRules()
        {
            System.IO.StreamReader sR = new StreamReader("gamerules.json");
            var rulesJson = await sR.ReadToEndAsync();
            var ruleList = JsonConvert.DeserializeObject<DtoRuleBody>(rulesJson, Helper.GetJsonSerializer());

            return JsonConvert.SerializeObject(ruleList, Helper.GetJsonSerializer());
        }
    }
}
