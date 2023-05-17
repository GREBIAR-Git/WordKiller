namespace WordKiller.Scripts;

public static class SaveHelper
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
