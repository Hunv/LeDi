using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeDi.Shared.DtoModel
{
    public class DtoSetting
    {
        public DtoSetting() { }
        public DtoSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The name of the setting
        /// </summary>
        [MaxLength(256, ErrorMessageResourceName = "NameLengthErrorMax", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoSetting))]
        [MinLength(2, ErrorMessageResourceName = "NameLengthErrorMin", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoSetting))]
        [RegularExpression(@"^[\w]*$", ErrorMessageResourceName = "NameFormatError", ErrorMessageResourceType = typeof(Resources.DtoModel.DtoSetting))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Name { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Value { get; set; }
    }
}
