using System;

namespace WordKiller.ViewModels
{
    [Serializable]
    public class ViewModelTaskSheet : ViewModelBase
    {
        string sourceData;

        public string SourceData
        {
            get => sourceData;
            set
            {
                SetProperty(ref sourceData, value);
            }
        }

        string toc;

        public string TOC
        {
            get => toc;
            set
            {
                SetProperty(ref toc, value);
            }
        }

        string reportingMaterial;

        public string ReportingMaterial
        {
            get => reportingMaterial;
            set
            {
                SetProperty(ref reportingMaterial, value);
            }
        }

        public ViewModelTaskSheet()
        {
            ReportingMaterial = string.Empty;
            TOC = string.Empty;
            SourceData = string.Empty;
        }
    }
}
