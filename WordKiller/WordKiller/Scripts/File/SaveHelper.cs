namespace WordKiller.Scripts.File;

public static class SaveHelper
{
    public delegate void MethodContainer();

    static bool needSave;

    public static bool NeedSave
    {
        get => needSave;
        set
        {
            needSave = value;
            Change();
        }
    }

    public static event MethodContainer Change;
}