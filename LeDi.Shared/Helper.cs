using Newtonsoft.Json;

namespace LeDi.Shared
{
    public class Helper
    {
        /// <summary>
        /// Helper method to get the Json Serializer settings
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Serializes an object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeObject(object data)
        {
            return (JsonConvert.SerializeObject(data, GetJsonSerializer()));
        }

        public static HttpRequestMessage GetRequestMessage(string method, string uri)
        {
            return new HttpRequestMessage()
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(uri)
            };
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

        /// <summary>
        /// Gets an object from the Server
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string?> ApiRequestGet(string url)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);
            
            using (var jsonStream = await client.GetStreamAsync(url))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();
                
                return json;
            }
        }

        /// <summary>
        /// PUTs a query for the API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<string?> ApiRequestPut(string url, string body)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = GetRequestMessage("PUT", url, body);

            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type header...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content == null)
                    return "";

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine("Failed to PUT {0}", url);
                return null;
            }
        }

        /// <summary>
        /// Executes a DELETE request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string?> ApiRequestDelete(string url, string? body = null)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            HttpRequestMessage requestMessage;

            if (body == null)
            {
                Console.WriteLine("No content set. Not setting content type header...");
                requestMessage = GetRequestMessage("DELETE", url);
            }
            else
            {
                requestMessage = GetRequestMessage("DELETE", url, body);
                if (requestMessage.Content != null)
                    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content == null)
                    return "";

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine("Failed to DELETE {0}", url);
                return null;
            }
        }

        /// <summary>
        /// POSTs a query for the API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static async Task<string?> ApiRequestPost(string url, string body)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("POST", url, body);

            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type header...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            var response = await client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                if (response.Content == null)
                    return "";

                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine("Failed to POST {0}", url);
                return null;
            }
        }
    }
}