using System.ComponentModel.DataAnnotations;

namespace Tiwaz.Server.DatabaseModel
{
    public class Setting
    {
        /// <summary>
        /// The ID of the Setting
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
        public string SettingName { get; set; }


        /// <summary>
        /// The Value of the Setting
        /// </summary>
        public string SettingValue { get; set; }
    }
}
