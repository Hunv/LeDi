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
                //Check if device ID was stated
                if (!string.IsNullOrEmpty(device.DeviceId))
                {
                    //Chec if device ID is known
                    if (dev.Any(x => x.DeviceId == device.DeviceId))
                    {
                        //DeviceID exists
                        Logger.Debug("Verified Device {0}", device.DeviceId);
                        var dtoExists = new DtoDevice(device.DeviceId, device.DeviceModel, device.DeviceType, device.DeviceName) 
                        {
                            Default = device.Default, Enabled = device.Enabled 
                        };

                        var json1 = JsonConvert.SerializeObject(dtoExists, Helper.GetJsonSerializer());
                        Logger.Debug("NewDevice returns the following result: {0}", json1);
                        return json1;
                    }
                }
                else
                {
                    //Create a new DeviceId if it was not stated
                    Logger.Info("Device registered without DeviceId, creating a new one.");
                    device.DeviceId = Guid.NewGuid().ToString();
                    Logger.Info("New DeviceId created: {0} ", device.DeviceId);
                }

                //Create a new device ID
                var devId = device.DeviceId;

                var newDevice = new Device(devId, device.DeviceModel, device.DeviceType, device.DeviceName) 
                {
                    Default = device.Default, Enabled = device.Enabled 
                };
                dev.Add(newDevice);

                // Add default settings
                if (dbContext.DeviceSettings != null)
                {
                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "width"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "width", "1"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "height"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "height", "1"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "brightness"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "brightness", "50"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "led_toptobottom"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_toptobottom", "true"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "led_alternatingrows"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_alternatingrows", "true"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "led_firstledleft"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "led_firstledleft", "true"));

                    if (!dbContext.DeviceSettings.Any(x => x.DeviceId == devId && x.SettingName == "mode"))
                        dbContext.DeviceSettings.Add(new DeviceSetting(devId, "mode", "none"));
                }

                await dbContext.SaveChangesAsync();
                Logger.Info("Created new Device with GUID {0}, Type {1} and Model {2}", devId, device.DeviceType, device.DeviceModel);

                var dto = new DtoDevice(newDevice.DeviceId, newDevice.DeviceModel, newDevice.DeviceType, newDevice.DeviceName)
                {
                    Enabled = newDevice.Enabled,
                    Default = newDevice.Default
                };

                var json2 = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Debug("NewDevice got the following result: {0}", json2);
                return json2;
            }

            Logger.Debug("NewDevice returns an empty response.");
            return null;
        }

        /// <summary>
        /// Sets properties (not settings) of a device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static async Task SetDevice(DtoDevice device)
        {
            Logger.Trace("Executing SetDevice...");

            using var dbContext = new TwDbContext();

            if (dbContext.Device != null)
            {
                var dev = dbContext.Device.SingleOrDefault(x => x.DeviceId == device.DeviceId);

                //Check if device ID exists
                if (dev == null)
                {
                    Logger.Error("Cannot find device ID {0}", device.DeviceId);
                    return;
                }

                // Set the properties
                dev.Default = device.Default;
                dev.Enabled = device.Enabled;
                dev.DeviceType = device.DeviceType;
                dev.DeviceModel = device.DeviceModel;
                dev.DeviceName = device.DeviceName;

                await dbContext.SaveChangesAsync();
                Logger.Info("Updated device {0}.", device.DeviceId);

            }
            else
            {
                Logger.Warn("SetDevice cannot find device {0}", device.DeviceId);
            }
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
