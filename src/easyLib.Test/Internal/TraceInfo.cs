namespace easyLib.Test.Internal;

sealed class TraceInfo : ITestResult
{
    public TraceInfo(string? msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
            Caption = "";
        else
        {
            string[] strs = msg.Replace(Environment.NewLine, "\n").
                Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            Caption = strs.Length > 0 ? strs[0] : "";

            for (int i = 1; i < strs.Length; ++i)
                m_lines.Add(strs[i]);
        }
    }

    public string Caption { get; }

    public bool IsFailure => false;

    public IEnumerable<string> Report => m_lines;

    public void AddLine(string line)
    {
        foreach (string s in line.Replace(Environment.NewLine, "\n").
                Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            m_lines.Add(s);
    }

    //private:
    readonly List<string> m_lines = new();
}

