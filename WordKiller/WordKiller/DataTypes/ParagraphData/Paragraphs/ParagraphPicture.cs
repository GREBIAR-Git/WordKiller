using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
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

    public Bitmap? Bitmap { get { return bitmap; } set { SetProperty(ref bitmap, value); ; bitmapImage = null; } }

    [NonSerialized]
    ImageSource? bitmapImage;

    public ImageSource? BitmapImage
    {
        get => GetBitmapImage();
        set
        {
            SetProperty(ref bitmapImage, value);
        }
    }

    public void UpdateBitmapImage()
    {
        BitmapImage = GetBitmapImage();
    }

    ImageSource GetBitmapImage()
    {
        if (bitmapImage == null)
        {
            if (bitmap != null)
            {
                using (var memory = new MemoryStream())
                {
                    Bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    this.BitmapImage = bitmapImage;
                    return bitmapImage;
                }
            }
            else
            {
                using (var memory = new MemoryStream())
                {
                    memory.Position = 0;

                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    this.bitmapImage = bitmapImage;
                    return bitmapImage;
                }
            }
        }
        else
        {
            return bitmapImage;
        }
    }

    public Visibility DescriptionVisibility
    {
        get => Visibility.Visible;
    }

    public ParagraphPicture(string description, Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.description = description;
        data = description;
    }

    public ParagraphPicture()
    {
        bitmap = new Bitmap(200, 200);
        description = string.Empty;
        data = description;
    }
}
