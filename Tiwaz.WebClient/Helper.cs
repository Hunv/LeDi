using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tiwaz.WebClient
{
    public static class Helper
    {
        public static JsonSerializerSettings GetJsonSerializer()
        {
            var js = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            };

            return js;
        }

        public static string SerializeObject(object data)
        {
            return (JsonConvert.SerializeObject(data, GetJsonSerializer()));
        }

        public static HttpRequestMessage GetRequestMessage(string method, string uri, string json)
        {
            return new HttpRequestMessage()
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(uri),
                Content = new StringContent(json)
            };
        }
    }
}
