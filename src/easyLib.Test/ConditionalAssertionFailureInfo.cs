using easyLib.Test.Internal;

namespace easyLib.Test;

sealed class ConditionalAssertionFailureInfo : AssertionFailureInfo
{
    public ConditionalAssertionFailureInfo(string caption) :
        base(caption)
    { }

    public Type? ExceptionType
    {
        get => m_exType;
        init
        {
            require(value == null || value.IsAssignableTo(typeof(Exception)));

            m_exType = value;
        }
    }

    public string? Action { get; init; }


    //protected:
    protected override IEnumerable<string> GetReport()
    {
        return base.GetReport().Concat(buildReport());


        //local:
        IEnumerable<string> buildReport()
        {
            if (Action != null)
                yield return $"Tested action: {Action}";

            if (ExceptionType != null)
                yield return $"Conditional exception: {ExceptionType}";
        }
    }

    //private:
    readonly Type? m_exType;
}
