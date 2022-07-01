using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingController : ControllerBase
    {
        private readonly ILogger<SettingController> _logger;

        public SettingController(ILogger<SettingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>A list of all settings</returns>
        /// <response code="200">Success</response>
        [HttpGet(Name = "GetSettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Setting> Get()
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings;
                return dto.ToArray();
            }
        }

        /// <summary>
        /// Creates a new setting
        /// </summary>
        /// <param name="settingName">The name of the setting (case sensitive!)</param>
        /// <param name="settingValue">The value of the setting</param>
        /// <returns>The newly created setting including the setting ID</returns>
        /// <remarks>
        /// Settingname is case sensitive!
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="409">The setting already exists. Use HTTP Post to set an existing value</response>
        [HttpPut(Name = "AddSetting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Put(
            [FromQuery]string settingName, 
            [FromQuery]string settingValue
            )
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings;
                if (dto.SingleOrDefault(x => x.SettingName == settingName) != null)
                {
                    _logger.LogError("The setting {0} already exists.", settingName);
                    return new ConflictResult();
                }

                dto.Add(new Setting { SettingName = settingName, SettingValue = settingValue });
                await dbContext.SaveChangesAsync();
                return new OkObjectResult(dbContext.Settings.Single(x => x.SettingName == settingName));
            }
        }

        /// <summary>
        /// Updates an existing setting
        /// </summary>
        /// <param name="settingName">The name of the setting (case sensitive!)</param>
        /// <param name="settingValue">The new value of the setting</param>
        /// <returns>The updated setting including the setting ID</returns>
        /// <remarks>
        /// Settingname is case sensitive!
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Cannot find the named setting</response>
        [HttpPost(Name = "UpdateSetting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post(
            [FromQuery] string settingName,
            [FromQuery] string settingValue
            )
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings;
                var settingItem = dto.SingleOrDefault(x => x.SettingName == settingName);
                if (settingItem == null)
                {
                    _logger.LogError("The setting {0} does not exist.", settingName);
                    return new NotFoundResult();
                }
                settingItem.SettingValue = settingValue;
                await dbContext.SaveChangesAsync();
                return new OkObjectResult(dbContext.Settings.Single(x => x.SettingName == settingName));
            }
        }


        /// <summary>
        /// Updates an existing setting
        /// </summary>
        /// <param name="settingName">The name of the setting (case sensitive!)</param>
        /// <remarks>
        /// Settingname is case sensitive!
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="404">Cannot find the named setting</response>
        [HttpDelete(Name = "DeleteSetting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromQuery] string settingName
            )
        {
            using (var dbContext = new TwDbContext())
            {
                var dto = dbContext.Settings;
                var settingItem = dto.SingleOrDefault(x => x.SettingName == settingName);
                if (settingItem == null)
                {
                    _logger.LogError("The setting {0} does not exist.", settingName);
                    return new NotFoundResult();
                }
                dbContext.Settings.Remove(settingItem);
                await dbContext.SaveChangesAsync();
                return new OkResult();
            }
        }
    }
}