using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Tiwaz.Server
{
    public class Helper
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

        public static string Hash(string text, string salt1, string salt2)
        {
            using (SHA512 hash512 = SHA512.Create())
            {
                //Hashing using salt1 + text
                byte[] bytes = hash512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt1 + text));

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                var turn1 = sb.ToString();

                //Second turn hashing turn1 hash again using a second salt
                bytes = hash512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(turn1 + salt2));

                sb = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
