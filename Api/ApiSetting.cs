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
        public static string? GetSetting()
        {
            using (var dbContext = new TwDbContext())
            {
                var sets = dbContext.Settings;
                if (sets != null)
                {
                    List<DtoSetting>? dto = sets.Select(x => x.ToDto()).ToList();

                    return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                }
            }
            return null;
        }

        /// <summary>
        /// Get a Setting
        /// </summary>
        public static string? GetSetting(string setting)
        {
            using (var dbContext = new TwDbContext())
            {
                var sets = dbContext.Settings;
                if (sets != null)
                {
                    var set = sets.SingleOrDefault(x => x.SettingName == setting);
                    if (set != null)
                    {
                        var dto = set.ToDto();

                        return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Set a Setting
        /// </summary>
        public static async Task SetSetting(string setting, string value)
        {
            using var dbContext = new TwDbContext();
            var set = dbContext.Settings;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.SettingName == setting);
                if (tm != null)
                {
                    tm.SettingValue = value;

                    await dbContext.SaveChangesAsync();
                }
            }
        }



        /// <summary>
        /// Get a Device Setting
        /// </summary>
        public static string? GetDeviceSetting(string deviceId, string setting)
        {
            using var dbContext = new TwDbContext();

            var sets = dbContext.DeviceSettings;
            if (sets != null)
            {
                var set = sets.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == setting);
                if (set != null)
                {
                    var dto = set.ToDto();

                    return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                }
            }
            return null;
        }

        /// <summary>
        /// Creates or sets a device Setting
        /// </summary>
        public static async Task SetDeviceSetting(string deviceId, string settingName, string settingValue)
        {
            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceSettings;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == settingName);
                if (tm != null)
                {
                    tm.SettingValue = settingValue;
                }
                else
                {
                    set.Add(new DeviceSetting(deviceId, settingName, settingValue));
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
