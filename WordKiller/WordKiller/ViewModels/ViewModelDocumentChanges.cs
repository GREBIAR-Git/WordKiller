﻿using System;
using System.Runtime.CompilerServices;
using WordKiller.Scripts.File;

namespace WordKiller.ViewModels;

[Serializable]
public class ViewModelDocumentChanges : ViewModelBase
{
    protected bool SetPropertyDocument<T>(ref T oldValue, T newValue, [CallerMemberName] string property = "")
    {
        bool sp = SetProperty(ref oldValue, newValue, property);
        if (sp)
        {
            SaveHelper.NeedSave = true;
        }

        return sp;
    }
}