using System;
using System.Diagnostics;
using WordKiller.Commands.Settings;
using WordKiller.Scripts;

namespace WordKiller.ViewModels.Settings;

public class ViewModelGeneralSettings : ViewModelBase
{
    public General Commands { get; set; }

    bool? spellCheckRTB;
    public bool? SpellCheckRTB
    {
        get => spellCheckRTB;
        set
        {
            SetProperty(ref spellCheckRTB, value);
        }
    }

    bool associationWKR;
    public bool AssociationWKR
    {
        get => associationWKR;
        set
        {
            SetProperty(ref associationWKR, value);
            bool association = AssociationWKR;
            if (association)
            {
                if (!FileAssociation.IsRunAsAdmin())
                {
                    ProcessStartInfo proc = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Environment.ProcessPath,
                        Verb = "runas"
                    };
                    proc.Arguments += "FileAssociation";
                    try
                    {
                        Process.Start(proc);
                    }
                    catch
                    {
                        UIHelper.ShowError("2");
                    }
                }
                else
                {
                    FileAssociation.Associate("WordKiller");
                }
            }
            else
            {
                if (!FileAssociation.IsRunAsAdmin())
                {
                    ProcessStartInfo proc = new()
                    {
                        UseShellExecute = true,
                        WorkingDirectory = Environment.CurrentDirectory,
                        FileName = Environment.ProcessPath,
                        Verb = "runas"
                    };
                    proc.Arguments += "RemoveFileAssociation";
                    try
                    {
                        Process.Start(proc);
                    }
                    catch
                    {
                        UIHelper.ShowError("2");
                    }
                }
                else
                {
                    FileAssociation.Remove();
                }
            }
        }
    }

    bool syntaxChecking;
    public bool SyntaxChecking
    {
        get => syntaxChecking;
        set
        {
            SetProperty(ref syntaxChecking, value);
            Properties.Settings.Default.SyntaxChecking = SyntaxChecking;
            Properties.Settings.Default.Save();
            SpellCheckRTB = SyntaxChecking;
        }
    }

    int encodingIndex;
    public int EncodingIndex
    {
        get => encodingIndex;
        set
        {
            SetProperty(ref encodingIndex, value);
            Properties.Settings.Default.NumberEncryption = EncodingIndex;
            Properties.Settings.Default.Save();
        }
    }


    bool closeWindow;
    public bool CloseWindow
    {
        get => closeWindow;
        set
        {
            SetProperty(ref closeWindow, value);
            Properties.Settings.Default.CloseWindow = CloseWindow;
            Properties.Settings.Default.Save();
        }
    }

    bool autoHeader;
    public bool AutoHeader
    {
        get => autoHeader;
        set
        {
            SetProperty(ref autoHeader, value);
            Properties.Settings.Default.AutoHeader = AutoHeader;
            Properties.Settings.Default.Save();
        }
    }

    public ViewModelGeneralSettings()
    {
        encodingIndex = Properties.Settings.Default.NumberEncryption;
        closeWindow = Properties.Settings.Default.CloseWindow;
        syntaxChecking = Properties.Settings.Default.SyntaxChecking;
        autoHeader = Properties.Settings.Default.AutoHeader;
        associationWKR = FileAssociation.IsAssociated;
        spellCheckRTB = Properties.Settings.Default.SyntaxChecking;
        Commands = new();
    }
}
