namespace WordKiller;

internal static class Config
{
    public const char specialBefore = '◄';
    public const char specialAfter = '►';
    public const string extension = ".wkr";
    public const string content = "Содержимое";

    public static string AddSpecialLeft(string str)
    {
        return specialBefore + str;
    }

    public static string AddSpecialRight(string str)
    {
        return str + specialAfter;
    }

    public static string AddSpecialBoth(string str)
    {
        return specialBefore + str + specialAfter;
    }
}
