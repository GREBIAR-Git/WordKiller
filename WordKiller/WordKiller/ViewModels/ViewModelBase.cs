using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WordKiller.ViewModels;

[Serializable]
public abstract class ViewModelBase : INotifyPropertyChanged
{
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string property = "")
    {
        if (oldValue == null && newValue == null) return false;
        if (oldValue != null && newValue != null)
            if (oldValue is byte[] && newValue is byte[])
                if ((oldValue as byte[]).Count() == (newValue as byte[]).Count())
                    if ((newValue as byte[]).SequenceEqual((oldValue as byte[])))
                        return false;
        if (!(oldValue is byte[]) || !(newValue is byte[]))
            if (Equals(oldValue, newValue))
                return false;
        oldValue = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        return true;
    }
}
