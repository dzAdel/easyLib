using easyLib.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace easyLib.ADT.Lists;

public sealed partial class ForwardList<T> : IReadOnlyForwardList<T>
{
    public ForwardList()
    { }

    public ForwardList(T? item) => m_firstNode = new(item);

    public ForwardList(IEnumerable<T?> items)
    {
        require(items != null);

        using IEnumerator<T?> enumerator = items.GetEnumerator();

        if (enumerator.MoveNext())
        {
            m_firstNode = new(enumerator.Current);

            Node node = m_firstNode;
            int count = 1;

            while (enumerator.MoveNext())
            {
                if (count++ == int.MaxValue)
                    throw new OverflowException();

                node.NextNode = new(enumerator.Current);
                node = node.NextNode;
            }
        }
    }

    public ForwardList(ReadOnlySpan<T?> span)
    {
        int len = span.Length;

        if (len > 0)
        {
            m_firstNode = new(span[0]);

            Node node = m_firstNode;

            for (int i = 1; i < len; ++i)
            {
                node.NextNode = new(span[i]);
                node = node.NextNode;
            }
        }
    }

    public ForwardList(Node node)
    {
        require(node != null);

        m_firstNode = node;
    }

    public ForwardList(ForwardList<T> other, Func<T?, T?>? clone = null)
    {
        require(other != null);

        if (other.m_firstNode != null)
            if (clone == null)
            {
                m_firstNode = new Node(other.m_firstNode.Item);
                Node node = m_firstNode;
                Node? otherNode = other.m_firstNode.NextNode;

                while (otherNode != null)
                {
                    node.NextNode = new(otherNode.Item);
                    otherNode = otherNode.NextNode;
                    node = node.NextNode;
                }
            }
            else
            {
                m_firstNode = new Node(clone(other.m_firstNode.Item));
                Node node = m_firstNode;
                Node? otherNode = other.m_firstNode.NextNode;

                while (otherNode != null)
                {
                    node.NextNode = new(clone(otherNode.Item));
                    otherNode = otherNode.NextNode;
                    node = node.NextNode;
                }
            }
    }

    [MemberNotNullWhen(false, nameof(m_firstNode))]
    public bool IsEmpty => m_firstNode == null;

    public Node FirstNode
    {
        get
        {
            require(!IsEmpty);

            return m_firstNode;
        }
    }

    public IEnumerable<Node> Nodes
    {
        get
        {
            for (Node? node = m_firstNode; node != null; node = node.NextNode)
                yield return node;
        }
    }

    public int GetCount()
    {
        int count = 0;

        for (Node? node = m_firstNode; node != null; node = node.NextNode)
            ++count;

        return count;
    }

    public IEnumerator<T?> GetEnumerator()
    {
        for (Node? node = m_firstNode; node != null; node = node.NextNode)
            yield return node.Item;
    }

    [MemberNotNullWhen(true, nameof(m_firstNode))]
    public bool Contains(T? item, Func<T?, T?, bool>? eql = null)
    {
        if (m_firstNode == null)
            return false;

        eql ??= EqualityComparer<T?>.Default.Equals;

        for (Node? node = m_firstNode; node != null; node = node.NextNode)
            if (eql(node.Item, item))
                return true;

        return false;
    }

    public bool Contains(Func<T?, bool> predicate)
    {
        require(predicate != null);

        for (Node? node = m_firstNode; node != null; node = node.NextNode)
            if (predicate(node.Item))
                return true;

        return false;
    }

    [MemberNotNullWhen(true, nameof(m_firstNode))]
    public bool Contains(IForwardListNode<T>? node) => node != null && m_firstNode != null && node.IsReachableFrom(m_firstNode);

    public Node? Locate(T? item, Func<T?, T?, bool>? eql = null)
    {
        eql ??= EqualityComparer<T?>.Default.Equals;
        Node? node = m_firstNode;

        while (node != null && !eql(node.Item, item))
            node = node.NextNode;

        return node;
    }

    public Node? Locate(Func<T?, bool> predicate)
    {
        require(predicate != null);

        Node? node = m_firstNode;

        while (node != null && !predicate(node.Item))
            node = node.NextNode;

        return node;
    }

    public Node? Locate(T? item, IForwardListNode<T> startNode, Func<T?, T?, bool>? eql = null)
    {
        require(Contains(startNode));

        eql ??= EqualityComparer<T>.Default.Equals;

        for (var node = (Node)startNode; node != null; node = node.NextNode)
            if (eql(node.Item, item))
                return node;

        return null;
    }

