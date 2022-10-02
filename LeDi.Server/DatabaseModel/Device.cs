using System.ComponentModel.DataAnnotations;
using LeDi.Shared.DtoModel;

namespace LeDi.Server.DatabaseModel
{
    public class Device
    {
        /// <summary>
        /// Create a new instance of Device
        /// </summary>
        public Device(string deviceId, string deviceModel, string deviceType)
        {
            DeviceId = deviceId;
            DeviceModel = deviceModel;
            DeviceType = deviceType;
        }


        /// <summary>
        /// The Device ID of this device
        /// </summary>
        [Key]
        public string DeviceId { get; set; }

        /// <summary>
        /// The Model of the Device
        /// </summary>        
        public string DeviceModel { get; set; }


        /// <summary>
        /// The Type of the Device (i.e. Browser, Display, App, ...)
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoDevice ToDto()
        {
            var dto = new DtoDevice(DeviceId, DeviceModel, DeviceType);

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoDevice dto)
        {
            DeviceModel = dto.DeviceModel;
            DeviceType = dto.DeviceType;
            DeviceId = dto.DeviceId;
        }
    }
}
