using Newtonsoft.Json;
using System.Text;
using LeDi.Shared.DtoModel;
using LeDi.WebClient.Data.Classes;

namespace LeDi.WebClient.Data
{
    public class MatchService
    {
        private static readonly string _ServerBaseUrl = "https://localhost:7077/api/";

        public MatchService()
        {
            Console.WriteLine("Using Serverbase URL " + _ServerBaseUrl);
        }

    }
}
