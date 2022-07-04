using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Spider_Solitaire
{
    //members are in the same order as in file with statistics
    public enum StatisticType
    {
        //general stats

        GamesStarted,   //0 number of total started games
        GamesWon,       //1 number of total won games
        CardsMoved,     //2 number of all the cards that were moved
        SuitsAssembled, //3 number of total suits that have been assembled
        SuitSpadesAssembled,    //4 number of total spade suits assembled
        SuitHeartsAssembled,    //5 number of total Hearts suits assembled
        SuitClubsAssembled,     //6 number of total Clubs suits assembled
        SuitDiamondsAssembled,  //7 number of total diamonds suits assembled
        HintsTaken,     //8 number of total hints taken

        //1 suit games

        OneSuitGamesStarted,    //9 number of games started with 1 suit
        OneSuitGamesWon,        //10 number of games won with 1 suit
        OneWinPercentage,       //11 percantage of winned 1 suit games

        //2 suit games

        TwoSuitGamesStarted,    //12
        TwoSuitGamesWon,        //13
        TwoWinPercentage,       //14

        //4 suit games

        FourSuitGamesStarted,   //15
        FourSuitGamesWon,       //16
        FourWinPercentage       //17
    }

    //handels updates of stats
    public abstract class Statistics
    {
        //increase general stats
        public static void IncreaseStat(StatisticType type, int value)
        {
            if (!File.Exists(@"statistics.txt") || type==StatisticType.OneWinPercentage ||
                                                   type==StatisticType.TwoWinPercentage ||
                                                   type==StatisticType.FourWinPercentage) return;
            try
            {
                var lines = File.ReadAllLines(@"statistics.txt");
                lines[(int)type] = (Convert.ToInt32(lines[(int)type]) + value).ToString();
                File.WriteAllLines(@"statistics.txt",lines);
            }
            catch (Exception e) { MessageBox.Show(e.ToString(),"Error",MessageBoxButton.OK,MessageBoxImage.Error); return; }
        }

        public static void ResetStatistics()
        {
            try
            {
                string[] lines = new string[18];
                for (int i = 0; i < 18; i++) { lines[i] = "0"; }
                File.WriteAllLines(@"statistics.txt", lines);
            }
            catch (Exception e) { MessageBox.Show(e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error); return; }
        }
    }
}
