using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Collections.Generic;
using System.Windows.Controls;

namespace WordKiller;

class DataComboBox
{
    public Dictionary<string, ElementComboBox> ComboBox { get; set; }
    public string Text { get; set; }

    public ElementComboBox SearchComboBox(ComboBox comboBoxForm)
    {
        foreach (KeyValuePair<string, ElementComboBox> comboBox in ComboBox)
        {
            if (comboBox.Value.Form == comboBoxForm)
            {
                return comboBox.Value;
            }
        }
        return null;
    }

    public int Sum()
    {
        int sum = 0;
        foreach (KeyValuePair<string, ElementComboBox> comboBox in ComboBox)
        {
            sum += comboBox.Value.Data.Count;
        }
        return sum;
    }

    public void AllClear()
    {
        foreach (KeyValuePair<string, ElementComboBox> comboBox in ComboBox)
        {
            comboBox.Value.Clear();
        }
    }

    public DataComboBox(ComboBox h1, ComboBox h2, ComboBox l, ComboBox p, ComboBox t, ComboBox c)
    {
        ComboBox = new Dictionary<string, ElementComboBox>
        {
            ["h1"] = new ElementComboBox(h1),
            ["h2"] = new ElementComboBox(h2),
            ["l"] = new ElementComboBox(l),
            ["p"] = new ElementComboBox(p),
            ["t"] = new ElementComboBox(t),
            ["c"] = new ElementComboBox(c)
        };
        Text = string.Empty;
    }
}
