using System.Windows.Input;

namespace WordKiller.Commands;

public class NetworkCommans
{
    ICommand createNetwork;

    ICommand joinNetwork;

    ICommand leaveNetwork;

    public ICommand CreateNetwork
    {
        get
        {
            return createNetwork ??= new RelayCommand(
                obj => { });
        }
    }

    public ICommand JoinNetwork
    {
        get
        {
            return joinNetwork ??= new RelayCommand(
                obj => { });
        }
    }

    public ICommand LeaveNetwork
    {
        get
        {
            return leaveNetwork ??= new RelayCommand(
                obj => { });
        }
    }
}