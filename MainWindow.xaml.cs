using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace EloCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Button> SearchButtonList { get; set; }
        private List<TextBox> SearchBoxList { get; set; }
        private List<GamePlayerView> PlayerViews { get; set; }
        private List<GamePlayerView> EnemyViews { get; set; }
        private PlayerStatsView playerStatsView;
        private APIConnectionHandler handler;
        private static TextInfo textInfo = new CultureInfo("en-US").TextInfo;

        public MainWindow()
        {
            InitializeComponent();
            InitializeViewCollections();

            handler = APIConnectionHandler.Handler;
            // Indicate connection, if not then lock UI
            ConnectionResultUpdate(handler.IsConnected);
        }

        private async void GameSearch()
        {
            EnableSearch(false);
            string region = GameRegion.Text;
            string name = GamePlayerName.Text;
            
            // Do work asynchronously
            try
            {
                GameStats stats = await SearchGameStats(name, region);
                ClearGameView();
                GameLookupStatusBox.Text = textInfo.ToTitleCase(stats.gameType.Replace('_', ' '));
                //stats.playerTeam.Zip(PlayerViews, (stat, view) =>
                //    view.Load(new GamePlayerViewModel(stat))).AsParallel();
                //stats.enemyTeam.Zip(EnemyViews, (stat, view) =>
                //    view.Load(new GamePlayerViewModel(stat))).AsParallel();
                for (int i = 0; i < stats.playerTeam.Count; i++)
                {
                    PlayerViews[i].Load(new GamePlayerViewModel(stats.playerTeam[i]));
                    EnemyViews[i].Load(new GamePlayerViewModel(stats.enemyTeam[i]));
                }
            }
            catch (ClientDisconnectedException cde)
            {
                GameLookupStatusBox.Text = cde.Message;
                EnableSearch( false);

            }
            catch (BaseGameLookupException bglue)
            {
                string json = bglue.Message;
                JObject o = JObject.Parse(json);
                string error = (string)o["error"];
                GameLookupStatusBox.Text = error;
            }
            catch (Exception e)
            {
                GameLookupStatusBox.Text = e.Message;
            }

            EnableSearch(true);
        }

        private void GameSearch_click(object sender, RoutedEventArgs e)
        {
            GameSearch();
        }

        private Task<GameStats> SearchGameStats(string name, string region)
        {
            return Task.Run(() =>
            {
                try
                {
                    return handler.GameRequest(name, region);
                }
                catch (BaseGameLookupException)
                {
                    throw;
                }
            });
        }

        private Task<PlayerStats> SearchPlayerStats(string name, string region)
        {
            return Task.Run(() =>
            {
                try
                {
                    return handler.SummonerRequest(name, region);
                }
                catch (ConnectionOfflineException)
                {
                    throw;
                }
            });
        }

        private void SummonerSearch_click(object sender, RoutedEventArgs e)
        {
            SummonerSearch();
        }

        private async void SummonerSearch()
        {
            EnableSearch( false);
            string region = SummonerRegion.Text;
            string name = SummonerName.Text;
            SummonerLookupStatusBox.Text = "";
            // Do work asynchronously
            try
            {
                PlayerStats stats = await SearchPlayerStats(name, region);
                playerStatsView.Load(new PlayerStatsViewModel(stats));
            }
            catch (ClientDisconnectedException cde)
            {
                SummonerLookupStatusBox.Text = cde.Message;
                EnableSearch(false);
            }
            catch (BaseGameLookupException bglue)
            {
                string json = bglue.Message;
                JObject o = JObject.Parse(json);
                string error = (string)o["Error"];
                SummonerLookupStatusBox.Text = error;
            }
            catch (Exception e)
            {
                GameLookupStatusBox.Text = e.Message;
            }

            EnableSearch( true);
        }

        private void imgSrc_update(object sender, DataTransferEventArgs e)
        {
            if (e.Property == null)
            {
     
            }
            else
            {

            }
        }

        private void GamePlayerName_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                GameSearch();
            }
        }

        private void SummonerName_keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SummonerSearch();
            }
        }
    }

}
