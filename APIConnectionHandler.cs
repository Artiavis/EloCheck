using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EloCheck
{
    class APIConnectionHandler
    {

        public static APIConnectionHandler APIConnection
        {
            get { return _conn; }
        }

        static readonly APIConnectionHandler _conn = new APIConnectionHandler();
        public bool IsConnected { private set; get; }
        private const string IDENT = "BOBDOLEISMYHERO";
        private LeagueAPI api;
        public List<BitmapImage> RankedChamps { get; private set; }

        public APIConnectionHandler()
        {
            RankedChamps = new List<BitmapImage>();
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
            // TODO make this processing a callback of the previous task
            PlayerStats stats = CompleteSummonerRequest(result);
            return result;
        }

        // TODO implement this using JsonSerializer.Deserialize and JReader
        private PlayerStats CompleteSummonerRequest(string json)
        {
            JsonSerializer ser = new JsonSerializer();
            PlayerStats playerStats = ser.Deserialize<PlayerStats>(new JReader(new StringReader(json)));
            return playerStats;
            // TODO Implement Ranked Stats and Match History
        }

        private void BuildChampions(string player, string json, string portraitsPath)
        {
            //        ScrollPane list = (ScrollPane) parent.lookup("#rankedChampList");
            //        AnchorPane anchor = (AnchorPane)list.lookup("#rankedChampions");
            RankedChamps.Clear();
            if (json == null)
                return;
            String[] champions = json.Split('_');
            // TODO sort array?
            int numChamps = champions.Length;

            // rankedChampClicked ??
            int i = 0, k = 0;
            for (; i < numChamps; i++)
            {
                string lCaseName = Regex.Replace(champions[i], "[^A-Za-z]", "").ToLowerInvariant();
                portraitsPath += lCaseName + "_square_0.png";
                Uri uri = new Uri(portraitsPath);
                BitmapImage portrait = new BitmapImage(uri);
                RankedChamps.Add(portrait);
            }
        }

        public bool StatsReqeust(string name, string region)
        {
            // TODO implement stats request
            return false;
        }

        public bool ChampionStatsRequest(string name, string champion, string region)
        {
            // TODO implement champion specific statistics reqeust
            return false;
        }

        public void GameRequest(string name, string region)
        {
            // TODO make this request using TPL
            string result = api.ApiRequest("game", name, region);
        }

        // TODO implement this
        public void CompleteGameRequest(string title)
        {

        }

        private class PlayerStats
        {
            bool Ranked { get; set; }
            Dictionary<String, Object> Ranked_Stats { get; set; }
            string Ranked_Champions { get; set; }
            string Tier { get; set; }
            string Division { get; set; }
            int Wins { get; set; }
            int Losses { get; set; }
            string S1 { get; set; }
            string S2 { get; set; }
            string S3 { get; set; }
        }

        /// <summary>
        /// Extend the JsonTextReader class to provide a version
        /// which corrects for key names with spaces.
        /// </summary>
        private class JReader : Newtonsoft.Json.JsonTextReader
        {
            public JReader(TextReader r)
                : base(r)
            { }

            public override bool Read()
            {
                bool b = base.Read();
                if (base.CurrentState == State.Property && ((string)base.Value).Contains(' '))
                {
                    base.SetToken(JsonToken.PropertyName, ((string)base.Value).Replace(' ', '_'));
                }
                return b;
            }
        }

    }
}
