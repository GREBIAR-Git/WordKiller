using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordKiller.ViewModels;

[Serializable]
public abstract class ViewModelBase : INotifyPropertyChanged
{
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        if (!Equals(field, newValue))
        {
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        return false;
    }
}
