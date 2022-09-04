using Tiwaz.Server.DatabaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tiwaz.Shared.DtoModel;
using Tiwaz.Shared;

namespace Tiwaz.Server.Api
{
    public static class ApiDeviceCommand
    {
        /// <summary>
        /// Get all Device commands of a device
        /// </summary>
        public static string? GetDeviceCommands(string deviceId)
        {
            using var dbContext = new TwDbContext();

            if (dbContext.DeviceCommands != null)
            {
                List<DtoDeviceCommand>? dto = dbContext.DeviceCommands.Select(aDevice => aDevice.ToDto()).ToList();
                return JsonConvert.SerializeObject(dto, Helper.GetJsonSerializer());
            }
            return null;
        }

        /// <summary>
        /// Creates device command
        /// </summary>
        public static async Task SetDeviceCommand(DtoDeviceCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.DeviceId) || string.IsNullOrWhiteSpace(command.Command))
            {
                Console.WriteLine("SetDeviceCommand is missing deviceId or Command.");
                return;
            }

            using var dbContext = new TwDbContext();
            var set = dbContext.DeviceCommands;
            if (set != null)
            {
                set.Add(new DeviceCommand(command.DeviceId, command.Command, command.Parameter ?? ""));
                await dbContext.SaveChangesAsync();
            }
        }

        
        /// <summary>
        /// Deletes a device Command
        /// </summary>
        public static async Task DeleteDeviceCommand(int id, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                Console.WriteLine("Cannot remove DeviceCommand because deviceId is missing");
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
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
