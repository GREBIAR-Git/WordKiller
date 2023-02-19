using Microsoft.Win32;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using WordKiller.DataTypes;
using WordKiller.DataTypes.ParagraphData;
using WordKiller.DataTypes.TypeXAML;
using WordKiller.Scripts.File;
using WordKiller.Scripts.ForUI;
using WordKiller.ViewModels;

namespace WordKiller.Scripts.ImportExport;

public class WordKillerFile
{
    string? savePath;

    public string? SavePath { get => savePath;}

    readonly SaveLogo saveLogo;

    public WordKillerFile(Image logo)
    {
        saveLogo = new(logo);
    }

    public bool SavePathExists()
    {
        return !string.IsNullOrEmpty(savePath);
    }

    public void Save(ref DocumentData data)
    {
        if (!string.IsNullOrEmpty(savePath))
        {
            SaveFile(savePath, ref data);
        }
        else
        {
            SaveAs(ref data);
        }
    }

    public bool SaveAs(ref DocumentData data)
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
            SaveFile(savePath, ref data);
            return true;
        }
        return false;
    }

    void SaveFile(string nameFile, ref DocumentData data)
    {
        using (FileStream stream = System.IO.File.Open(nameFile, false ? FileMode.Append : FileMode.Create))
        {
            byte[] buffer = new byte[stream.Length];

            stream.Read(buffer, 0, buffer.Length);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, data);
            saveLogo.Show();
        }

        /*
        if (Properties.Settings.Default.NumberEncoding == 0)
        {
            output.Write("0\r\n" + save);
        }
        else if (Properties.Settings.Default.NumberEncoding == 1)
        {
            output.Write("1\r\n" + Encryption.MegaConvertE(save.ToString()));
        }*/
    }

    public void OpenFile(string fileName, ref DocumentData data, ref ViewModelMain viewModel)
    {
        try
        {
            ClearGlobal(ref data, ref viewModel);
            using (FileStream stream = System.IO.File.Open(fileName, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                data = (DocumentData)binaryFormatter.Deserialize(stream);
            }
            ReferenceComboBox(data, ref viewModel);
            savePath = fileName;
        }
        catch
        {
            MessageBox.Show(UIHelper.FindResourse("Error7"), UIHelper.FindResourse("Error"), MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
        }

        /*
        if (text[0] == '1' && text[1] == '\r' && text[2] == '\n')
        {
            text = Encryption.MegaConvertD(text[3..]);
        }
        else if (text[0] == '0' && text[1] == '\r' && text[2] == '\n')
        {
            text = text[3..];
        }*/
    }

    void ReferenceComboBox(DocumentData data, ref ViewModelMain viewModel)
    {
        foreach (IParagraphData paragraph in data.Paragraphs)
        {
            if (paragraph is ParagraphH1)
            {
                viewModel.H1P.Add(paragraph as ParagraphH1);
            }
            else if (paragraph is ParagraphH2)
            {
                viewModel.H2P.Add(paragraph as ParagraphH2);
            }
            else if (paragraph is ParagraphList)
            {
                viewModel.LP.Add(paragraph as ParagraphList);
            }
            else if (paragraph is ParagraphPicture)
            {
                viewModel.PP.Add(paragraph as ParagraphPicture);
            }
            else if (paragraph is ParagraphTable)
            {
                viewModel.TP.Add(paragraph as ParagraphTable);
            }
            else if (paragraph is ParagraphCode)
            {
                viewModel.CP.Add(paragraph as ParagraphCode);
            }
        }
    }


    public void NewFile(ref DocumentData data, RTBox richTextBox, ref ViewModelMain viewModel)
    {
        NeedSave(ref data, ref viewModel);
        viewModel.WinTitle = "WordKiller";
        ClearGlobal(ref data, ref viewModel);
        richTextBox.Document.Blocks.Clear();
        savePath = string.Empty;
    }

    bool NeedSave(ref DocumentData data, ref ViewModelMain viewModel)
    {
        MessageBoxResult result = MessageBox.Show(UIHelper.FindResourse("Question1"), UIHelper.FindResourse("Question1"), MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);
        if (result == MessageBoxResult.Yes)
        {
            Save(ref data);
            return true;
        }
        return false;
    }

    void ClearGlobal(ref DocumentData data, ref ViewModelMain viewModel)
    {
        data.Paragraphs.Clear();
        data.Title = new();
        data.Properties = new();

        viewModel.H1P.Clear();
        viewModel.H2P.Clear();
        viewModel.LP.Clear();
        viewModel.PP.Clear();
        viewModel.TP.Clear();
        viewModel.CP.Clear();
    }
}