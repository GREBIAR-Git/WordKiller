namespace WordKiller.ViewModels;

static class NumberingHelper
{
    public delegate void MethodContainer();

    static bool needUpdate;

    public static bool NeedUpdate
    {
        get => needUpdate;
        set
        {
            needUpdate = value;
            Change();
        }
    }

    public static event MethodContainer Change;
}
