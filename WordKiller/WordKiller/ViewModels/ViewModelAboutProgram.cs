namespace WordKiller.ViewModels
{
    public class ViewModelAboutProgram : ViewModelBase
    {
        string mainColor;

        public string MainColor
        {
            get => mainColor;
            set
            {
                SetProperty(ref mainColor, value);
            }
        }

        string additionalColor;

        public string AdditionalColor { get => additionalColor; set => SetProperty(ref additionalColor, value); }

        string alternativeColor;

        public string AlternativeColor { get => alternativeColor; set => SetProperty(ref alternativeColor, value); }

        string hoverColor;

        public string HoverColor { get => hoverColor; set => SetProperty(ref hoverColor, value); }

        public ViewModelAboutProgram()
        {
            mainColor = WordKiller.Properties.Settings.Default.MainColor;
            additionalColor = WordKiller.Properties.Settings.Default.AdditionalColor;
            alternativeColor = WordKiller.Properties.Settings.Default.AlternativeColor;
            hoverColor = WordKiller.Properties.Settings.Default.HoverColor;
        }
    }
}
