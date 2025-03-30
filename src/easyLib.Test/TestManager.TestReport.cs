using easyLib.Test.Internal;

namespace easyLib.Test;

partial class TestManager
{
    readonly struct TestReport
    {
        public TestReport(IReadOnlyList<PassReport> repport) => Reports = repport;

        public IReadOnlyList<PassReport> Reports { get; }
    }
}
