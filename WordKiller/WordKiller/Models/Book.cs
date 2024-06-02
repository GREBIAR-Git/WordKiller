using System;

namespace WordKiller.Models;

[Serializable]
public class Book : ListOfReferencesResources
{
    public string Authors { get; set; }
    public string Publication { get; set; }
    public string Year { get; set; }
    public string Page { get; set; }

    public override string Full => Authors + " " + Name + ". " + Publication + ", " + Year + ". " + Page + " с.";
}