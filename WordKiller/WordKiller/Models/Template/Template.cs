using System;

namespace WordKiller.Models.Template;

[Serializable]
public class Template
{
    public Template(string name, int size = 14, string justify = "left",
        bool bold = false, int before = 0, int after = 0, float lineSpacing = 1, float left = 0, float right = 0,
        float firstLine = 0)
    {
        Name = name;
        Size = size;
        Justify = justify;
        Bold = bold;
        Before = before;
        After = after;
        LineSpacing = lineSpacing;
        Left = left;
        Right = right;
        FirstLine = firstLine;
    }

    public Template()
    {
        Name = string.Empty;
        Size = 14;
        Justify = "left";
        Bold = false;
        Before = 0;
        After = 0;
        LineSpacing = 1;
        Left = 0;
        Right = 0;
        FirstLine = 0;
    }


    public string Name { get; set; }
    public int Size { get; set; }
    public string Justify { get; set; }
    public bool Bold { get; set; }
    public int Before { get; set; }
    public int After { get; set; }
    public float LineSpacing { get; set; }
    public float Left { get; set; }
    public float Right { get; set; }
    public float FirstLine { get; set; }
}