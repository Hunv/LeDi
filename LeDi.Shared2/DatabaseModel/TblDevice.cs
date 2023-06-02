using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblDevice
    {
        /// <summary>
        /// Create a new instance of Device
        /// </summary>
        public TblDevice(string deviceId, string deviceModel, string deviceType, string deviceName)
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
        /// The List of match that devices are used at
        /// </summary>
        public List<TblDevice2Match>? MatchList { get; set; }

        /// <summary>
        /// The List of match that devices are used at
        /// </summary>
        public List<TblDevice2Tournament>? TournamentList { get; set; }

    }
}
