using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WordKiller;

class ElementComboBox
{
    public List<string[]> Data { get; set; }
    public ComboBox Form { get; set; }

    public void Clear()
    {
        Data = new List<string[]>();
    }

    public ElementComboBox(ComboBox form)
    {
        Form = form;
        Data = new List<string[]>();
    }
}
