﻿using System;
using System.Diagnostics;
using WordKiller.Commands.Settings;
using WordKiller.Scripts;

namespace WordKiller.ViewModels.Settings;

public class ViewModelGeneralSettings : ViewModelBase
{
    bool associationWKR;

    bool autoHeader;


    bool closeWindow;

    int encodingIndex;

    bool? spellCheckRTB;

    bool syntaxChecking;

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

    public General Commands { get; set; }

    public bool? SpellCheckRTB
    {
        get => spellCheckRTB;
        set => SetProperty(ref spellCheckRTB, value);
    }

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
}