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
    public class SettingController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;

        public SettingController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Gets all System Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSetting()
        {
            _logger.LogDebug("{0}: Get Settinglist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiSetting.GetSetting();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Settinglist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets a System Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{settingName}")]
        public IActionResult GetSetting(string settingName)
        {
            _logger.LogDebug("{0}: Get Setting {1}", Request.HttpContext.Connection.RemoteIpAddress, settingName);

            var json = Api.ApiSetting.GetSetting(settingName);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Setting {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName, json);
            return result;
        }

        /// <summary>
        /// Modify a System Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetSetting(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoSetting setting
            )
        {
            _logger.LogDebug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, setting.Name, setting.Value);

            await Api.ApiSetting.SetSetting(setting.Name, setting.Value);
            
            _logger.LogDebug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, setting.Name, setting.Value);
            return new OkResult(); ;
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

            var json = Api.ApiSetting.GetDeviceSetting(deviceId, settingName);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Setting {1} for {2} JSON {3}", Request.HttpContext.Connection.RemoteIpAddress, settingName, deviceId, json);
            return result;
        }

        /// <summary>
        /// Modify or creates a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetDeviceSetting(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoDeviceSetting setting
            )
        {
            _logger.LogDebug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, setting.DeviceId, setting.Name, setting.Value);

            await Api.ApiSetting.SetDeviceSetting(setting.DeviceId, setting.Name, setting.Value);

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
            _logger.LogDebug("{0}: Creating a new device... {1}, {2}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceModel, device.DeviceType);

            await Api.ApiSetting.NewDevice(device);

            _logger.LogDebug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceId, device.DeviceModel, device.DeviceType);
            return new OkResult(); ;
        }
    }
}
