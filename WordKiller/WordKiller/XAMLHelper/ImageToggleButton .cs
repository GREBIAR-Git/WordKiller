using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WordKiller.XAMLHelper
{
    public class ImageToggleButton : RadioButton
    {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public static readonly DependencyProperty ImageSourcePropertyR =
            DependencyProperty.Register("ImageSourceR", typeof(ImageSource), typeof(ImageToggleButton), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get 
                {
                return (ImageSource)GetValue(ImageSourceProperty); 
                }
            set 
                { 
                SetValue(ImageSourceProperty, value); 
                }
        }

        public ImageSource ImageSourceR
        {
            get 
                {
                return (ImageSource)GetValue(ImageSourcePropertyR); 
                }
            set 
                { 
                SetValue(ImageSourcePropertyR, value); 
                }
        }

        
    }
}
