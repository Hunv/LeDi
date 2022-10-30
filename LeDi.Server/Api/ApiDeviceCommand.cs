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
    public static class ApiDeviceCommand
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get all Device commands of a device
        /// </summary>
        public static string? GetDeviceCommands(string deviceId)
        {
            Logger.Trace("Executing GetDeviceCommands for deviceId {0}...", deviceId);

            using var dbContext = new TwDbContext();

            if (dbContext.DeviceCommands != null)
            {
                List<DtoDeviceCommand>? dto = dbContext.DeviceCommands.Where(x => x.DeviceId == deviceId).Select(aDevice => aDevice.ToDto()).ToList();

                var json = JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
                Logger.Trace("GetDeviceCommands got the follwoing results: {0}", json);
                return json;
            }

            Logger.Debug("GetDeviceCommands returns an empty response.");
            return null;
        }

        /// <summary>
        /// Creates device command
        /// </summary>
        public static async Task SetDeviceCommand(DtoDeviceCommand command)
        {
            Logger.Trace("Executing SetDeviceCommand...");

            if (string.IsNullOrWhiteSpace(command.DeviceId) || string.IsNullOrWhiteSpace(command.Command))
            {
                Logger.Warn("SetDeviceCommand is missing deviceId or Command.");
                return;
            }

            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceCommands;
            if (set != null)
            {
                set.Add(new DeviceCommand(command.DeviceId, command.Command, command.Parameter ?? ""));
                await dbContext.SaveChangesAsync();

                Logger.Debug("SetDeviceCommand \"{0}\" for Device {1} with Parameter \"{2}\" set", command.Command, command.DeviceId, command.Parameter);
            }
        }

        
        /// <summary>
        /// Deletes a device Command
        /// </summary>
        public static async Task DeleteDeviceCommand(int id, string deviceId)
        {
            Logger.Trace("Executing DeleteDeviceCommand...");

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                Logger.Warn("Cannot remove DeviceCommand because deviceId is missing");
                return;
            }

            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceCommands;
            if (set != null)
            {
                var tm = set.SingleOrDefault(x => x.DeviceId == deviceId && x.Id == id);
                if (tm != null)
                {
                    set.Remove(tm);
                    await dbContext.SaveChangesAsync();

                    Logger.Debug("Removed device command {0} for device {1}", id, deviceId);
                }
            }
        }
    }
}
