using System.Text;

namespace easyLib.Test.Internal;

static class TimeFormatter
{
    public static string Format(long ms)
    {
        require(ms >= 0);

        var ts = TimeSpan.FromMilliseconds(ms);
        var sb = new StringBuilder("(");

        if (ts.Hours > 0)
            sb.Append(ts.Hours).Append("h:").Append(ts.Minutes).Append("m:").Append(ts.Seconds).Append('s');
        else if (ts.Minutes > 0)
            sb.Append(ts.Minutes).Append("m:").Append(ts.Seconds).Append('s');
        else if (ts.Seconds > 0)
            sb.Append(ts.Seconds).Append("s:").Append(ts.Milliseconds).Append("ms");
        else
            sb.Append(ts.Milliseconds).Append("ms");

        sb.Append(')');

        return sb.ToString();
    }
}
