using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LeDi.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleController : ControllerBase
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the rules for all game types
        /// </summary>
        /// <returns></returns>
        [HttpGet("rules")]
        public async Task<IActionResult> GetRules()
        {
            _logger.Debug("{0}: Get Rules", Request.HttpContext.Connection.RemoteIpAddress);

            var json = await Api.ApiRule.GetRules();
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Rules. JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }
    }
}
