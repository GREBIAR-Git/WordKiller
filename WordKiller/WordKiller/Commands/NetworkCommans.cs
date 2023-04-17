using System.Windows.Input;

namespace WordKiller.Commands;

public class NetworkCommans
{
    private ICommand createNetwork;

    public ICommand CreateNetwork
    {
        get
        {
            return createNetwork ??= new RelayCommand(
            obj =>
            {

            });
        }
    }

    private ICommand joinNetwork;

    public ICommand JoinNetwork
    {
        get
        {
            return joinNetwork ??= new RelayCommand(
            obj =>
            {

            });
        }
    }

    private ICommand leaveNetwork;

    public ICommand LeaveNetwork
    {
        get
        {
            return leaveNetwork ??= new RelayCommand(
            obj =>
            {

            });
        }
    }
}
