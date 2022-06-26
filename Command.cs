using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            try { File.AppendAllText(@"autosave.soli", $"S{indexes}\n"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private static void LogMove(int index)
        {
            try { File.AppendAllText(@"autosave.soli", $"M{index}\n"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private static void LogAdd()
        {
            try { File.AppendAllText(@"autosave.soli", $"A\n"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
