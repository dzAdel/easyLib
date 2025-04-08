using easyLib.Debug.Exceptions;
using easyLib.Debug.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace easyLib.Debug;

public static class DebugHelper
{
    public static DisposableTracker DisposablesTracker => m_disposables ??= new DisposableTracker();

    [System.Diagnostics.Conditional("DEBUG")]
    public static void assert([DoesNotReturnIf(false)] bool condition,
                              string? message = null,
                              [CallerMemberName] string? callerName = null,
                              [CallerFilePath] string? filePath = null,
                              [CallerLineNumber] int lineNber = 0,
                              [CallerArgumentExpression(nameof(condition))] string? condStr = null)
    {
        if (!condition)
        {
            StringBuilder sb = new("\nAssertion failure: ");

            if (message != null)
                sb.Append(message.TrimEnd());

            sb.AppendLine();

            sb.Append("\tMethod: ").Append(callerName).AppendLine();
            sb.Append("\tFile: ").Append(filePath).AppendLine();
            sb.Append("\tLine: ").Append(lineNber).AppendLine();
            sb.Append("\tExpression: ").Append(condStr);

            string msg = sb.ToString();
            System.Diagnostics.Debug.AutoFlush = true;

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.Fail(msg);
            else
                throw new AssertionFailedException(msg);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void require([DoesNotReturnIf(false)] bool condition,
                              string? message = null,
                              [CallerMemberName] string? callerName = null,
                              [CallerFilePath] string? filePath = null,
                              [CallerLineNumber] int lineNber = 0,
                              [CallerArgumentExpression(nameof(condition))] string? condStr = null)
    {
        if (!condition)
        {
            StringBuilder sb = new("\nPrecondition failure: ");

            if (message != null)
                sb.Append(message.TrimEnd());

            sb.AppendLine();

            sb.Append("\tMethod: ").Append(callerName).AppendLine();
            sb.Append("\tFile: ").Append(filePath).AppendLine();
            sb.Append("\tLine: ").Append(lineNber).AppendLine();
            sb.Append("\tExpression: ").Append(condStr);

            string msg = sb.ToString();

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.Fail(msg);
            else
                throw new PreconditionFailedException(msg);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void ensure([DoesNotReturnIf(false)] bool condition,
                              string? message = null,
                              [CallerMemberName] string? callerName = null,
                              [CallerFilePath] string? filePath = null,
                              [CallerLineNumber] int lineNber = 0,
                              [CallerArgumentExpression(nameof(condition))] string? condStr = null)
    {
        if (!condition)
        {
            StringBuilder sb = new("\nPostcondition failure: ");

            if (message != null)
                sb.Append(message.TrimEnd());

            sb.AppendLine();

            sb.Append("\tMethod: ").Append(callerName).AppendLine();
            sb.Append("\tFile: ").Append(filePath).AppendLine();
            sb.Append("\tLine: ").Append(lineNber).AppendLine();
            sb.Append("\tExpression: ").Append(condStr);

            string msg = sb.ToString();

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.Fail(msg);
            else
                throw new PreconditionFailedException(msg);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void WriteDebugMessage(string msg, params string[] lines)
    {
        StringBuilder sb = new();

        sb.AppendLine();

        if (msg != null)
            sb.Append(msg.TrimEnd()).AppendLine();

        for (int i = 0; i < lines.Length; ++i)
        {
            string str = MessageFormatter.Format(1, lines[i]);
            sb.Append(str);
        }

        System.Diagnostics.Debug.Write(sb.ToString());
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void WriteDebugMessage(this Exception ex,
                           string? message = null,
                           [CallerMemberName] string? callerName = null,
                           [CallerFilePath] string? filePath = null,
                           [CallerLineNumber] int lineNber = 0)
    {
        if (ex != null)
        {
            message ??= "An exception occured:";

            StringBuilder sb = new();
            sb.AppendLine()
              .Append(message.TrimEnd())
              .AppendLine()
              .AppendLine($"\tFile; {filePath}")
              .AppendLine($"\tMethod: {callerName}")
              .AppendLine($"\tLine: {lineNber}");

            buildMessage(sb, ex, 1);
            System.Diagnostics.Debug.Write(sb.ToString());


            //local:
            static void buildMessage(StringBuilder sb, Exception ex, int indentCount)
            {
                string spaces = new('\t', indentCount);

                sb.Append(spaces).AppendLine($"Message: {ex.Message}");
                sb.Append(spaces).AppendLine($"Type: {ex.GetType()}.");
                sb.Append(spaces).AppendLine($"Site: {ex.TargetSite}");
                sb.Append(spaces).AppendLine($"Source: {ex.Source}");

                if (ex.StackTrace != null)
                {
                    string str = MessageFormatter.Format(indentCount, "Stack Trace:" + ex.StackTrace);
                    sb.Append(str);
                }

                if (ex.InnerException != null)
                {
                    sb.Append(spaces).AppendLine("Inner exception:");
                    buildMessage(sb, ex.InnerException, indentCount + 1);
                }
            }
        }
    }


    //private:
    static DisposableTracker? m_disposables;
}
