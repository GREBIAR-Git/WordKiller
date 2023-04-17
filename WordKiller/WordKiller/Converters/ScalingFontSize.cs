namespace WordKiller.Converters;

static class ScalingFontSize
{
    public static double Scale(string parameter, double fontSize)
    {
        if (parameter == null)
        {
            return fontSize;
        }
        return double.Parse(parameter) / 16 * fontSize;
    }
}
