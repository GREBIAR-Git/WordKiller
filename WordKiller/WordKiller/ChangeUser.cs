using System;

namespace WordKiller
{
    internal class ChangeUser
    {
        public static void Start(ElementComboBox elementComboBox)
        {
            for (int i = 0; i < elementComboBox.Data.Count; i++)
            {
                if (elementComboBox.Data[i][1].Contains(":\\Users\\"))
                {
                    string[] directory = elementComboBox.Data[i][1].Split('\\');
                    for (int f = 0; f < directory.Length; f++)
                    {
                        if (directory[f] == "Users")
                        {
                            directory[f + 1] = Environment.UserName;
                            break;
                        }
                    }
                    elementComboBox.Data[i][1] = String.Join("\\", directory);
                }
            }
        }
    }
}