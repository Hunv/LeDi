using System.ComponentModel.DataAnnotations;

namespace LeDi.Shared2.DatabaseModel
{
    public class TblSetting
    {
        /// <summary>
        /// Creates a new instance of Setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public TblSetting(string settingName, string settingValue)
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

    }
}
