namespace easyLib.Test.Internal;

sealed class RTExceptionInfo : FailureInfo
{
    public RTExceptionInfo(Exception ex) :
        base("Unasserted exception catched")
    {
        require(ex != null);

        Exception = ex;
    }

    public Exception Exception { get; }

    //protected:
    protected override IEnumerable<string> GetReport()
    {
        yield return Exception.Message;

        yield return $"Exception: {Exception.GetType()}";
        yield return $"Target site: {Exception.TargetSite}";

        if (Exception.Source != null)
            yield return $"Source: {Exception.Source}";
    }
}
