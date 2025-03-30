namespace easyLib.ADT.Lists;

public interface IReadOnlyForwardList<T> : IEnumerable<T>
{
    bool IsEmpty { get; }
    IForwardListNode<T> FirstNode { get; }
    IEnumerable<IForwardListNode<T>> Nodes { get; }

    int GetCount();
    bool Contains(T item, Func<T, T, bool>? eql = null);
    bool Contains(Func<T, bool> predicate);
    bool Contains(IForwardListNode<T> node);
    IForwardListNode<T>? Locate(T item, Func<T, T, bool>? eql = null);
    IForwardListNode<T>? Locate(Func<T, bool> predicate);
    IForwardListNode<T>? Locate(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eql = null);
    IForwardListNode<T>? Locate(IForwardListNode<T> startNode, Func<T, bool> predicate);
    IForwardListNode<T> LocateForward(T item, Func<T, T, bool>? eql = null);
    IForwardListNode<T> LocateForward(Func<T, bool> predicate);
    IForwardListNode<T> LocateForward(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eql = null);
    IForwardListNode<T> LocateForward(IForwardListNode<T> startNode, Func<T, bool> predicate);
}