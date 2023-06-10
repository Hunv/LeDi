using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeDi.Display2.Display
{
    public class DeviceConfig
    {
        /// <summary>
        /// The Servername or IP where the LeDi server runs
        /// </summary>
        public string ServerURL { get; set; } = "https://localhost";

        /// <summary>
        /// The Device Name after register
        /// </summary>
        public string DeviceName { get; set; } = "LeDi";

        /// <summary>
        /// The Device Type after register
        /// </summary>
        public string DeviceType { get; set; } = "LED Screen";

        /// <summary>
        /// The Device Model after register
        /// </summary>
        public string DeviceModel { get; set; } = "LeDi LED Display";

        /// <summary>
        /// The GPIO Pin where the Display is connected to
        /// </summary>
        public int GpioPin { get; set; } = 12;

        /// <summary>
        /// The PWM Channel the Display is connected to
        /// </summary>
        public int PwmChannel { get; set; } = 0;

        /// <summary>
        /// The Device ID of this device to identify it on the server
        /// </summary>
        public string DeviceId { get; set; } = "";

        /// <summary>
        /// The Layout to use for the screen
        /// </summary>
        public string Layout { get; set; } = "10x5";

        /// <summary>
        /// The Width of the device
        /// </summary>
        public int DeviceWidth { get; set; } = 10;

        /// <summary>
        /// The Height of the device
        /// </summary>
        public int DeviceHeight { get; set; } = 5;

        /// <summary>
        /// The brightness of the Display (0-255)
        /// </summary>
        public byte Brightness { get; set; } = 50;

        /// <summary>
        /// The frequency of the update of the LEDs
        /// </summary>
        public uint Frequency { get; set; } = 0;

        /// <summary>
        /// The DMA Channel for the LEDs
        /// </summary>
        public int DMAChannel { get; set; } = 5;

        /// <summary>
        /// The name of the characterset to use
        /// </summary>
        public string CharacterSet { get; set; } = "default";

        /// <summary>
        /// Are areas cut off in case the content is larger than the area?
        /// </summary>
        public bool HardAreaBorders { get; set; } = false;

        /// <summary>
        /// Is the first LED at the top?
        /// </summary>
        public bool LedTopToBottom { get; set; } = false;

        /// <summary>
        /// Das the data flow of the rows is alternating?
        /// </summary>
        public bool LedAlternatingRows { get; set; } = true;

        /// <summary>
        /// Is the first LED on the left?
        /// </summary>
        public bool LedFirstLedLeft { get; set; } = false;
    }
}