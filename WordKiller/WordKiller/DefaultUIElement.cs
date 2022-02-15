using System.Windows;

namespace WordKiller
{
    internal class DefaultUIElement
    {
        public UIElement Element { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public DefaultUIElement()
        {
            Element = new UIElement();
        }
    }
}