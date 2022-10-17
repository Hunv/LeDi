using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Shared.DtoModel
{
    public class DtoDeviceCommand
    {
        /// <summary>
        /// The Id of the Device Command
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The device Id to extecute the command on
        /// </summary>
        [RegularExpression(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", ErrorMessageResourceName = "DeviceIdFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDeviceCommand))]
        public string DeviceId { get; set; } = "";

        /// <summary>
        /// The command to run on the device
        /// </summary>
        [MinLength(1, ErrorMessageResourceName = "DeviceCommandErrorMin", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoDeviceCommand))]
        public string Command { get; set; } = "";

        /// <summary>
        /// The parameter for the device command
        /// </summary>
        public string Parameter { get; set; } = "";
    }
}
