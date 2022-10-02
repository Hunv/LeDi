using Newtonsoft.Json;
using LeDi.Shared.DtoModel;
using LeDi.Shared;

namespace LeDi.Server.Api
{
    public static class ApiRule
    {
        /// <summary>
        /// Gets the possbile fields and fieldtypes for the rules
        /// </summary>
        /// <returns></returns>
        public static string GetRuleFields()
        {
            var fields = new Dictionary<string, string>();
            fields.Add("gamename", "string");

            fields.Add("halftime_count", "int");
            fields.Add("halftime_length", "int");
            fields.Add("halftime_overtime", "bool");
            fields.Add("halftime_last_pause_time_on_event", "bool");
            fields.Add("halftime_last_pause_time_on_event_seconds", "int");

            fields.Add("match_extension_on_draw", "bool");

            fields.Add("foul_penaltytime_1", "bool");
            fields.Add("foul_penaltytime_1_seconds", "int");
            fields.Add("foul_penaltytime_2", "bool");
            fields.Add("foul_penaltytime_2_seconds", "int");
            fields.Add("foul_penaltytime_3", "bool");
            fields.Add("foul_penaltytime_3_seconds", "int");
            fields.Add("foul_warning_1", "bool");
            fields.Add("foul_warning_1_alias", "string");
            fields.Add("foul_warning_2", "bool");
            fields.Add("foul_warning_2_alias", "string");
            fields.Add("foul_warning_3", "bool");
            fields.Add("foul_warning_3_alias", "string");

            return JsonConvert.SerializeObject(fields, Helper.GetJsonSerializer());
        }

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