    public Node? Locate(IForwardListNode<T> startNode, Func<T?, bool> predicate)
    {
        require(Contains(startNode));
        require(predicate != null);

        for (var node = (Node)startNode; node != null; node = node.NextNode)
            if (predicate(node.Item))
                return node;

        return null;
    }

    public Node LocateForward(T? item, Func<T?, T?, bool>? eql = null)
    {
        require(!IsEmpty);

        return LocateForward(item, m_firstNode, eql);
    }

    public Node LocateForward(Func<T?, bool> predicate)
    {
        require(!IsEmpty);
        require(predicate != null);

        return LocateForward(m_firstNode, predicate);
    }

    public Node LocateForward(T? item, IForwardListNode<T> startNode, Func<T?, T?, bool>? eql = null)
    {
        require(Contains(startNode));

        eql ??= EqualityComparer<T?>.Default.Equals;
        var node = (Node)startNode;

        while (node.NextNode != null && !eql(node.NextNode.Item, item))
            node = node.NextNode;

        return node;
    }

    public Node LocateForward(IForwardListNode<T> startNode, Func<T?, bool> predicate)
    {
        require(Contains(startNode));
        require(predicate != null);

        var node = (Node)startNode;

        while (node.NextNode != null && !predicate(node.NextNode.Item))
            node = node.NextNode;

        return node;
    }

    public void Prepend(T? item)
    {
        require(GetCount() < int.MaxValue);

        m_firstNode = new(item, m_firstNode);
    }

    public Node? Prepend(IEnumerable<T?> items)
    {
        require(items != null);
        //assume GetCount() <= int.MaxValue - items.Count()

        assert(!items.TryGetNonEnumeratedCount(out int count) || GetCount() <= int.MaxValue - count);

        Node? node = null;
        IEnumerator<T?> enumerator = items.GetEnumerator();

        if (enumerator.MoveNext())
        {
            Node firstNode = node = new(enumerator.Current);

            while (enumerator.MoveNext())
            {
                node.NextNode = new(enumerator.Current);
                node = node.NextNode;
            }

            node.NextNode = m_firstNode;
            m_firstNode = firstNode;
        }

        return node;
    }

    public Node? Prepend(ReadOnlySpan<T?> items)
    {
        require(GetCount() <= int.MaxValue - items.Length);

        Node? node = null;
        int len = items.Length;

        if (len > 0)
        {
            Node firstNode = node = new(items[0]);

            for (int i = 1; i < len; ++i)
            {
                node.NextNode = new(items[i]);
                node = node.NextNode;
            }

            node.NextNode = m_firstNode;
            m_firstNode = firstNode;
        }

        return node;
    }

    public Node Prepend(Node node)
    {
        require(node != null);
        require(IsEmpty || !Nodes.Last().IsReachableFrom(node));
        require(EnumerableEx.Emit(n => n?.NextNode, node, null).Count() <= int.MaxValue - GetCount());

        Node curNode = node;

        while (curNode.NextNode != null)
            curNode = curNode.NextNode;

        curNode.NextNode = m_firstNode;
        m_firstNode = node;

        return curNode;
    }

    public Node Insert(T? item, Node prevNode)
    {
        require(Contains(prevNode));
        require(GetCount() < int.MaxValue);

        prevNode.NextNode = new(item, prevNode.NextNode);

        return prevNode.NextNode;
    }

    public Node? Insert(IEnumerable<T?> items, Node prevNode)
    {
        require(items != null);
        require(Contains(prevNode));
        //assume GetCount() <= int.MaxValue - items.Count()

        assert(!items.TryGetNonEnumeratedCount(out int count) || GetCount() <= int.MaxValue - count);

        Node? node = null;
        IEnumerator<T?> enumerator = items.GetEnumerator();

        if (enumerator.MoveNext())
        {
            Node? nextNode = prevNode.NextNode;
            node = prevNode.NextNode = new(enumerator.Current);

            while (enumerator.MoveNext())
            {
                node.NextNode = new(enumerator.Current);
                node = node.NextNode;
            }

            node.NextNode = nextNode;
        }

        return node;
    }

