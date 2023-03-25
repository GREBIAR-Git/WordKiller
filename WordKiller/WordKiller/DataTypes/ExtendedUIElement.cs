﻿using System.Windows;

namespace WordKiller.DataTypes;

class ExtendedUIElement
{
    public UIElement Element { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    public ExtendedUIElement()
    {
        Element = new UIElement();
    }
}
