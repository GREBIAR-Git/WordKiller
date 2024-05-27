using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WordKiller.DataTypes.ParagraphData.Paragraphs;

[Serializable]
public class ParagraphPicture : Numbered, IParagraphData
{
    Bitmap? bitmap;

    [NonSerialized] ImageSource? bitmapImage;

    string data;

    string description;

    public ParagraphPicture(string description, Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.description = description;
        data = description;
    }

    public ParagraphPicture()
    {
        bitmap = new(200, 200);
        description = string.Empty;
        data = description;
    }

    public Bitmap? Bitmap
    {
        get => bitmap;
        set
        {
            SetPropertyDocument(ref bitmap, value);
            bitmapImage = null;
        }
    }

    public ImageSource? BitmapImage
    {
        get => GetBitmapImage();
        set => SetProperty(ref bitmapImage, value);
    }

    public string Type => "Picture";

    public string Data
    {
        get => data;
        set => SetPropertyDocument(ref data, value);
    }

    public string Description
    {
        get => description;
        set => SetPropertyDocument(ref description, value);
    }

    public Visibility DescriptionVisibility => Visibility.Visible;

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
                    BitmapImage = bitmapImage;
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
}