    public Node? Insert(ReadOnlySpan<T?> items, Node prevNode)
    {
        require(Contains(prevNode));
        require(GetCount() <= int.MaxValue - items.Length);

        Node? node = null;
        int len = items.Length;

        if (len > 0)
        {
            Node? nextNode = prevNode.NextNode;
            node = prevNode;

            for (int i = 0; i < len; ++i)
            {
                node.NextNode = new(items[i]);
                node = node.NextNode;
            }

            node.NextNode = nextNode;
        }

        return node;
    }

    public Node Insert(Node node, Node prevNode)
    {
        require(node != null);
        require(!Nodes.Last().IsReachableFrom(node));
        require(Contains(prevNode));
        require(GetCount() <= int.MaxValue - EnumerableEx.Emit(n => n?.NextNode, prevNode, null).Count());

        Node curNode = node;

        while (curNode.NextNode != null)
            curNode = curNode.NextNode;

        curNode.NextNode = prevNode.NextNode;
        prevNode.NextNode = node;

        return curNode;
    }

    public Node RemoveFirstNode()
    {
        require(!IsEmpty);

        Node result = m_firstNode;
        m_firstNode = m_firstNode.NextNode;
        result.NextNode = null;

        return result;
    }

    public void Remove(Node startNode, int count = 1)
    {
        require(Contains(startNode));
        require(count >= 0);
        require(count <= Nodes.SkipWhile(n => n != startNode).Count());

        if (count == 0)
            return;

        if (startNode == m_firstNode)
        {
            for (int i = 1; i < count; i++)
            {
                assert(startNode.NextNode != null);
                startNode = startNode.NextNode;
            }

            m_firstNode = startNode.NextNode;
            startNode.NextNode = null;
        }
        else
        {
            Node prevNode = m_firstNode;

            while (prevNode.NextNode != startNode)
            {
                assert(prevNode.NextNode != null);
                prevNode = prevNode.NextNode;
            }

            RemoveForward(prevNode, count);
        }
    }

    public Node? RemoveForward(Node startNode, int count = 1)
    {
        require(Contains(startNode));
        require(count >= 0);
        require(count < Nodes.SkipWhile(n => n != startNode).Count());

        if (count == 0)
            return null;

        assert(startNode.NextNode != null);
        Node result = startNode.NextNode;
        Node lastNode = result;

        for (int i = 1; i < count; ++i)
        {
            assert(lastNode.NextNode != null);
            lastNode = lastNode.NextNode;
        }

        startNode.NextNode = lastNode.NextNode;
        lastNode.NextNode = null;

        return result;
    }

    public Node? StripForward(Node node)
    {
        require(Contains(node));

        Node? result = node.NextNode;
        node.NextNode = null;

        return result;
    }

    public void Clear() => m_firstNode = null;


    //explicit:
    IForwardListNode<T> IReadOnlyForwardList<T>.FirstNode => FirstNode;
    IEnumerable<IForwardListNode<T>> IReadOnlyForwardList<T>.Nodes => Nodes;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IForwardListNode<T>? IReadOnlyForwardList<T>.Locate(T? item, Func<T?, T?, bool>? eql) => Locate(item, eql);

    IForwardListNode<T>? IReadOnlyForwardList<T>.Locate(T? item, IForwardListNode<T> startNode, Func<T?, T?, bool>? eql) =>
        Locate(item, startNode, eql);

    IForwardListNode<T> IReadOnlyForwardList<T>.LocateForward(T? item, Func<T?, T?, bool>? eql) => LocateForward(item, eql);

    IForwardListNode<T> IReadOnlyForwardList<T>.LocateForward(T? item, IForwardListNode<T> startNode, Func<T?, T?, bool>? eql) =>
        LocateForward(item, startNode, eql);

    IForwardListNode<T>? IReadOnlyForwardList<T>.Locate(Func<T?, bool> predicate) => Locate(predicate);

    IForwardListNode<T>? IReadOnlyForwardList<T>.Locate(IForwardListNode<T> startNode, Func<T?, bool> predicate) =>
        Locate(startNode, predicate);
    IForwardListNode<T> IReadOnlyForwardList<T>.LocateForward(Func<T?, bool> predicate) => LocateForward(predicate);

    IForwardListNode<T> IReadOnlyForwardList<T>.LocateForward(IForwardListNode<T> startNode, Func<T?, bool> predicate) =>
        LocateForward(startNode, predicate);

    //private:
    Node? m_firstNode;
}

