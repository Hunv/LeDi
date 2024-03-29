﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LeDi.Shared.DtoModel;

namespace LeDi.Server.DatabaseModel
{
    public class Device
    {
        /// <summary>
        /// Create a new instance of Device
        /// </summary>
        public Device(string deviceId, string deviceModel, string deviceType, string deviceName)
        {
            DeviceId = deviceId;
            DeviceModel = deviceModel;
            DeviceType = deviceType;
            DeviceName = deviceName;
        }

        /// <summary>
        /// Internal Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Device ID of this device
        /// </summary>
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
        /// The alias of the Device (i.e. LeDi Display)
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Is this device enabled?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Is this the default device?
        /// </summary>
        public bool Default { get; set; }


        /// <summary>
        /// The List of match IDs that player participated at
        /// </summary>
        [NotMapped]
        [System.Text.Json.Serialization.JsonIgnore]
        public int[]? MatchIdList { get; set; }

        /// <summary>
        /// The List of match that devices are used at
        /// </summary>
        public Device2Match[]? MatchList { get; set; }

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoDevice ToDto()
        {
            var dto = new DtoDevice(DeviceId, DeviceModel, DeviceType, DeviceName)
            {
                Enabled = Enabled,
                Default = Default
            };

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
            DeviceName = dto.DeviceName;
            Enabled = dto.Enabled;
            Default = dto.Default;
        }
    }
}
