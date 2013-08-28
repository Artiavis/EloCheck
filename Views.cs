using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EloCheck
{
    public partial class MainWindow : Window
    {
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

        /// <summary>
        /// A class representing the display data which is entered into the 
        /// GamePlayerView class
        /// </summary>
        private class GamePlayerViewModel
        {
            private static string portraitURI = "pack://application:,,,/Resources/splash/";
            private static string medalURI = "pack://application:,,,/Resources/medals/";

            public GamePlayerViewModel(GamePlayer player)
            {
                Name = player.name;
                if (player.champ != null)
                {
                    string champ = player.champ.Replace(" ", string.Empty).Replace("'", string.Empty).ToLowerInvariant();
                    Portrait = new BitmapImage(new Uri(portraitURI + champ + "_square_0.png"));
                }
                else
                    Portrait = null;

                int div = EloCheckUtility.RomanNumeralToInt(player.division);
                string medal = player.tier + (div == -1 ? "" : "_" + div);
                DivisionMedal = new BitmapImage(new Uri(medalURI + medal + ".png"));
            }

            public BitmapImage Portrait { get; private set; }
            public BitmapImage DivisionMedal { get; private set; }
            public string Name { get; private set; }

        }

        private class PlayerStatsView
        {
            public PlayerStatsView(TextBlock name, Image s1medal, Image s2medal, 
                Image s3medal, TextBlock tierDiv, TextBlock games, TextBlock wins,
                TextBlock losses, DataGrid grid)
            {
                Name = name;
                S1Medal = s1medal;
                S2Medal = s2medal;
                S3Medal = s3medal;
                TierDivision = tierDiv;
                TotalGames = games;
                TotalWins = wins;
                TotalLosses = losses;
                PlayerData = grid;
            }

            public PlayerStatsView Load(PlayerStatsViewModel vm)
            {
                Name.Text = vm.Name;
                S1Medal.SetCurrentValue(Image.SourceProperty, vm.S1Medal);
                S2Medal.SetCurrentValue(Image.SourceProperty, vm.S2Medal);
                S3Medal.SetCurrentValue(Image.SourceProperty, vm.S3Medal);
                TierDivision.Text = vm.Tier + " " + vm.Division;
                TotalGames.Text = (vm.Wins + vm.Losses).ToString();
                TotalWins.Text = vm.Wins.ToString();
                TotalLosses.Text = vm.Losses.ToString();

                PlayerData.ItemsSource = vm.RankedStats;
                
                return this;
            }

            public TextBlock Name { get; set; }
            public Image S1Medal { get; set; }
            public Image S2Medal { get; set; }
            public Image S3Medal { get; set; }
            public TextBlock TierDivision { get; set; }
            public TextBlock TotalGames { get; set; }
            public TextBlock TotalWins { get; set; }
            public TextBlock TotalLosses { get; set; }
            public DataGrid PlayerData { get; set; }
        }

        /// <summary>
        /// A class representing the display data which is entered into the 
        /// PlayerStatsView class
        /// </summary>
        private class PlayerStatsViewModel
        {
            private static string portraitURI = "pack://application:,,,/Resources/splash/";
            private static string medalURI = "pack://application:,,,/Resources/medals/";

            public PlayerStatsViewModel(PlayerStats pstats)
            {
                Ranked = pstats.ranked;
                string[] champs = pstats.rankedChampions.Split('_');
                Tier = pstats.tier;
                Division = pstats.division;
                S3Medal = FindMedal(Ranked, Tier, Division);
                S1Medal = FindLegacyMedal(pstats.s1);
                S2Medal = FindLegacyMedal(pstats.s2);
                Name = pstats.player;
                Wins = pstats.wins;
                Losses = pstats.losses;

                RankedStats = pstats.rankedStats.Select((kv) =>
                                new Stats(kv)).ToList();
            }

            private BitmapImage FindLegacyMedal(string tier)
            {
                string medalName = tier.ToLowerInvariant();
                medalName += (!medalName.Equals("unranked") ? "_1.png" : ".png");
                return new BitmapImage(new Uri(medalURI + medalName));
            }

            private BitmapImage FindMedal(bool ranked, string tier, string division)
            {
                if (!ranked)
                    return new BitmapImage(new Uri(medalURI + "unranked.png"));
                else
                {
                    int div = EloCheckUtility.RomanNumeralToInt(division);
                    string medal = tier + (div == -1 ? "" : "_" + div);
                    return new BitmapImage(new Uri(medalURI + medal + ".png"));
                }
            }

            private Dictionary<string, int> NormalizeRankedStats(Dictionary<string, int> stats)
            {
                var newStats = new Dictionary<string, int>();
                foreach(KeyValuePair<string, int> entry in stats)
                {
                    string key = entry.Key.Replace('_', ' ').ToLowerInvariant();
                    newStats.Add(textInfo.ToTitleCase(key), entry.Value);
                }
                return newStats;
            }

            public bool Ranked { get; set; }
            public string Name { get; set; }
            public List<Stats> RankedStats { get; set; }
            public List<BitmapImage> RankedChampions { get; set; }
            public string Tier { get; set; }
            public string Division { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public BitmapImage S1Medal { get; set; }
            public BitmapImage S2Medal { get; set; }
            public BitmapImage S3Medal { get; set; }

            public class Stats
            {
                public Stats(KeyValuePair<string, int> pair)
                {
                    Count = pair.Value;
                    string stat = pair.Key.Replace('_', ' ').ToLowerInvariant();
                    Statistic = textInfo.ToTitleCase(stat);
                }

                public string Statistic { get; set; }
                public int Count { get; set; }

            }
        }
    }
}
