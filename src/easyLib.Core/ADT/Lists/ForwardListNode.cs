namespace easyLib.ADT.Lists;

public interface IForwardListNode<T>
{
    ref readonly T? Item { get; }
    IForwardListNode<T>? NextNode { get; }

    bool IsReachableFrom(IForwardListNode<T> node);
}
