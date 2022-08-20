using System;
using System.Collections.Generic;
using System.Text;

namespace Tiwaz.Shared.DtoModel
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
        public string? Name { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        public string? Value { get; set; }
    }
}
