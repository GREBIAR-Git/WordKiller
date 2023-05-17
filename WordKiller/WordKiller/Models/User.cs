using System;

namespace WordKiller.Models;

[Serializable]
public class User
{
    public string Full => string.Concat(LastName, " ", string.IsNullOrEmpty(FirstName) ? "" : char.ToUpper(FirstName[0]) + ".", string.IsNullOrEmpty(MiddleName) ? "" : char.ToUpper(MiddleName[0]) + ".");
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public string Shifr { get; set; }
    public bool AutoSelected { get; set; }

    public User()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        MiddleName = string.Empty;
        Shifr = string.Empty;
        AutoSelected = false;
    }
}
