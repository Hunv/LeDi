using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tiwaz.Server.DatabaseModel;
using Tiwaz.Server.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Tiwaz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;

        public DeviceController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDeviceSetting()
        {
            _logger.LogDebug("{0}: Get Device List", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiDevice.GetDeviceList();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Devicelist. {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{deviceId}/{settingName}")]
        public IActionResult GetDeviceSetting(
            [FromQuery] string deviceId,
            [FromQuery] string settingName
            )
        {
            _logger.LogDebug("{0}: Get Device Setting {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName, deviceId);

            var json = Api.ApiDevice.GetDeviceSetting(deviceId, settingName);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Setting {1} for {2} JSON {3}", Request.HttpContext.Connection.RemoteIpAddress, settingName, deviceId, json);
            return result;
        }

        /// <summary>
        /// Modify or creates a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut("{deviceId}")]
        public async Task<IActionResult> SetDeviceSetting(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoDeviceSetting setting
            )
        {
            _logger.LogDebug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, setting.DeviceId, setting.Name, setting.Value);

            await Api.ApiDevice.SetDeviceSetting(setting.DeviceId, setting.Name, setting.Value);

            _logger.LogDebug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, setting.DeviceId, setting.Name, setting.Value);
            return new OkResult(); ;
        }

        /// <summary>
        /// Creates a new device
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewDevice(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoDevice device
            )
        {
            _logger.LogDebug("{0}: Creating a new device... Model: {1}, Type: {2}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceModel, device.DeviceType);

            var json = await Api.ApiDevice.NewDevice(device);

            _logger.LogDebug("{0}: Created Device Model: {1} Type: {2}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceModel, device.DeviceType);
            return new OkObjectResult(json); ;
        }
    }
}
