using System;

namespace WordKiller.Models;

[Serializable]
public class Book : ListOfReferencesResources
{
    public string Autors { get; set; }
    public string Publication { get; set; }
    public string Year { get; set; }
    public string Page { get; set; }

    public override string Full => Autors + " " + Name + ". " + Publication + ", " + Year + ". " + Page + " с.";
}