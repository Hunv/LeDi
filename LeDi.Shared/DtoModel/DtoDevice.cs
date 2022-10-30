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
        public DtoDevice(string deviceId, string deviceModel, string deviceType, string deviceName)
        {
            DeviceId = deviceId;
            DeviceModel = deviceModel;
            DeviceType = deviceType;
            DeviceName = deviceName;
        }


        /// <summary>
        /// The Device ID of this device
        /// </summary>
        [Key]
        [RegularExpression(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", ErrorMessageResourceName = "DeviceIdFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDevice))]
        public string DeviceId { get; set; }

        /// <summary>
        /// The Model of the Device
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9\s,.-_]+$", ErrorMessageResourceName = "DeviceModelFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDevice))]
        public string DeviceModel { get; set; }


        /// <summary>
        /// The Type of the Device (i.e. Browser, Display, App, ...)
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9\s,.-_]+$", ErrorMessageResourceName = "DeviceTypeFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDevice))]
        public string DeviceType { get; set; }

        /// <summary>
        /// An alias for the device
        /// </summary>
        [RegularExpression(@"^[A-Za-z0-9\s,.-_]+$", ErrorMessageResourceName = "DeviceNameFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDevice))]
        public string DeviceName { get; set; }

        /// <summary>
        /// Is this device enabled?
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Is this device the default device?
        /// </summary>
        public bool Default { get; set; }
    }
}
