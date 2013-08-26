﻿using System;
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
    public partial class MainWindow : Window
    {
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

        private void InitializeViewCollections()
        {
            // Make a list of buttons to inactivate during blocking behavior
            var listOfSearchButtons = new List<Button>();
            listOfSearchButtons.Add(GameSearchButton);
            listOfSearchButtons.Add(SummonerSearchButton);
            SearchButtonList = listOfSearchButtons;

            // Make a list of team players
            var listOfPlayerViews = new List<GamePlayerView>();
            listOfPlayerViews.Add(new GamePlayerView(PlayerImage1, PlayerName1, PlayerDiv1));
            listOfPlayerViews.Add(new GamePlayerView(PlayerImage2, PlayerName2, PlayerDiv2));
            listOfPlayerViews.Add(new GamePlayerView(PlayerImage3, PlayerName3, PlayerDiv3));
            listOfPlayerViews.Add(new GamePlayerView(PlayerImage4, PlayerName4, PlayerDiv4));
            listOfPlayerViews.Add(new GamePlayerView(PlayerImage5, PlayerName5, PlayerDiv5));
            PlayerViews = listOfPlayerViews;

            var listOfEnemyViews = new List<GamePlayerView>();
            listOfEnemyViews.Add(new GamePlayerView(EnemyImage1, EnemyName1, EnemyDiv1));
            listOfEnemyViews.Add(new GamePlayerView(EnemyImage2, EnemyName2, EnemyDiv2));
            listOfEnemyViews.Add(new GamePlayerView(EnemyImage3, EnemyName3, EnemyDiv3));
            listOfEnemyViews.Add(new GamePlayerView(EnemyImage4, EnemyName4, EnemyDiv4));
            listOfEnemyViews.Add(new GamePlayerView(EnemyImage5, EnemyName5, EnemyDiv5));
            EnemyViews = listOfEnemyViews;
        }

        private void ClearGameView()
        {
            ClearGamePlayerViews(PlayerViews);
            ClearGamePlayerViews(EnemyViews);
            GameLookupStatusBox.Text = "";
        }

        private void ClearGamePlayerViews(List<GamePlayerView> viewList)
        {
            foreach (GamePlayerView view in viewList)
            {
                view.ClearView();
            }
        }

        /// <summary>
        /// A view encapsulating a game player's display controls.
        /// </summary>
        private class GamePlayerView
        {
            private static BitmapImage img = new BitmapImage(
                new Uri("pack://application:,,,/Resources/splash/default_square_0.png"));
            public GamePlayerView(Image img, TextBlock name, Image div)
            {
                Portrait = img;
                Name = name;
                Division = div;
            }
            public Image Portrait { get; set; }
            public TextBlock Name { get; set; }
            public Image Division { get; set; }

            public void ClearView()
            {
                Portrait.SetCurrentValue(Image.SourceProperty, img);
                Name.Text = "";
                Division.SetCurrentValue(Image.SourceProperty, null);
            }

            public GamePlayerView Load(GamePlayerViewModel vm)
            {
                Portrait.SetCurrentValue(Image.SourceProperty, vm.Portrait);
                Division.SetCurrentValue(Image.SourceProperty, vm.DivisionMedal);
                Name.Text = vm.Name;
                return this;
            }
        }

        private class GamePlayerViewModel
        {
            private static string portraitURI = "pack://application:,,,/Resources/splash/";
            private static string medalURI = "pack://application:,,,/Resources/medals/";

            public GamePlayerViewModel(GamePlayer player)
            {
                Name = player.name;
                string champ = player.champ.Replace(" ", string.Empty).ToLowerInvariant();
                Portrait = new BitmapImage(new Uri(portraitURI + champ + "_square_0.png"));
                string medal = player.tier + "_" + EloCheckUtility.RomanNumeralToInt(player.division);
                DivisionMedal = new BitmapImage(new Uri(medalURI + medal + ".png"));
            }

            public BitmapImage Portrait { get; private set; }
            public BitmapImage DivisionMedal { get; private set; }
            public string Name { get; private set; }

        }
    }
}