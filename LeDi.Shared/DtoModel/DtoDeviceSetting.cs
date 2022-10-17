using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeDi.Shared.DtoModel
{
    public class DtoDeviceSetting
    {
        public DtoDeviceSetting(string deviceId, string name, string value)
        {
            DeviceId = deviceId;
            Name = name;
            Value = value;
        }


        /// <summary>
        /// The DeivceId for the setting
        /// </summary>
        [RegularExpression(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", ErrorMessageResourceName = "DeviceIdFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDeviceSetting))]
        public string DeviceId { get; set; }

        /// <summary>
        /// The name of the setting
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessageResourceName = "SettingNameFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDeviceSetting))]
        public string Name { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        public string Value { get; set; }
    }
}
