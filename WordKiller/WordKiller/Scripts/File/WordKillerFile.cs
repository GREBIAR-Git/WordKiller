using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using WordKiller.DataTypes;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels;

namespace WordKiller.Scripts.ImportExport;

public class WordKillerFile : ViewModelBase
{
    string? savePath;
    public string? SavePath { get => savePath; }

    Visibility visibilitySaveLogo;
    public Visibility VisibilitySaveLogo { get => visibilitySaveLogo; set => SetProperty(ref visibilitySaveLogo, value); }

    public bool SavePathExists()
    {
        return !string.IsNullOrEmpty(savePath);
    }

    public void Save(DocumentData data)
    {
        if (!string.IsNullOrEmpty(savePath))
        {
            SaveFile(savePath, data);
        }
        else
        {
            SaveAs(data);
        }
    }

    public bool SaveAs(DocumentData data)
    {
        SaveFileDialog saveFileDialog = new()
        {
            OverwritePrompt = true,
            Filter = "wordkiller file (*" + Properties.Settings.Default.Extension + ")|*" + Properties.Settings.Default.Extension + "|All files (*.*)|*.*",
            FileName = "1"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            savePath = saveFileDialog.FileName;
            SaveFile(savePath, data);
            return true;
        }
        return false;
    }

    async void SaveFile(string nameFile, DocumentData data)
    {
        VisibilitySaveLogo = Visibility.Visible;
        await Task.Run(() =>
        {
            using (FileStream stream = System.IO.File.Open(nameFile, false ? FileMode.Append : FileMode.Create))
            {
                byte[] buffer = new byte[stream.Length];

                stream.ReadAsync(buffer, 0, buffer.Length);
                BinaryFormatter binaryFormatter = new();
                binaryFormatter.Serialize(stream, data);
            }
        });
        VisibilitySaveLogo = Visibility.Collapsed;
    }

    public DocumentData OpenFile(string fileName)
    {
        DocumentData data = new();
        try
        {
            using (FileStream stream = System.IO.File.Open(fileName, FileMode.Open))
            {
                if (stream.Length == 0)
                {

                }
                else
                {
                    var binaryFormatter = new BinaryFormatter();
                    data = (DocumentData)binaryFormatter.Deserialize(stream);
                }
            }
            savePath = fileName;
        }
        catch
        {
            MessageBox.Show(UIHelper.FindResourse("Error8"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
        }
        return data;
    }

    public DocumentData NewFile()
    {
        DocumentData data = new();
        NeedSave(data);
        data.Clear();
        savePath = string.Empty;
        return data;
    }

    bool NeedSave(DocumentData data)
    {
        MessageBoxResult result = MessageBox.Show(UIHelper.FindResourse("Question1"), UIHelper.FindResourse("Question1"), MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
        if (result == MessageBoxResult.Yes)
        {
            Save(data);
            return true;
        }
        return false;
    }
}