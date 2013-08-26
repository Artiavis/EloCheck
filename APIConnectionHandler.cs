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

        public static APIConnectionHandler Handler
        {
            get { return _conn; }
        }

        static readonly APIConnectionHandler _conn = new APIConnectionHandler();
        public bool IsConnected { private set; get; }
        private const string IDENT = "BOBDOLEISMYHERO";
        private LeagueAPI api;

        public APIConnectionHandler()
        {
            api = new LeagueAPI(IDENT);
            
            IsConnected = api.Connect();
        }

        /// <summary>
        /// Calls a request for specific summoner
        /// </summary>
        /// <param name="name"></param>
        /// <param name="region"></param>
        /// <returns>Returns string containing response</returns>
        public PlayerStats SummonerRequest(string name, string region)
        {
            try
            {
                string result = api.ApiRequest("summoner", name, region);
                return CompleteSummonerRequest(result);
            }
            catch (ConnectionOfflineException)
            {
                throw;
            }
        }

        private PlayerStats CompleteSummonerRequest(string json)
        {
            JsonSerializer ser = new JsonSerializer();
            PlayerStats playerStats = ser.Deserialize<PlayerStats>(new JReader(new StringReader(json)));
            return playerStats;
            // TODO Implement Ranked Stats and Match History
        }

        //private void BuildChampions(string player, string json, string portraitsPath)
        //{
        //    //        ScrollPane list = (ScrollPane) parent.lookup("#rankedChampList");
        //    //        AnchorPane anchor = (AnchorPane)list.lookup("#rankedChampions");
        //    if (json == null)
        //        return;
        //    String[] champions = json.Split('_');
        //    // TODO sort array?
        //    int numChamps = champions.Length;

        //    // rankedChampClicked ??
        //    int i = 0, k = 0;
        //    for (; i < numChamps; i++)
        //    {
        //        string lCaseName = Regex.Replace(champions[i], "[^A-Za-z]", "").ToLowerInvariant();
        //        portraitsPath += lCaseName + "_square_0.png";
        //        Uri uri = new Uri(portraitsPath);
        //        BitmapImage portrait = new BitmapImage(uri);
        //    }
        //}

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

        public GameStats GameRequest(string name, string region)
        {
            try
            {
                string result = api.ApiRequest("game", name, region);
                return CompleteGameRequest(result);
            }
            catch (ConnectionOfflineException)
            {
                throw;
            }
        }

        private GameStats CompleteGameRequest(string json)
        {
            JsonSerializer ser = new JsonSerializer();
            GameStats gameStats = ser.Deserialize<GameStats>(new JReader(new StringReader(json)));
            return gameStats;
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

    // the API uses camelcase and the data mapper relies on property names

    /// <summary>
    /// Represents a model of a player's statistics.
    /// </summary>
    public class PlayerStats
    {
        [JsonProperty]
        public bool ranked { get; set; }
        [JsonProperty]
        public Dictionary<string, int> rankedStats { get; set; }
        [JsonProperty]
        public string rankedChampions { get; set; }
        [JsonProperty]
        public string tier { get; set; }
        [JsonProperty]
        public string division { get; set; }
        [JsonProperty]
        public int wins { get; set; }
        [JsonProperty]
        public int losses { get; set; }
        [JsonProperty]
        public string s1 { get; set; }
        [JsonProperty]
        public string s2 { get; set; }
    }

    /// <summary>
    /// Represents a model of a match's statistics.
    /// </summary>
    public class GameStats
    {
        [JsonProperty]
        public string gameType { get; set; }
        [JsonProperty]
        public List<GamePlayer> playerTeam { get; set; }
        [JsonProperty]
        public List<GamePlayer> enemyTeam { get; set; }
    }

    public class GamePlayer
    {
        [JsonProperty]
        public string champ { get; set; }
        [JsonProperty]
        public string name { get; set; }
        [JsonProperty]
        public string tier { get; set; }
        [JsonProperty]
        public string division { get; set; }
        [JsonProperty]
        public int approxElo { get; set; }
    }
}
