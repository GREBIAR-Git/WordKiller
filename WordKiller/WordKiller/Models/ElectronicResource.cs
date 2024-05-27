using System;

namespace WordKiller.Models;

[Serializable]
public class ElectronicResource : ListOfReferencesResources
{
    public string Url { get; set; }
    public string CirculationDate { get; set; }

    public override string Full =>
        Name + " [Электронный ресурс]. URL: " + Url + " (дата обращения: " + CirculationDate + ").";
}