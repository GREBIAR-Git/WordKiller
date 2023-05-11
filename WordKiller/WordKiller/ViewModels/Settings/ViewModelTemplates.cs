using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using WordKiller.Commands;
using WordKiller.DataTypes.Enums;
using WordKiller.Models.Template;

namespace WordKiller.ViewModels.Settings
{
    public class ViewModelTemplatesSettings : ViewModelBase
    {
        ObservableCollection<TemplateType> templateType;
        public ObservableCollection<TemplateType> TemplateType
        {
            get => templateType;
            set
            {
                SetProperty(ref templateType, value);
            }
        }

        ICommand? editPartnersCell;
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

        void DataGrid_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Properties.Settings.Default.TemplateTypes = TemplateType;
            Properties.Settings.Default.Save();
        }

        public ViewModelTemplatesSettings()
        {
            if (Properties.Settings.Default.TemplateTypes.Count == 0)
            {
                TemplateType = new ObservableCollection<TemplateType>();

                TemplateType template = new TemplateType(DocumentType.DefaultDocument);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.LaboratoryWork);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.PracticeWork);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.Coursework);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.ControlWork);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.Referat);
                TemplateType.Add(template);

                template = new TemplateType(DocumentType.VKR);
                TemplateType.Add(template);
                Properties.Settings.Default.TemplateTypes = TemplateType;
                Properties.Settings.Default.Save();
            }
            else
            {
                TemplateType = Properties.Settings.Default.TemplateTypes;
            }

            TemplateType.CollectionChanged += new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
        }
    }
}
