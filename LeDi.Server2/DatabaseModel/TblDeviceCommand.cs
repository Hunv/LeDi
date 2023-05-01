using System.ComponentModel.DataAnnotations;

namespace LeDi.Server2.DatabaseModel
{
    public class TblDeviceCommand
    {
        /// <summary>
        /// Create a new instance of DeviceCommand
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        public TblDeviceCommand(string deviceId, string command, string parameter)
        {
            DeviceId = deviceId;
            Command = command;
            Parameter = parameter;
        }

        /// <summary>
        /// The internal ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Device ID this setting is for
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
        public string Command { get; set; }


        /// <summary>
        /// The Value of the Setting
        /// </summary>
        public string Parameter { get; set; }

    }
}
