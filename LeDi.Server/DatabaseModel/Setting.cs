using System.ComponentModel.DataAnnotations;
using LeDi.Shared.DtoModel;

namespace LeDi.Server.DatabaseModel
{
    public class Setting
    {
        /// <summary>
        /// Creates a new instance of Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public Setting(string settingName, string settingValue)
        {
            SettingName = settingName;
            SettingValue = settingValue;
        }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
        [Key]
        public string SettingName { get; set; }


        /// <summary>
        /// The Value of the Setting
        /// </summary>
        public string SettingValue { get; set; }

        /// <summary>
        /// Converts the object to a DTO object
        /// </summary>
        /// <returns></returns>
        public DtoSetting ToDto()
        {
            var dto = new DtoSetting(SettingName, SettingValue);

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoSetting dto)
        {
            if (dto == null)
                return;

            SettingName = dto.Name ?? "";
            SettingValue = dto.Value ?? "";
        }
    }
}
