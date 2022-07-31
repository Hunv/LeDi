using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Tiwaz.Server.DatabaseModel;

namespace Tiwaz.Server.Api.DtoModel
{
    public class DtoDevice : Device
    {
        public DtoDevice(string deviceId, string deviceModel, string deviceType) : base(deviceId, deviceModel, deviceType)
        {
        }
    }
}
