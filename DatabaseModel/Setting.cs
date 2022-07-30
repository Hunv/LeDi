using System.ComponentModel.DataAnnotations;
using Tiwaz.Server.Api.DtoModel;

namespace Tiwaz.Server.DatabaseModel
{
    public class Setting
    {
        /// <summary>
        /// The ID of the Setting
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The Name of the Setting
        /// </summary>
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
            var dto = new DtoSetting()
            {
                Name = SettingName,
                Value = SettingValue
            };

            return dto;
        }

        /// <summary>
        /// Converts the object from a DTO object
        /// </summary>
        /// <param name="dto"></param>
        public void FromDto(DtoSetting dto)
        {
            SettingName = dto.Name;
            SettingValue = dto.Value;

            using (var dbContext = new TwDbContext())
            {
                Id = dbContext.Settings.Single(x => x.SettingName == SettingName).Id;
            }
        }
    }
}
