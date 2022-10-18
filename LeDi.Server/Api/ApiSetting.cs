using LeDi.Server.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeDi.Shared.DtoModel;
using LeDi.Shared;

namespace LeDi.Server.Api
{
    public static class ApiSetting
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get all settings
        /// </summary>
        public static string? GetSetting()
        {
            Logger.Trace("Executing GetSetting...");

            using (var dbContext = new TwDbContext())
            {
                var sets = dbContext.Settings;
                if (sets != null)
                {
                    List<DtoSetting>? dto = sets.Select(x => x.ToDto()).ToList();

                    var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                    Logger.Debug("GetSetting returns the following result: {0}", json);
                    return json;
                }
            }

            Logger.Debug("GetSetting returns an empty response.");
            return null;
        }

        /// <summary>
        /// Get a Setting
        /// </summary>
        public static string? GetSetting(string setting)
        {
            Logger.Trace("Executing GetSetting(setting)...");

            using (var dbContext = new TwDbContext())
            {
                var sets = dbContext.Settings;
                if (sets != null)
                {
                    var set = sets.SingleOrDefault(x => x.SettingName == setting);
                    if (set != null)
                    {
                        var dto = set.ToDto();

                        var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                        Logger.Debug("GetSetting(setting) with setting {0} returns the following result: {1}",setting, json);
                        return json;
                    }
                }
            }
            Logger.Debug("GetSetting(setting) with setting {0} returns an empty response.", setting);
            return null;
        }

        /// <summary>
        /// Set a Setting
        /// </summary>
        public static async Task SetSetting(string setting, string value)
        {
            Logger.Trace("Executing SetSetting...");

            using var dbContext = new TwDbContext();
            var set = dbContext.Settings;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.SettingName == setting);
                if (tm != null)
                {
                    tm.SettingValue = value;

                    await dbContext.SaveChangesAsync();
                    Logger.Debug("SetSetting for setting \"{0}\" with value \"{1}\" executed", setting, value);
                }
            }
        }
    }
}
