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
        /// Gets a Setting
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
        /// Gets a Setting
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
        /// Modify a Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetSetting(
            [FromBody] Tiwaz.Server.Api.DtoModel.DtoSetting settingName
            )
        {
            _logger.LogDebug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName.Name, settingName.Value);

            await Api.ApiSetting.SetSetting(settingName.Name, settingName.Value);
            
            _logger.LogDebug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName.Name, settingName.Value);
            return new OkResult(); ;
        }
        
        /// <summary>
        /// Gets the fields available to define rules
        /// </summary>
        /// <returns></returns>
        [HttpGet("rulefields")]
        public IActionResult GetRuleFields()
        {
            _logger.LogDebug("{0}: Get Rules", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiSetting.GetRuleFields();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Rules. JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets the rules for all game types
        /// </summary>
        /// <returns></returns>
        [HttpGet("rules")]
        public async Task<IActionResult> GetRules()
        {
            _logger.LogDebug("{0}: Get Rules", Request.HttpContext.Connection.RemoteIpAddress);

            var json = await Api.ApiSetting.GetRules();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Rules. JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }
    }
}
