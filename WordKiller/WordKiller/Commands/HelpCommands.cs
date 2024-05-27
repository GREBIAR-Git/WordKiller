using System.Diagnostics;
using System.Windows.Input;

namespace WordKiller.Commands;

public class HelpCommands
{
    ICommand aboutProgram;

    ICommand documentation;

    public ICommand AboutProgram
    {
        get
        {
            return aboutProgram ??= new RelayCommand(
                obj =>
                {
                    AboutProgram aboutProgram = new();
                    aboutProgram.Show();
                });
        }
    }

    public ICommand Documentation
    {
        get
        {
            return documentation ??= new RelayCommand(obj =>
            {
                Process.Start(
                    new ProcessStartInfo("https://github.com/GREBIAR-Git/WordKiller/blob/master/README.md")
                        { UseShellExecute = true });
            });
        }
    }
}