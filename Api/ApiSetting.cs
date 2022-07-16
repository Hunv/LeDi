using Tiwaz.Server.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tiwaz.Server.Api
{
    public static class ApiSetting
    {
        /// <summary>
        /// Get a Setting
        /// </summary>
        public static string GetSetting(string setting)
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings.SingleOrDefault(x => x.SettingName == setting);

                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
        }

        /// <summary>
        /// Get a clubs
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
    }
}
