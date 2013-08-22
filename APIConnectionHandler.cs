using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace EloCheck
{
    class APIConnectionHandler
    {

        static readonly APIConnectionHandler APIConnection 
        {
            private set; 
            public get; 
        }

        bool IsConnected { private set; public get; }
        private const string IDENT = "BOBDOLEISMYHERO";
        private LeagueAPI api;
        private List<BitmapImage> rankedChamps;

        public APIConnectionHandler()
        {
            APIConnection = new APIConnectionHandler();
            rankedChamps = new List<BitmapImage>();
            api = new LeagueAPI(IDENT);
            IsConnected = api.Connect();
        }

        /// <summary>
        /// Calls a request for specific summoner
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <returns>Returns string containing response</returns>
        public string RequestSummoner(string name, string region)
        {
            // TODO perform this task using TPL
            string result = api.ApiRequest("summoner", name, region);
            return result;
        }

        // TODO implement this
        private void CompleteSummonerRequest(string title)
        {
            
        }
    }
}
