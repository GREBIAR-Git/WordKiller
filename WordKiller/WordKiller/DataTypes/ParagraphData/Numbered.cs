using System;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData;

[Serializable]
public class Numbered : ViewModelDocumentChanges
{
    string number = string.Empty;

    public string Number
    {
        get => number;
        set => SetPropertyDocument(ref number, value);
    }
}