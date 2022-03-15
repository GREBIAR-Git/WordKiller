using System.Collections.Generic;
using System.Windows.Controls;

namespace WordKiller;

class ElementComboBox
{
    public List<string[]> Data { get; set; }
    public ComboBox Form { get; set; }

    public ElementComboBox(ComboBox form)
    {
        Form = form;
        Data = new List<string[]>();
    }
}
