using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WordKiller.ViewModels;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphPicture : ViewModelBase, IParagraphData
{
    public string Type { get => "Picture"; }

    string data;

    public string Data { get => data; set => SetProperty(ref data, value); }

    string description;

    public string Description { get => description; set => SetProperty(ref description, value); }

    Bitmap? bitmap;

    public Bitmap? Bitmap { get { return bitmap; } set { bitmap = value; bitmapImage = null; } }

    [NonSerialized]
    BitmapImage? bitmapImage;

    public BitmapImage? BitmapImage { get => GetBitmapImage(); set => bitmapImage = value; }

    BitmapImage GetBitmapImage()
    {
        if (bitmapImage == null)
        {
            if (bitmap != null)
            {
                using (var memory = new MemoryStream())
                {
                    Bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            else
            {
                using (var memory = new MemoryStream())
                {
                    memory.Position = 0;

                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
        }
        else
        {
            return bitmapImage;
        }
    }

    public Visibility DescriptionVisibility()
    {
        return Visibility.Visible;
    }

    public ParagraphPicture(string description, Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.description = description;
        data = description;
    }

    public ParagraphPicture()
    {
        bitmap = new Bitmap(400, 400);
        description = string.Empty;
        data = description;
    }
}
