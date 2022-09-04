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
using Tiwaz.Shared.DtoModel;

namespace Tiwaz.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceCommandController : ControllerBase
    {
        private readonly ILogger<DeviceCommandController> _logger;

        public DeviceCommandController(ILogger<DeviceCommandController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Gets all DeviceCommands of a device
        /// </summary>
        /// <returns></returns>
        [HttpGet("{deviceId}")]
        public IActionResult GetDeviceCommand(string deviceId)
        {
            _logger.LogDebug("{0}: Get DeviceCommands for {1}", Request.HttpContext.Connection.RemoteIpAddress, deviceId);

            var json = Api.ApiDeviceCommand.GetDeviceCommands(deviceId);
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got DeviceCommands for {1}. JSON: {2}", Request.HttpContext.Connection.RemoteIpAddress, deviceId, json);
            return result;
        }

        /// <summary>
        /// Adds a new DeviceCommand
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetDeviceCommand(
            [FromBody]DtoDeviceCommand command
            )
        {
            _logger.LogDebug("{0}: Add DeviceCommand {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, command.Command, command.DeviceId);

            await Api.ApiDeviceCommand.SetDeviceCommand(command);
            
            _logger.LogDebug("{0}: Added DeviceCommand {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, command.Command, command.DeviceId);
            return new OkResult(); ;
        }

        /// <summary>
        /// Adds a new DeviceCommand
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> RemoveDeviceCommand(
            [FromBody] DtoDeviceCommand command
            )
        {
            _logger.LogDebug("{0}: Removing DeviceCommand {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, command.Id, command.DeviceId);

            await Api.ApiDeviceCommand.DeleteDeviceCommand(command.Id, command.DeviceId ?? "");

            _logger.LogDebug("{0}: Removed DeviceCommand {1} for {2}", Request.HttpContext.Connection.RemoteIpAddress, command.Id, command.DeviceId);
            return new OkResult(); ;
        }
    }
}
