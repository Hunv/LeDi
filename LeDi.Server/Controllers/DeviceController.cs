using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeDi.Shared.DtoModel;

namespace LeDi.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets all devices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDeviceList()
        {
            _logger.Debug("{0}: Get Device List", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiDevice.GetDeviceList();
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Devicelist. {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Modify a device
        /// </summary>
        /// <returns></returns>
        [HttpPut("{deviceId}")]
        public async Task<IActionResult> SetDevice(
            string deviceId,
            [FromBody] DtoDevice device
            )
        {
            _logger.Debug("{0}: Set device for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceId);

            await Api.ApiDevice.SetDevice(device);

            _logger.Debug("{0}: Set device for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceId);
            return new OkResult(); ;
        }

        /// <summary>
        /// Gets all Settings of a Device
        /// </summary>
        /// <returns></returns>
        [HttpGet("{deviceId}")]
        public IActionResult GetDeviceSetting(
            string deviceId
            )
        {
            _logger.Debug("{0}: Get Device settings for {1}", Request.HttpContext.Connection.RemoteIpAddress, deviceId);

            var json = Api.ApiDevice.GetDeviceSettings(deviceId);
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Device settings for {1}. {2}", Request.HttpContext.Connection.RemoteIpAddress, deviceId, json);
            return result;
        }

        /// <summary>
        /// Gets a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{deviceId}/{settingName}")]
        public IActionResult GetDeviceSetting(
            string deviceId,
            string settingName
            )
        {
            _logger.Debug("{0}: Get Device Setting {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName, deviceId);

            var json = Api.ApiDevice.GetDeviceSetting(deviceId, settingName);
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Setting {1} for {2} JSON {3}", Request.HttpContext.Connection.RemoteIpAddress, settingName, deviceId, json);
            return result;
        }

        /// <summary>
        /// Modify or creates a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetDeviceSetting(
            [FromBody] DtoDeviceSetting setting
            )
        {
            _logger.Debug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, setting.DeviceId, setting.Name, setting.Value);

            await Api.ApiDevice.SetDeviceSetting(setting.DeviceId, setting.Name, setting.Value);

            _logger.Debug("{0}: Set Setting for {1} from {2} to {3}", Request.HttpContext.Connection.RemoteIpAddress, setting.DeviceId, setting.Name, setting.Value);
            return new OkResult(); ;
        }

        /// <summary>
        /// Delete a Device Setting
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{deviceId}/{settingName}")]
        public async Task<IActionResult> DeleteDeviceSetting(
            [FromRoute] string deviceId,
            [FromRoute] string settingName
            )
        {
            _logger.Debug("{0}: Delete Setting for {1} from {2}", Request.HttpContext.Connection.RemoteIpAddress, deviceId, settingName);

            await Api.ApiDevice.DeleteDeviceSetting(deviceId, settingName);

            _logger.Debug("{0}: Delete Setting for {1} from {2}", Request.HttpContext.Connection.RemoteIpAddress, deviceId, settingName);
            return new OkResult(); ;
        }


        /// <summary>
        /// Creates a new device
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> NewDevice(
            [FromBody] DtoDevice device
            )
        {
            if (string.IsNullOrEmpty(device.DeviceId))
                _logger.Debug("{0}: Creating a new device... Model: {1}, Type: {2}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceModel, device.DeviceType);
            else
                _logger.Debug("{0}: Verifing device... ID: {1}, Model: {2}, Type: {3}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceId, device.DeviceModel, device.DeviceType);

            var json = await Api.ApiDevice.NewDevice(device);

            _logger.Debug("{0}: Created/Verified Device. Model: {1} Type: {2}", Request.HttpContext.Connection.RemoteIpAddress, device.DeviceModel, device.DeviceType);
            return new OkObjectResult(json); ;
        }

        /// <summary>
        /// Delete a Device
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> DeleteDevice(
            [FromRoute] string deviceId
            )
        {
            _logger.Debug("{0}: Delete Device {1}", Request.HttpContext.Connection.RemoteIpAddress, deviceId);

            await Api.ApiDevice.DeleteDevice(deviceId);

            _logger.Debug("{0}: Deleted Device {1}", Request.HttpContext.Connection.RemoteIpAddress, deviceId);
            return new OkResult(); ;
        }

    }
}
