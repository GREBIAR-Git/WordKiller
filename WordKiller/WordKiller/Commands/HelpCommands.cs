using System.Windows.Input;

namespace WordKiller.Commands;

public class HelpCommands
{
    private ICommand aboutProgram;

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

    private ICommand documentation;

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
