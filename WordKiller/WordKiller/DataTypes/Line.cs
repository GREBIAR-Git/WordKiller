using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Xml.Serialization;

namespace WordKiller.DataTypes
{
    [Serializable]
    public class Line
    {
        public bool newPara = false;

        //[XMl]
        public string Text { get; set; }
        public string RunProperties { get; set; }
        public string ParagraphProperties { get; set; }
        public string sectionProperties { get; set; }
        public Line(string text)
        {
            Text = text;
        }

        public Line(string text, string runProperties)
        {
            Text = text;
            RunProperties = runProperties;
        }

        public Line(bool newPara)
        {
            this.newPara = newPara;
        }
        public Line()
        {
            Text = "";
        }


        /*string nameFont;
        string fontSize;
        bool caps;
        bool bold;

        JustificationValues justification;
        int outlineLevel;
        string after;
        string before;
        string line;
        string left;
        string right;
        string firstLine;
        string hanging;

        public void ParagraphProperties(JustificationValues justification, int outlineLevel, string after, string before, string line, string left, string right, string firstLine, string hanging)
        {
            this.justification = justification;
            this.outlineLevel = outlineLevel;
            this.after = after;
            this.before = before;
            this.line = line;
            this.left = left;
            this.right = right;
            this.firstLine = firstLine;
            this.hanging = hanging;
        }

        public Line(string text, string nameFont, string fontSize, bool caps, bool bold, JustificationValues justification, int outlineLevel, string after, string before, string line, string left, string right, string firstLine, string hanging)
        {
            this.text = text;
            this.nameFont = nameFont;
            this.fontSize = fontSize;
            this.caps = caps;
            this.bold = bold;
            this.justification = justification;
            this.outlineLevel = outlineLevel;
            this.after = after;
            this.before = before;
            this.line = line;
            this.left = left;
            this.right = right;
            this.firstLine = firstLine;
            this.hanging = hanging;
        }

        public Line(string text, string nameFont, string fontSize, bool caps, bool bold)
        {
            this.text = text;
            this.nameFont = nameFont;
            this.fontSize = fontSize;
            this.caps = caps;
            this.bold = bold;
        }*/
    }
}
