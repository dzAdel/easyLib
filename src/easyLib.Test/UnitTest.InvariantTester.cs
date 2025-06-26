using easyLib.Test.Internal;
using System.Runtime.CompilerServices;

namespace easyLib.Test;

partial class UnitTest
{
    sealed class InvariantTester : IInvariantTester
    {
        public InvariantTester(UnitTest unitTest)
        {
            require(unitTest != null);

            m_owner = unitTest;
        }

        public string? CallSite { get; init; }
        public string? SourceFile { get; init; }
        public int LineNumber { get; init; }

        public IInvariantTester this[bool expr,
                                     [CallerLineNumber] int line = 0,
                                     [CallerArgumentExpression(nameof(expr))] string? testExp = null]
        {
            get
            {
                if (!expr)
                {
                    FailureInfo.Expressions.Add((testExp, line));

                    if (m_owner.BreakOnFailure && System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
                }

                return this;
            }
        }

        //private:
        readonly UnitTest m_owner;
        InvariantFailureInfo? m_failureInfo;

        InvariantFailureInfo FailureInfo
        {
            get
            {
                if (m_failureInfo == null)
                {
                    m_failureInfo = new InvariantFailureInfo
                    {
                        CallerName = CallSite,
                        SourceFile = SourceFile,
                        LineNumber = LineNumber
                    };

                    m_owner.m_results.Add(m_failureInfo);
                }

                return m_failureInfo;
            }
        }
    }
}
