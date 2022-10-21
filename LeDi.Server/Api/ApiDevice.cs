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
    public static class ApiDevice
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get all Devices
        /// </summary>
        public static string? GetDeviceList()
        {
            Logger.Trace("Executing GetDeviceList...");

            using var dbContext = new TwDbContext();

            if (dbContext.Device != null)
            {
                List<DtoDevice>? dto = dbContext.Device.Select(aDevice => aDevice.ToDto()).ToList();
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Trace("GetDeviceList returns following result: {0}", json);
                return json;
            }

            Logger.Debug("GetDeviceList returns an empty response.");
            return null;
        }

        /// <summary>
        /// Get a Device
        /// </summary>
        public static string? GetDevice(string deviceId)
        {
            Logger.Trace("Executing GetDevice...");

            using var dbContext = new TwDbContext();

            if (dbContext.Device != null)
            {
                var dev = dbContext.Device.SingleOrDefault(x => x.DeviceId == deviceId);

                if (dev == null)
                {
                    Logger.Warn("DeviceId {0} not found.", deviceId);
                    return null;
                }

                DtoDevice? dto = dev.ToDto();
                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Trace("GetDevice returns following result: {0}", json);
                return json;
            }

            Logger.Debug("GetDevice returns an empty response.");
            return null;
        }

        /// <summary>
        /// Gets all settings of a device
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static string? GetDeviceSettings(string deviceId)
        {
            Logger.Trace("Executing GetDeviceSettings...");

            using var dbContext = new TwDbContext();

            if (dbContext.DeviceSettings != null)
            {
                List<DtoDeviceSetting>? dto = dbContext.DeviceSettings.Select(aSet => aSet.ToDto()).ToList();

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Trace("GetDeviceSettings returns following result: {0}", json);
                return json;
            }

            Logger.Debug("GetDeviceSettings returns an empty response.");
            return null;
        }

        /// <summary>
        /// Get a Device Setting
        /// </summary>
        public static string? GetDeviceSetting(string deviceId, string setting)
        {
            Logger.Trace("Executing GetDeviceSetting...");

            using var dbContext = new TwDbContext();

            var sets = dbContext.DeviceSettings;
            if (sets != null)
            {
                var set = sets.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == setting);
                if (set != null)
                {
                    var dto = set.ToDto();

                    var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                    Logger.Trace("GetDeviceSetting returns following result: {0}", json);
                    return json;
                }
                else
                {
                    Logger.Warn("DeviceSetting {0} for device {1} not found.", setting, deviceId);
                }
            }

            Logger.Debug("GetDeviceSetting returns an empty response.");
            return null;
        }

        /// <summary>
        /// Creates or sets a device Setting
        /// </summary>
        public static async Task SetDeviceSetting(string deviceId, string settingName, string settingValue)
        {
            Logger.Trace("Executing SetDeviceSetting...");

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
                Logger.Debug("Device {0} got the setting {1} set to \"{2}\"", deviceId, settingName, settingValue);
            }
        }

        
        /// <summary>
        /// Deletes a device Setting
        /// </summary>
        public static async Task DeleteDeviceSetting(string deviceId, string settingName)
        {
            Logger.Trace("Executing DeleteDeviceSetting...");

            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceSettings;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.DeviceId == deviceId && x.SettingName == settingName);
                if (tm != null)
                {
                    set.Remove(tm);

                    await dbContext.SaveChangesAsync();
                    Logger.Debug("Device {0} got the setting {1} removed.", deviceId, settingName);
                }
                else
                {
                    Logger.Warn("Cannot find setting {0} for device {1}", settingName, deviceId);
                }
            }
        }

        /// <summary>
        /// Creates a new device for device setting configuration
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static async Task<string?> NewDevice(DtoDevice device)
        {
            Logger.Trace("Executing NewDevice...");

            using var dbContext = new TwDbContext();
            var dev = dbContext.Device;
            if (dev != null)
            {
                //Check if device ID exists
                if (!string.IsNullOrEmpty(device.DeviceId))
                {
                    if (dev.Any(x => x.DeviceId == device.DeviceId))
                    {
                        //DeviceID exists
                        Logger.Debug("Verified Device {0}", device.DeviceId);
                        var dtoExists = new DtoDevice(device.DeviceId, device.DeviceModel, device.DeviceType);

                        var json1 = JsonConvert.SerializeObject(dtoExists, Helper.GetJsonSerializer());
                        Logger.Debug("NewDevice returns the following result: {0}", json1);
                        return json1;
                    }
                }

                //Create a new device ID
                var devId = Guid.NewGuid().ToString();
                var newDevice = new Device(devId, device.DeviceModel, device.DeviceType);
                dev.Add(newDevice);

                // Add default settings
                if (dbContext.DeviceSettings != null)
                {
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "width", "1"));
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "height", "1"));
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "brightness", "50"));
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_toptobottom", "true"));
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_alternatingrows", "true"));
                    dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_firstledleft", "true"));
                }

                await dbContext.SaveChangesAsync();
                Logger.Info("Created new Device with GUID {0}, Type {1} and Model {2}", devId, device.DeviceType, device.DeviceModel);

                var dto = new DtoDevice(newDevice.DeviceId, newDevice.DeviceModel, newDevice.DeviceType);

                var json2 = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("NewDevice got the following result: {0}", json2);
                return json2;
            }

            Logger.Debug("NewDevice returns an empty response.");
            return null;
        }

        /// <summary>
        /// Deletes a device
        /// </summary>
        public static async Task DeleteDevice(string deviceId)
        {
            Logger.Trace("Executing DeleteDevice...");

            using var dbContext = new TwDbContext();

            // Delete the device settings
            var set = dbContext.DeviceSettings;
            if (set != null)
            {
                // Get all Settings of the device
                var tm = set.Where(x => x.DeviceId == deviceId);
                if (tm != null)
                {
                    // Delete all settings of the device
                    set.RemoveRange(tm);

                    await dbContext.SaveChangesAsync();
                    Logger.Debug("Deleted settings for device {0}", deviceId);
                }
            }

            // Delete the device
            var dev = dbContext.Device;
            if (dev != null)
            {
                // Get all Settings of the device
                var tm = dev.SingleOrDefault(x => x.DeviceId == deviceId);
                if (tm != null)
                {
                    // Delete all settings of the device
                    dev.Remove(tm);

                    await dbContext.SaveChangesAsync();
                    Logger.Info("Deleted device {0}", deviceId);
                }
            }
        }
    }
}
