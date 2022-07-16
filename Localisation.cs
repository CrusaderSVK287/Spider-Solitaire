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

        //Menu buttons
        MenuWelcomeBanner,
        MenuOneSuitButton,
        MenuTwoSuitButton,
        MenuFourSuitButton,
        MenuHowToPlayButton,
        MenuStatisticsButton,
        MenuUpdateButton,

    }

    public abstract class Localisation
    {
        public static string? SetText(TextType type, string languageFileName)
        {
            return File.ReadLines(@"localisation/" + languageFileName).Skip((int)type).Take(1).First();
        }
    }
}
