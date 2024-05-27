using System;

namespace WordKiller.Models;

[Serializable]
public abstract class ListOfReferencesResources
{
    public string Id { get; set; }
    public string Name { get; set; }

    public abstract string Full { get; }
}