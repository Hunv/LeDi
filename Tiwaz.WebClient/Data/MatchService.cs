using Newtonsoft.Json;
using System.Text;
using Tiwaz.Shared.DtoModel;
using Tiwaz.WebClient.Data.Classes;

namespace Tiwaz.WebClient.Data
{
    public class MatchService
    {
        private static readonly string _ServerBaseUrl = "https://localhost:7077/api/";

        public MatchService()
        {
            Console.WriteLine("Using Serverbase URL " + _ServerBaseUrl);
        }


        /// <summary>
        /// Gets a Match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<DtoMatch?> GetMatchAsync(int matchId)
        {
            var match = new DtoMatch();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Match/" + matchId))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                match = JsonConvert.DeserializeObject<DtoMatch>(json, Helper.GetJsonSerializer());
            }

            return match;
        }

        /// <summary>
        /// Gets the time left of a match
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task<int> GetMatchTimeAsync(int matchId)
        {
            var matchTime = new int();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Match/" + matchId + "/time"))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                matchTime = JsonConvert.DeserializeObject<int>(json, Helper.GetJsonSerializer());
            }

            return matchTime;
        }

        /// <summary>
        /// Gets all Matches
        /// </summary>
        /// <returns></returns>
        public async Task<List<DtoMatch>> GetMatchListAsync()
        {
            var matchList = new List<DtoMatch>();

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            using (var jsonStream = await client.GetStreamAsync(_ServerBaseUrl + "Match"))
            {
                var sR = new StreamReader(jsonStream);
                var json = await sR.ReadToEndAsync();
                sR.Close();

                matchList = JsonConvert.DeserializeObject<List<DtoMatch>>(json, Helper.GetJsonSerializer());
            }

            return matchList ?? new List<DtoMatch>();
        }


        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task AddMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("POST", _ServerBaseUrl + "Match", json);
            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Add a new Tournament
        /// </summary>
        /// <param name="match"></param>
        public async Task SetMatchAsync(DtoMatch match)
        {
            var json = JsonConvert.SerializeObject(match, Helper.GetJsonSerializer());

            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("PUT", _ServerBaseUrl + "Match/" + match.Id, json);

            if (requestMessage.Content == null)
            {
                Console.WriteLine("No content set. Not setting content type...");
            }
            else
            {
                requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }
            

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Send Control Commands of a Match
        /// </summary>
        public async Task ControlMatchtimeAsync(int matchId, string command)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("PUT", _ServerBaseUrl + "Match/" + matchId + "/time/" + command, "");
            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task SetMatchGoalAsync(int matchId, int teamId, int amount)
        {
            //Allow untrusted certificates
            var handler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            HttpClient client = new HttpClient(handler);

            var requestMessage = Helper.GetRequestMessage("PUT", _ServerBaseUrl + "Match/" + matchId + "/goal/" + teamId + "/" + amount, "");
            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
