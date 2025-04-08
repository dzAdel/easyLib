using System.Text;

namespace easyLib.Debug.Internal;

static class MessageFormatter
{
    public static string Format(int indentCount, params string[] msg)
    {
        StringBuilder sb = new();
        BuildMessage(sb, indentCount, msg);

        return sb.ToString();
    }

    //private:
    static void BuildMessage(StringBuilder sb, int indentCount, params string[] msg)
    {
        string spaces = new('\t', indentCount);

        foreach (string str in msg)
        {
            string[] lines = str.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 0)
            {
                sb.Append(spaces).AppendLine(lines[0]);

                for (int i = 1; i < lines.Length; ++i)
                    BuildMessage(sb, indentCount + 1, lines[i]);
            }
        }
    }
}
