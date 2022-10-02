using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeDi.Shared.DtoModel
{
    public class DtoDevice
    {
        /// <summary>
        /// Create a new instance of Device
        /// </summary>
        public DtoDevice(string deviceId, string deviceModel, string deviceType)
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
    }
}
