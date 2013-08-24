using System;
using System.Collections.Generic;
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
        private APIConnectionHandler handler;

        public MainWindow()
        {
            InitializeComponent();
            // Make a list of buttons to inactivate during blocking behavior
            var listOfSearchButtons = new List<Button>();
            
            listOfSearchButtons.Add(GameSearch);
            listOfSearchButtons.Add(ChampSearch);
            SearchButtonList = listOfSearchButtons;
            handler = APIConnectionHandler.Handler;


            // Indicate connection, if not then lock UI
            ConnectionResultUpdate(handler.IsConnected);
        }

        /// <summary>
        /// Update the UI to post a message indicating that the connection
        /// either succeeded or failed. If it failed, lock search buttons.
        /// </summary>
        /// <param name="isConnected"></param>
        private void ConnectionResultUpdate(bool isConnected)
        {
            if (isConnected)
            {
                Title += " - Connection Succeeded";
            }
            else
            {
                Title += " - Connection Failed";
                EnableSearchButtons(SearchButtonList, false);
            }
        }

        /// <summary>
        /// Function to disable/reenable the search buttons in the UI
        /// so that a new search cannot be launched while an existing
        /// search is in progress.
        /// </summary>
        /// <param name="buttons">A list of Button objects to disable/enable</param>
        /// <param name="isEnabled">A boolean indicating whether to enable/disable</param>
        private void EnableSearchButtons(List<Button> buttons, bool isEnabled)
        {
            foreach (Button button in buttons)
            {
                button.IsEnabled = isEnabled;
            }
            Mouse.OverrideCursor = isEnabled ? null : Cursors.Wait;
        }

        private async void GameSearch_click(object sender, RoutedEventArgs e)
        {
            GameLookupStatusBox.Text = "";
            EnableSearchButtons(SearchButtonList, false);
            string region = GameRegion.Text;
            string name = GamePlayerName.Text;
            
            // Do work asynchronously
            try
            {
                GameStats stats = await SearchGameStats(name, region);
            }
            catch (ConnectionOfflineException coe)
            {
                string json = coe.Message;
                JObject o = JObject.Parse(json);
                string error = (string)o["Error"];
                GameLookupStatusBox.Text = error;
            }

            EnableSearchButtons(SearchButtonList, true);
        }

        private Task<GameStats> SearchGameStats(string name, string region)
        {
            return Task.Run(() =>
            {
                try
                {
                    return handler.GameRequest(name, region);
                }
                catch (ConnectionOfflineException)
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

        private async void ChampSearch_click(object sender, RoutedEventArgs e)
        {
            EnableSearchButtons(SearchButtonList, false);
            string region = ChampionRegion.Text;
            string name = ChampionName.Text;
            SummonerLookupStatusBox.Text = "";
            // Do work asynchronously
            try
            {
                PlayerStats stats = await SearchPlayerStats(name, region);
            }
            catch (ConnectionOfflineException coe)
            {
                string json = coe.Message;
                JObject o = JObject.Parse(json);
                string error = (string)o["Error"];
                SummonerLookupStatusBox.Text = error;
            }

            EnableSearchButtons(SearchButtonList, true);
        }
    }
}
