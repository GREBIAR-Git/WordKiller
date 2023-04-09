using System.Windows.Input;
using WordKiller.Scripts;
using WordKiller.ViewModels;

namespace WordKiller.Commands.Settings
{
    public class General : ViewModelBase
    {
        ICommand? configFileOpen;

        public ICommand ConfigFileOpen
        {
            get
            {
                return configFileOpen ??= new RelayCommand(obj =>
                {
                    ConfigFile.Open();
                });
            }
        }

        ICommand? configFileDelete;

        public ICommand ConfigFileDelete
        {
            get
            {
                return configFileDelete ??= new RelayCommand(obj =>
                {
                    ConfigFile.Delete();
                });
            }
        }

        ICommand? configFileDeleteAll;

        public ICommand ConfigFileDeleteAll
        {
            get
            {
                return configFileDeleteAll ??= new RelayCommand(obj =>
                {
                    ConfigFile.DeleteAll();
                });
            }
        }
    }
}
