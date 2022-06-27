using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spider_Solitaire
{
    enum CommandType
    {
        select,
        move,
        add
    }
    internal class Command
    {
        private readonly MouseDevice? _mouseDevice;
        public readonly CommandType type;
        public string[]? args;

        public Command(CommandType Type, string[]? Arguments)
        {
            type = Type;
            if (Arguments != null)
            {
                try
                {
                    args = new string[Arguments.Length];
                    Arguments.CopyTo(args, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void LogCommand(Command command)
        {
            try
            {
                switch (command.type)
                {
                    case CommandType.select:
                        if (command.args != null) LogSelect(command.args[0]);
                        break;
                    case CommandType.move:
                        if (command.args != null) LogMove(Convert.ToInt32(command.args[0]));
                        break;
                    case CommandType.add:
                        LogAdd();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void LogSelect(string indexes)
        {
            File.AppendAllText(@"autosave.soli", $"S{indexes}\n");
        }

        private static void LogMove(int index)
        {
            File.AppendAllText(@"autosave.soli", $"M{index}\n");
        }

        private static void LogAdd()
        {
            File.AppendAllText(@"autosave.soli", $"A\n");
        }

        public static void ExecuteSelect(Action<object, MouseButtonEventArgs> CardSelect, string[] args)
        {
            Image img = new() { Name = args[0] };
            CardSelect(img,new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));
        }
        public static void ExecuteMove(Action<object, MouseButtonEventArgs> ColumnClick, string[] args)
        {
            Grid grid = new() { Name = args[0] };
            ColumnClick(grid, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));
        }
        public static void ExecuteAdd(Action<object, MouseButtonEventArgs> NewCardsClick)
        {
            NewCardsClick(new Image(), new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));
        }
    }
}
