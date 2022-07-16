using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Spider_Solitaire
{
    public enum TextType
    {
        //  Structure of this enum. Example:
        //  TextType.MainMenuOneSuitButton
        //
        //  TextType = this enum
        //  the name of the enum element consists of two parts
        //  1. where is the control located (in this case MainMenu)
        //  2. name of the control (in this case it's the OneSuitButton)
        //  
        //  therefore this element would be used to set text to OneSuitButton in main menu
        //  
        //  In case there can be multiple text a third part can be added, for example the information box in game
        //  can be set as GameInformationTextHint or GameInformationTextRestart etc.
        //  Another case is when the control is supposed to be wrapped inside another control, for example a scrollviewer.
        //  Than the name of the element should look like this: PageNameOfParentControlNameOfChildControlOptitionalTag
        //
        //  If the text is featured in a Messagebox, just describe the messagebox e.g UpdateErrorOccured 

        //Menu buttons
        MenuWelcomeBanner,
        MenuOneSuitButton,
        MenuTwoSuitButton,
        MenuFourSuitButton,
        MenuHowToPlayButton,
        MenuStatisticsButton,
        MenuUpdateButton,
        MenuSettingsButton,

        //How to play text
        MenuSPInformationHowToPlayPart1,
        MenuSPInformationHowToPlayPart2,
        MenuSPInformationHowToPlayPart3,
        MenuSPInformationHowToPlayPart4,
        MenuSPInformationHowToPlayPart5,
        MenuSPInformationHowToPlayPart6,
        MenuSPInformationHowToPlayPart7,
        MenuSPInformationHowToPlayPart8,
        MenuSPInformationHowToPlayPart9,
        MenuSPInformationHowToPlayPart10,
        MenuSPInformationHowToPlayPart11,
        MenuSPInformationHowToPlayPart12,
        MenuSPInformationHowToPlayPart13,

        //Statistics text
        MenuSPInformationStatisticsErrorOpeningFile,
        MenuSPInformationStatisticsStatisticsTitle,
        MenuSPInformationStatisticsGeneral,
        MenuSPInformationStatisticsGamesStarted,
        MenuSPInformationStatisticsGamesWon,
        MenuSPInformationStatisticsCardsMoved,
        MenuSPInformationStatisticsSuitsAssembled,
        MenuSPInformationStatisticsSpadesAssembled,
        MenuSPInformationStatisticsHeartsAssembled,
        MenuSPInformationStatisticsClubsAssembled,
        MenuSPInformationStatisticsDiamondsAssembled,
        MenuSPInformationStatisticsHintsTaken,
        MenuSPInformationStatisticsOneSuitGames,
        MenuSPInformationStatisticsOneSuitGamesPlyed,
        MenuSPInformationStatisticsOneSuitGamesWon,
        MenuSPInformationStatisticsOneSuitGamesPercentage,
        MenuSPInformationStatisticsTwoSuitGames,
        MenuSPInformationStatisticsTwoSuitGamesPlyed,
        MenuSPInformationStatisticsTwoSuitGamesWon,
        MenuSPInformationStatisticsTwoSuitGamesPercentage,
        MenuSPInformationStatisticsFourSuitGames,
        MenuSPInformationStatisticsFourSuitGamesPlyed,
        MenuSPInformationStatisticsFourSuitGamesWon,
        MenuSPInformationStatisticsFourSuitGamesPercentage,

        //Updating text
        UpdateBoxUpdateAutomaticallyQuestionPart1,
        UpdateBoxUpdateAutomaticallyQuestionPart2,
        UpdateBoxUpdateAutomaticallyUpdaterNotInstalled,

        //Game
        GameInformationTextButtonsHome,
        GameInformationTextButtonsRestart,
        GameInformationTextButtonsUndo,
        GameInformationTextButtonsHint,
        GameInformationTextEmptyColumn,
        GameHintTextBoxNoMoreHints,
        GameHintTextBoxYouHave,
        GameHintTextBoxRemaining,
        GameVictoryText,

        //Settings
        SettingsVisuals,
        SettingsCardSize,
        SettingsCardSpacing,
        SettingsPlayAnimations,
        SettingsNoteAboutScrolling,
        SettingsGameplay,
        SettingsHintMode,
        SettingsLanguage,
        SettingsMiscellaneous,
        SettingsResetStats,
        SettingsDefaultSettings,
        SettingsButtonApply,
        SettingsButtonCancel,
    }

    public abstract class Localisation
    {
        public static string? SetText(TextType type, string languageFileName)
        {
            return File.ReadLines(@"localisation/" + languageFileName).Skip((int)type).Take(1).First();
        }
    }
}
