using Tiwaz.Server.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tiwaz.Server.Api.DtoModel;

namespace Tiwaz.Server.Api
{
    public static class ApiSetting
    {
        /// <summary>
        /// Get all settings
        /// </summary>
        public static string GetSetting()
        {
            using (var dbContext = new TwDbContext())
            {
                List<DtoSetting>? dto = dbContext.Settings.Select(x => x.ToDto()).ToList();

                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
        }

        /// <summary>
        /// Get a Setting
        /// </summary>
        public static string GetSetting(string setting)
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings.SingleOrDefault(x => x.SettingName == setting).ToDto();

                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
        }

        /// <summary>
        /// Set a Setting
        /// </summary>
        public static async Task SetSetting(string setting, string value)
        {
            using (var dbContext = new TwDbContext())
            {
                var tm = dbContext.Settings.SingleOrDefault(x => x.SettingName == setting);
                if (tm != null)
                {
                    tm.SettingValue = value;

                    await dbContext.SaveChangesAsync();
                }
            }
        }

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
