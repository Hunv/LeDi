using System.ComponentModel.DataAnnotations;
using Tiwaz.Shared.DtoModel;

namespace Tiwaz.Server.DatabaseModel
{
    public class DeviceCommand
    {
        public DeviceCommand()
        {
        }

        /// <summary>
        /// Create a new instance of DeviceCommand
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        public DeviceCommand(string deviceId, string command, string parameter)
        {
            DeviceId = deviceId;
            Command = command;
            Parameter = parameter;
        }

        /// <summary>
        /// The Deivce ID this setting is for
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Deivce ID this setting is for
        /// </summary>
        public string? DeviceId { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
        public string? Command { get; set; }


        /// <summary>
        /// The Value of the Setting
        /// </summary>
        public string? Parameter { get; set; }

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoDeviceCommand ToDto()
        {
            var dto = new DtoDeviceCommand()
            {
                Id = Id,
                DeviceId = DeviceId, 
                Command = Command, 
                Parameter = Parameter
            };

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoDeviceCommand dto)
        {
            Id = dto.Id;
            Command = dto.Command;
            Parameter = dto.Parameter;
            DeviceId = dto.DeviceId;
        }
    }
}
