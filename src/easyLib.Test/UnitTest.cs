using easyLib.Test.Internal;
using System.Runtime.CompilerServices;

namespace easyLib.Test;

public abstract partial class UnitTest : ITest
{
    public string Name { get; }

    public bool BreakOnFailure { get; set; }

    public IEnumerable<ITestResult> Run()
    {
        try
        {
            m_results.Clear();
            Start();
        }
        catch (Exception ex)
        {
            m_results.Add(new RTExceptionInfo(ex));

            ex.WriteDebugMessage($"# Unasserted exception catched in unit test: {Name}");
        }

        return m_results.ToArray();
    }

    public void Dispose()
    {
        m_cleaner?.Clean();
        GC.SuppressFinalize(this);
    }

    //protected:
    protected UnitTest(string name)
    {
        require(!string.IsNullOrEmpty(name));

        Name = name;
    }

    protected ITestCleaner Cleaner => m_cleaner ??= new();

    protected void Ensure(bool expr,
                          [CallerMemberName] string? caller = null,
                          [CallerFilePath] string? file = null,
                          [CallerLineNumber] int line = 0,
                          [CallerArgumentExpression(nameof(expr))] string? expStr = null)
    {
        if (!expr)
        {
            var res = new AssertionFailureInfo($"{nameof(Ensure)} test failure in {Name}")
            {
                CallerName = caller,
                SourceFile = file,
                LineNumber = line,
                TestExpression = expStr
            };

            WriteDebugMessage($"# {nameof(Ensure)} test failure in {Name}:",
                $"Call site: {caller}",
                $"File: {file}",
                $"Line number: {line}",
                $"Expression: {expStr}");

            m_results.Add(res);

            if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }

    protected void EnsureThrow<T>(Action action,
                                  [CallerMemberName] string? caller = null,
                                  [CallerFilePath] string? file = null,
                                  [CallerLineNumber] int line = 0,
                                  [CallerArgumentExpression(nameof(action))] string? actStr = null) where T : Exception
    {
        require(action != null);

        try
        {
            action();

            string caption = $"# {nameof(EnsureThrow)}<T> test failure in {Name}";

            var res = new ExceptionFailureInfo(caption)
            {
                CallerName = caller,
                SourceFile = file,
                LineNumber = line,
                Action = actStr,
                ExpectedExceptionType = typeof(T)
            };

            WriteDebugMessage(caption,
                $"Tested action: {actStr}",
                $"Expected exception: {res.ExpectedExceptionType}, got: None.",
                $"Caller site: {caller}",
                $"File: {file}",
                $"Line number: {line}");

            m_results.Add(res);

            if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
        catch (T)
        { }
        catch (Exception ex)
        {
            string caption = $"# {nameof(EnsureThrow)}<T> test failure in {Name}";

            var res = new ExceptionFailureInfo(caption)
            {
                CallerName = caller,
                SourceFile = file,
                LineNumber = line,
                Action = actStr,
                ExpectedExceptionType = typeof(T),
                CatchedException = ex
            };

            WriteDebugMessage(caption,
                $"Tested action: {actStr}",
                $"Expected exception: {res.ExpectedExceptionType}, got: {ex.GetType()}.",
                $"Caller site: {caller}",
                $"File: {file}",
                $"Line number: {line}");

            m_results.Add(res);

            if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }

    protected void EnsureIfThrow<T>(Action action,
                                    Func<bool> fn,
                                    [CallerMemberName] string? caller = null,
                                    [CallerFilePath] string? file = null,
                                    [CallerLineNumber] int line = 0,
                                    [CallerArgumentExpression(nameof(action))] string? actStr = null,
                                    [CallerArgumentExpression(nameof(fn))] string? fnStr = null) where T : Exception
    {
        require(action != null);
        require(fn != null);

        try
        {
            action();
        }
        catch (T)
        {
            if (!fn())
            {
                string caption = $"# {nameof(EnsureIfThrow)}<T> test failure in {Name}";

                var res = new ConditionalAssertionFailureInfo(caption)
                {
                    CallerName = caller,
                    SourceFile = file,
                    LineNumber = line,
                    Action = actStr,
                    TestExpression = fnStr,
                    ExceptionType = typeof(T)
                };

                WriteDebugMessage(caption,
                    $"Conditional exception: {typeof(T)}",
                    $"Caller site: {caller}",
                    $"File: {file}",
                    $"Line number: {line}",
                    $"Tested Action: {actStr}",
                    $"Test expression: {fnStr}");

                m_results.Add(res);

                if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
        }
        catch (Exception ex)
        {
            WriteDebugMessage($"# Smothered exception at {nameof(EnsureIfThrow)}<T> in {Name}",
                $"Conditional assertion on exception: {typeof(T)}",
                $"Exception got: {ex.GetType()}",
                $"Caller site: {caller}",
                $"File: {file}",
                $"Line number: {line}",
                $"Tested Action: {actStr}",
                $"Test expression: {fnStr}");
        }
    }

    protected void EnsureIfThrow(Action action,
                                    Func<bool> fn,
                                    [CallerMemberName] string? caller = null,
                                    [CallerFilePath] string? file = null,
                                    [CallerLineNumber] int line = 0,
                                    [CallerArgumentExpression(nameof(action))] string? actStr = null,
                                    [CallerArgumentExpression(nameof(fn))] string? fnStr = null)
    {
        require(action != null);
        require(fn != null);

        try
        {
            action();
        }
        catch
        {
            if (!fn())
            {
                string caption = $"# {nameof(EnsureIfThrow)} test failure in {Name}";

                var res = new ConditionalAssertionFailureInfo(caption)
                {
                    CallerName = caller,
                    SourceFile = file,
                    LineNumber = line,
                    Action = actStr,
                    TestExpression = fnStr
                };

                WriteDebugMessage(caption,
                    "Conditional exception: any",
                    $"Caller site: {caller}",
                    $"File: {file}",
                    $"Line number: {line}",
                    $"Tested Action: {actStr}",
                    $"Test expression: {fnStr}");

                m_results.Add(res);

                if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
            }
        }
    }

    protected void EnsureNoThrow(Action action,
                                  [CallerMemberName] string? caller = null,
                                  [CallerFilePath] string? file = null,
                                  [CallerLineNumber] int line = 0,
                                  [CallerArgumentExpression(nameof(action))] string? actStr = null)
    {
        require(action != null);

        try
        {
            action();
        }
        catch (Exception ex)
        {
            string caption = $"# {nameof(EnsureNoThrow)} test failure in {Name}";

            var res = new ExceptionFailureInfo(caption)
            {
                CallerName = caller,
                SourceFile = file,
                LineNumber = line,
                Action = actStr,
                CatchedException = ex
            };

            WriteDebugMessage(caption,
                $"Catched exception: {ex.GetType()}.",
                $"Tested action: {actStr}",
                $"Caller site: {caller}",
                $"File: {file}",
                $"Line number: {line}");

            m_results.Add(res);

            if (BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }
    }

    protected IInvariantTester GetInvariantTester([CallerMemberName] string? caller = null,
                                                  [CallerFilePath] string? file = null,
                                                  [CallerLineNumber] int line = 0)
        => new InvariantTester(this)
        {
            CallSite = caller,
            SourceFile = file,
            LineNumber = line
        };

    protected void Trace(string msg, params string[] lines)
    {
        var ti = new TraceInfo(msg);

        foreach (string s in lines)
            ti.AddLine(s);

        m_results.Add(ti);

        WriteDebugMessage(msg, lines);
    }

    protected abstract void Start();

    //private:
    readonly List<ITestResult> m_results = new();
    TestCleaner? m_cleaner;
}
//----------------------------------------------------------------------------

public abstract class UnitTest<T> : UnitTest
{
    //protected:
    protected UnitTest(string name) :
        base(name)
    { }

    protected void TestInvariant(T obj,
                                 [CallerMemberName] string? caller = null,
                                 [CallerFilePath] string? file = null,
                                 [CallerLineNumber] int line = 0)
    {
        IInvariantTester invTester = GetInvariantTester(caller, file, line);
        DefineInvariant(obj, invTester);
    }

    protected abstract IInvariantTester DefineInvariant(T obj, IInvariantTester invTester);
}
