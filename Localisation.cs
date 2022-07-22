using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

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
        MenuLoadGameButton,
        MenuHowToPlayButton,
        MenuStatisticsButton,
        MenuUpdateButton,
        MenuLicenceButton,

        //How to play text
        MenuSPInformationHowToPlayPart01,
        MenuSPInformationHowToPlayPart02,
        MenuSPInformationHowToPlayPart03,
        MenuSPInformationHowToPlayPart04,
        MenuSPInformationHowToPlayPart05,
        MenuSPInformationHowToPlayPart06,
        MenuSPInformationHowToPlayPart07,
        MenuSPInformationHowToPlayPart08,
        MenuSPInformationHowToPlayPart09,
        MenuSPInformationHowToPlayPart10,
        MenuSPInformationHowToPlayPart11,
        MenuSPInformationHowToPlayPart12,
        MenuSPInformationHowToPlayPart13,

        //Statistics text
        MenuSPInformationStatisticsErrorOpeningFile,
        MenuSPInformationStatisticsTitle,
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
        MenuSPInformationStatisticsOneSuitGamesPlayed,
        MenuSPInformationStatisticsOneSuitGamesWon,
        MenuSPInformationStatisticsOneSuitGamesPercentage,
        MenuSPInformationStatisticsTwoSuitGames,
        MenuSPInformationStatisticsTwoSuitGamesPlayed,
        MenuSPInformationStatisticsTwoSuitGamesWon,
        MenuSPInformationStatisticsTwoSuitGamesPercentage,
        MenuSPInformationStatisticsFourSuitGames,
        MenuSPInformationStatisticsFourSuitGamesPlayed,
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
        GameNoMoreMovesPossible,
        GameRestartQuestion,

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
        SettingsResetStatsQuestion,
        SettingsApplyMessage,
        SettingsButtonApply,
        SettingsButtonCancel,
        SettingsHintModeItemEnabled,
        SettingsHintModeItemRestricted,
        SettingsHintModeItemDisabled,
        WindowSettingsButton,
        SettingsRestartOnLangChange,
    }

    public abstract class Localisation
    {
        public static string? SetText(TextType type, string languageFileName)
        {
            try
            {
                string? line = File.ReadLines(@"localisation/" + languageFileName + ".txt").Skip((int)type).Take(1).First().Replace('→', '\n');
                if (line == null || line.Length == 0) throw new FileFormatException();
                else return line;
            } 
            catch (Exception ex)
            {
                MessageBox.Show("There has been an error when trying to access a file of the selected language\n" +
                    "DEBUG localisation file will be used instead\n\n" + ex.ToString(), "Localisation error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return File.ReadLines(@"localisation/DEBUG.txt").Skip((int)type).Take(1).First().Replace('→', '\n'); ;
            }
        }

        public static string GetCurrentLanguage()
        {
            string language = File.ReadLines(@"settings.txt").Skip(4).Take(1).First();
            string[] data = language.Split(' ');
            if (data.Length != 2) throw new FileFormatException();
            return data[1];
        }

        //checks whether the localisation directory exists and contains at least one language
        public static void LocalisationIntegrityCheck()
        {
            if (!Directory.Exists("@localisation")) Directory.CreateDirectory(@"localisation");
            if(Directory.GetFiles(@"localisation").Length==0)
            {
                MessageBox.Show("No localisation file found, reinstall the game please", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
    }
}
