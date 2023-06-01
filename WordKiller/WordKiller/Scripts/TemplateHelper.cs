namespace WordKiller.Scripts
{
    internal class TemplateHelper
    {
        public delegate void MethodContainer();

        public static event MethodContainer Change;

        static bool needSave;
        static public bool NeedSave
        {
            get => needSave;
            set
            {
                needSave = value;
                Change();
            }
        }
    }
}
