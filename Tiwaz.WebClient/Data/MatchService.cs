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

    }
}
