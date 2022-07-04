using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider_Solitaire
{
    //members are in the same order as in file with statistics
    public enum StatisticType
    {
        //general stats

        GamesStarted,
        GamesWon,
        CardsMoved,
        SuitsAssembled,
        HintsTaken,

        //1 suit games

        OneSuitGamesStarted,
        OneSuitGamesWon,
        OneSpadeSuitsAssembled,
        OneWinPercentage,

        //2 suit games

        TwoSuitGamesStarted,
        TwoSuitGamesWon,
        TwoSpadeSuitsAssembled,
        TwoHeartsSuitsAssembled,
        TwoWinPercentage,

        //4 suit games

        FourSuitGamesStarted,
        FourSuitGamesWon,
        FourSpadeSuitsAssembled,
        FourHeartsSuitsAssembled,
        FourClubSuitsAssembled,
        FourDiamondsSuitsAssembled,
        FourWinPercentage
    }

    //handels updates of stats
    public abstract class Statistics
    {
        //increase general stats
        public static void GamesStarted()
        {
            
        }
    }
}
