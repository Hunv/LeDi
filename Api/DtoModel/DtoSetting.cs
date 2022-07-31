﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tiwaz.Server.Api.DtoModel
{
    public class DtoSetting
    {
        /// <summary>
        /// Create a new instance of DtoSetting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DtoSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }


        /// <summary>
        /// The name of the setting
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the setting
        /// </summary>
        public string Value { get; set; }
    }
}
