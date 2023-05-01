using System.ComponentModel.DataAnnotations;

namespace LeDi.Server2.DatabaseModel
{
    public class TblDeviceSetting
    {
        /// <summary>
        /// Create a new instance of DeviceSetting
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public TblDeviceSetting(string deviceId, string settingName, string settingValue)
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
    }
}
