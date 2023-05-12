using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WordKiller.DataTypes;
using WordKiller.Scripts.File.Encryption;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels;
using WordKiller.Views;

namespace WordKiller.Scripts.ImportExport;

public class WordKillerFile : ViewModelBase
{
    string? savePath;
    public string? SavePath { get => savePath; }

    Visibility visibilitySaveLogo;
    public Visibility VisibilitySaveLogo { get => visibilitySaveLogo; set => SetProperty(ref visibilitySaveLogo, value); }

    Visibility visibilityNeedSaveLogo;
    public Visibility VisibilityNeedSaveLogo { get => visibilityNeedSaveLogo; set => SetProperty(ref visibilityNeedSaveLogo, value); }

    public WordKillerFile()
    {
        SaveHelper.Change += UpdateNeedSave;
        VisibilitySaveLogo = Visibility.Collapsed;
        VisibilityNeedSaveLogo = Visibility.Collapsed;
    }

    void UpdateNeedSave()
    {
        if (SaveHelper.NeedSave)
        {
            VisibilityNeedSaveLogo = Visibility.Visible;
        }
        else
        {
            VisibilityNeedSaveLogo = Visibility.Collapsed;
        }
    }

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
            using MemoryStream stream = new();

            byte[] buffer = new byte[stream.Length];

            stream.ReadAsync(buffer, 0, buffer.Length);
            BinaryFormatter binaryFormatter = new();
            binaryFormatter.Serialize(stream, data);
            string serString = Convert.ToBase64String(stream.ToArray());
            if (Properties.Settings.Default.NumberEncryption != 0)
            {
                IEncryption encryption = new RLEEncryption();
                serString = encryption.Encrypt(serString);
            }
            GenerateStreamFromString(nameFile, Properties.Settings.Default.NumberEncryption + serString);
            SaveHelper.NeedSave = false;
            Thread.Sleep(3000);
            VisibilitySaveLogo = Visibility.Collapsed;
        });
    }

    static void GenerateStreamFromString(string nameFile, string s)
    {
        using FileStream stream = System.IO.File.Open(nameFile, false ? FileMode.Append : FileMode.Create);

        StreamWriter writer = new(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
    }

    public async Task<DocumentData> OpenFile(string fileName)
    {
        DocumentData data = new();
        try
        {
            await Task.Run(() =>
            {
                using (FileStream stream = System.IO.File.Open(fileName, FileMode.Open))
                {
                    if (stream.Length > 0)
                    {
                        using StreamReader reader = new(stream);
                        string fileContents = reader.ReadToEnd();
                        char numberEncryption = fileContents[0];
                        BinaryFormatter binaryFormatter = new();
                        fileContents = fileContents[1..];
                        if (numberEncryption != '0')
                        {
                            IEncryption encryption = new RLEEncryption();
                            fileContents = encryption.Decrypt(fileContents);
                        }
                        MemoryStream stream1 = new(Convert.FromBase64String(fileContents));
                        data = (DocumentData)binaryFormatter.Deserialize(stream1);
                    }
                }
                savePath = fileName;
            });
        }
        catch
        {
            UIHelper.ShowError("10");
        }
        return data;
    }

    public DocumentData? NewFile(DocumentData data)
    {
        if (SaveHelper.NeedSave)
        {
            if (!NeedSave(data))
            {
                return null;
            }
        }
        savePath = string.Empty;
        data = new();
        SaveHelper.NeedSave = true;
        return data;
    }

    public bool NeedSave(DocumentData data)
    {
        MessageNeedSave mns = new();
        mns.ShowDialog();
        if (mns.ViewModel.Number == 0)
        {
            Save(data);
            return true;
        }
        else if (mns.ViewModel.Number == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}