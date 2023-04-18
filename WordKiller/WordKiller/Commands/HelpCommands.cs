using System.Windows.Input;

namespace WordKiller.Commands;

public class HelpCommands
{
    ICommand aboutProgram;

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

    ICommand documentation;

    public ICommand Documentation
    {
        get
        {
            return documentation ??= new RelayCommand(obj =>
            {
                Documentation documentation = new();
                documentation.Show();
            });
        }
    }
}
