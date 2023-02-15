using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WordKiller.DataTypes.ParagraphData;

namespace WordKiller.Scripts;

public class ChangeUser<T>
{
    public void Start(ObservableCollection<T> paragraph)
    {
        foreach(IParagraphData item in paragraph)
        {
            if (item.Data.Contains(":\\Users\\"))
            {
                string[] directory = item.Data.Split('\\');
                for (int f = 0; f < directory.Length; f++)
                {
                    if (directory[f] == "Users")
                    {
                        directory[f + 1] = Environment.UserName;
                        break;
                    }
                }
                item.Data = String.Join("\\", directory);
            }
        }
    }
}