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
    }
}
