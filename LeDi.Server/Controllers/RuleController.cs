using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeDi.Server.DatabaseModel;
using LeDi.Server.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LeDi.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;

        public RuleController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Gets the fields available to define rules
        /// </summary>
        /// <returns></returns>
        [HttpGet("rulefields")]
        public IActionResult GetRuleFields()
        {
            _logger.LogDebug("{0}: Get Rules", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiRule.GetRuleFields();
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

            var json = await Api.ApiRule.GetRules();
            var result = new OkObjectResult(json);

            _logger.LogDebug("{0}: Got Rules. JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }
    }
}
