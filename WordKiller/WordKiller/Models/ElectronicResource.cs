using System;

namespace WordKiller.Models
{
    [Serializable]
    public class ElectronicResource
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string CirculationDate { get; set; }
    }
}
