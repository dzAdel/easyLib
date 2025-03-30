using easyLib.Test.Internal;
using System.Diagnostics;

namespace easyLib.Test;

public sealed partial class TestManager
{
    public bool BreakOnFailure
    {
        get => m_breakOnFailure;
        set
        {
            m_breakOnFailure = value;

            foreach (ITest test in m_tests)
                test.BreakOnFailure = value;
        }
    }

    public IEnumerable<ITest> Tests => m_tests;

    public void AddTest(ITest test)
    {
        require(test != null);

        test.BreakOnFailure = m_breakOnFailure;
        m_tests.Add(test);
    }

    public void Execute(int passCount = 1)
    {
        require(passCount > 0);

        m_reports.Clear();

        if (m_tests.Count == 0)
        {
            Console.WriteLine("No test to execute");
            return;
        }


        Console.WriteLine($"Running {passCount} passes tests.");

        Console.CursorVisible = false;
        Parallel.ForEach(m_tests, runTest);
        Console.CursorVisible = true;

        Console.WriteLine("All tests finished. Press Enter to continue...");
        Console.Read();

        foreach (ITest test in m_tests)
            test.Dispose();

        if (LogRepport(Console.Out) > 0)
        {
            string fName = Path.GetTempFileName() + ".txt";

            using (TextWriter txtWrier = File.CreateText(fName))
            {
                txtWrier.WriteLine(DateTime.Now);
                txtWrier.WriteLine();

                LogRepport(txtWrier);
            }

            var psi = new ProcessStartInfo(fName)
            {
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        //local:
        void runTest(ITest tst)
        {
            List<PassReport> repports = new();

            Console.WriteLine($"Running {tst.Name}...");

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 1; i <= passCount; ++i)
            {
                WriteDebugMessage($"# Starting {tst.Name}: pass {i}.");

                IEnumerable<ITestResult> passLog = tst.Run();

                if (passLog.Any())
                    repports.Add(new PassReport(i, passLog));
            }

            sw.Stop();

            if (repports.Count > 0)
                lock (m_reports)
                    m_reports[tst.Name] = new TestReport(repports);

            Console.WriteLine($"{tst.Name} done. {TimeFormatter.Format(sw.ElapsedMilliseconds)}");
        }
    }



    //private:
    readonly List<ITest> m_tests = new();
    readonly Dictionary<string, TestReport> m_reports = new();
    bool m_breakOnFailure;

    int LogRepport(TextWriter txtWriter)
    {
        int nErr = 0;

        foreach (string key in m_reports.Keys)
        {
            TestReport tstResult = m_reports[key];

            txtWriter.WriteLine($"* * * {key} * * *");

            foreach (PassReport passResult in tstResult.Reports)
            {
                txtWriter.WriteLine('*');
                nErr += passResult.Print(txtWriter);
            }

            txtWriter.WriteLine('*');
            txtWriter.WriteLine($"* * * {key} done. * * *");
            txtWriter.WriteLine();
        }

        txtWriter.WriteLine($"{m_tests.Count} test(s) executed.");
        txtWriter.WriteLine($"{nErr} error(s).");

        return nErr;
    }
}
