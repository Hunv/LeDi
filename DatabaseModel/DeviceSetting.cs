using System.ComponentModel.DataAnnotations;
using Tiwaz.Server.Api.DtoModel;

namespace Tiwaz.Server.DatabaseModel
{
    public class DeviceSetting
    {
        /// <summary>
        /// Create a new instance of DeviceSetting
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public DeviceSetting(string deviceId, string settingName, string settingValue)
        {
            DeviceId = deviceId;
            SettingName = settingName;
            SettingValue = settingValue;
        }

        /// <summary>
        /// The Deivce ID this setting is for
        /// </summary>
        ///[Key] Set in TwDbContext because multiple keys cannot being set via Attribute
        public string DeviceId { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
        ///[Key] Set in TwDbContext because multiple keys cannot being set via Attribute
        public string SettingName { get; set; }


        /// <summary>
        /// The Value of the Setting
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoDeviceSetting ToDto()
        {
            var dto = new DtoDeviceSetting(DeviceId, SettingName, SettingValue);

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoDeviceSetting dto)
        {
            SettingName = dto.Name;
            SettingValue = dto.Value;
            DeviceId = dto.DeviceId;
        }
    }
}
