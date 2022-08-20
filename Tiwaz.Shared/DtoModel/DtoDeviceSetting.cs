using System;
using System.Collections.Generic;
using System.Text;

namespace Tiwaz.Shared.DtoModel
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
        public string DeviceId { get; set; }

        /// <summary>
        /// The name of the setting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        public string Value { get; set; }
    }
}
