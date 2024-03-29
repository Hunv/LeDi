﻿using System;
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
    public class SettingController : ControllerBase
    {
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets all System Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSetting()
        {
            _logger.Debug("{0}: Get Settinglist", Request.HttpContext.Connection.RemoteIpAddress);

            var json = Api.ApiSetting.GetSetting();
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Settinglist JSON {1}", Request.HttpContext.Connection.RemoteIpAddress, json);
            return result;
        }

        /// <summary>
        /// Gets a System Setting
        /// </summary>
        /// <returns></returns>
        [HttpGet("{settingName}")]
        public IActionResult GetSetting(string settingName)
        {
            _logger.Debug("{0}: Get Setting {1}", Request.HttpContext.Connection.RemoteIpAddress, settingName);

            var json = Api.ApiSetting.GetSetting(settingName);
            var result = new OkObjectResult(json);

            _logger.Debug("{0}: Got Setting {1} JSON {2}", Request.HttpContext.Connection.RemoteIpAddress, settingName, json);
            return result;
        }

        /// <summary>
        /// Modify a System Setting
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> SetSetting(
            [FromBody] DtoSetting setting
            )
        {
            _logger.Debug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, setting.Name, setting.Value);

            await Api.ApiSetting.SetSetting(setting.Name ?? "", setting.Value ?? "");
            
            _logger.Debug("{0}: Set Setting {1} to {2}", Request.HttpContext.Connection.RemoteIpAddress, setting.Name, setting.Value);
            return new OkResult(); ;
        }
    }
}
