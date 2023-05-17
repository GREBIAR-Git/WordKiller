using System;

namespace WordKiller.Models;

[Serializable]
public class Book
{
    public string Autors { get; set; }
    public string Name { get; set; }
    public string Publication { get; set; }
    public string Year { get; set; }
    public string Page { get; set; }
}
