namespace easyLib.Test.Internal;

sealed class PassReport
{
    public PassReport(int passNumber, IEnumerable<ITestResult> results)
    {
        require(passNumber > 0);
        require(results != null);

        PassNumber = passNumber;
        Results = results;
    }

    public int PassNumber { get; }
    public IEnumerable<ITestResult> Results { get; }

    public int Print(TextWriter txtWriter)
    {
        require(txtWriter != null);

        txtWriter.WriteLine($"* Pass: {PassNumber}");

        using IEnumerator<ITestResult> iter = Results.GetEnumerator();
        int nErr = 0;

        if (iter.MoveNext())
        {
            ITestResult tr = iter.Current;
            txtWriter.WriteLine($"*  {tr.Caption}");

            foreach (string str in tr.Report)
                txtWriter.WriteLine($"*\t{str}");

            if (tr.IsFailure)
                ++nErr;

            while (iter.MoveNext())
            {
                txtWriter.WriteLine('*');

                tr = iter.Current;
                txtWriter.WriteLine($"*  {tr.Caption}");

                foreach (string str in tr.Report)
                    txtWriter.WriteLine($"*\t{str}");

                if (tr.IsFailure)
                    ++nErr;
            }
        }

        return nErr;
    }
}
