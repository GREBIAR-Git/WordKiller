using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.Enums;
using WordKiller.Models.Template;
using WordKiller.Scripts;

namespace WordKiller.ViewModels.Settings;

public class ViewModelTemplatesSettings : ViewModelBase
{
    ICommand? editPartnersCell;
    ObservableCollection<TemplateType> templateType;

    public ViewModelTemplatesSettings()
    {
        TemplateHelper.Change += UpdateCollection;
        if (Properties.Settings.Default.TemplateTypes.Count == 0)
        {
            TemplateType = [];

            TemplateType template = new(DocumentType.DefaultDocument);
            TemplateType.Add(template);

            template = new(DocumentType.LaboratoryWork);
            TemplateType.Add(template);

            template = new(DocumentType.PracticeWork);
            TemplateType.Add(template);

            template = new(DocumentType.Coursework);
            TemplateType.Add(template);

            template = new(DocumentType.ControlWork);
            TemplateType.Add(template);

            template = new(DocumentType.Referat);
            TemplateType.Add(template);

            template = new(DocumentType.ProductionPractice);
            TemplateType.Add(template);

            template = new(DocumentType.VKR);
            TemplateType.Add(template);
            Properties.Settings.Default.TemplateTypes = TemplateType;
            Properties.Settings.Default.Save();
        }
        else
        {
            TemplateType = Properties.Settings.Default.TemplateTypes;
        }

        TemplateType.CollectionChanged += DataGrid_CollectionChanged;


    }

    public ObservableCollection<TemplateType> TemplateType
    {
        get => templateType;
        set => SetProperty(ref templateType, value);
    }

    public ICommand EditPartnersCell
    {
        get
        {
            return editPartnersCell ??= new RelayCommand(obj =>
            {
                Properties.Settings.Default.TemplateTypes = TemplateType;
                Properties.Settings.Default.Save();
            });
        }
    }

    void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Properties.Settings.Default.TemplateTypes = TemplateType;
        Properties.Settings.Default.Save();
    }

    void UpdateCollection()
    {
        Properties.Settings.Default.TemplateTypes = TemplateType;
        Properties.Settings.Default.Save();
    }
}