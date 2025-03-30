using easyLib.ADT.Lists;
using easyLib.Extensions;
using easyLib.Test;

namespace easyLibTester.Core.ADT;

sealed class ForwardListTest : UnitTest<ForwardList<int>>
{
    public ForwardListTest() :
        base(nameof(ForwardListTest))
    { }

    //protected:
    protected override IInvariantTester DefineInvariant(ForwardList<int> fl, IInvariantTester invTester) =>
        invTester[fl.IsEmpty == !fl.Any()]
        [fl.IsEmpty == !fl.Nodes.Any()]
        [fl.GetCount() == fl.Nodes.Count()]
        [fl.Nodes.All(n => n.IsReachableFrom(fl.FirstNode))]
        [fl.Nodes.All(n => fl.Contains(n))]
        [fl.All(item => fl.Contains(item))]
        [fl.All(item => fl.Locate(item) != null)]
        [fl.IsEmpty || fl.Skip(1).All(item => fl.Locate(item, fl.FirstNode) != null)]
        [fl.IsEmpty || fl.Skip(1).All(item => fl.LocateForward(item).NextNode != null)]
        [fl.IsEmpty || fl.Skip(1).All(item => fl.LocateForward(item, fl.FirstNode) != null)];

    protected override void Start()
    {
        TestNode();
        TestConstruction();
        TestContains();
        TestLocate();
        TestLocateForward();
        TestPrepend();
        TestInsert();
        TestRemoveFirstNode();
        TestRemoveForward();
        TestRemove();
        TestStripForward();
        TestClear();
    }

    //private:
    void TestClear()
    {
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(SampleFactory.NextByte));
        fl.Clear();
        TestInvariant(fl);
        Ensure(fl.IsEmpty);
    }

    void TestStripForward()
    {
        int count = SampleFactory.NextByte + 1;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(count));
        ForwardList<int>.Node node = fl.Nodes.Shuffle().First();
        ForwardList<int>.Node? oldNextNode = node.NextNode;
        ForwardList<int>.Node?[] delNodes = EnumerableEx.Emit(n => n?.NextNode, node.NextNode, null).ToArray();
        ForwardList<int>.Node? result = fl.StripForward(node);
        TestInvariant(fl);
        Ensure(result == oldNextNode);
        Ensure(result == null || !fl.Contains(result));
        Ensure(node.NextNode == null);
        Ensure(fl.GetCount() == count - delNodes.Length);
        Ensure(delNodes.All(n => !fl.Contains(n!)));
    }

    void TestRemove()
    {
        int count = SampleFactory.NextByte + 1;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(count));
        ForwardList<int>.Node node = fl.Nodes.Shuffle().First();
        int delCount = SampleFactory.CreateInts(0, EnumerableEx.Emit(n => n?.NextNode, node, null).Count() + 1).First();
        ForwardList<int>.Node?[] delNodes = EnumerableEx.Emit(n => n?.NextNode, node, null).Take(delCount).ToArray();
        fl.Remove(node, delCount);
        TestInvariant(fl);
        Ensure(fl.GetCount() == count - delCount);
        Ensure(delCount == 0 || !fl.Contains(node));
        Ensure(delCount == 0 || !delNodes.Any(n => fl.Contains(n!)));
    }

    void TestRemoveForward()
    {
        int count = SampleFactory.NextByte + 1;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(count));
        ForwardList<int>.Node prevNode = fl.Nodes.Shuffle().First();
        ForwardList<int>.Node? oldNextNode = prevNode.NextNode;
        ForwardList<int>.Node?[] nodes = EnumerableEx.Emit(n => n?.NextNode, prevNode.NextNode, null).ToArray();
        int delCount = SampleFactory.CreateInts(0, nodes.Length + 1).First();
        ForwardList<int>.Node? newNextNode = delCount == 0 ? prevNode.NextNode : nodes[delCount - 1]?.NextNode;

        ForwardList<int>.Node? result = fl.RemoveForward(prevNode, delCount);
        TestInvariant(fl);
        Ensure(result == null == (delCount == 0));
        Ensure(delCount == 0 || result == oldNextNode);
        Ensure(prevNode.NextNode == newNextNode);
        Ensure(fl.GetCount() == count - delCount);
        Ensure(nodes.Take(delCount).All(n => !fl.Contains(n!)));
    }

    void TestRemoveFirstNode()
    {
        int count = SampleFactory.NextByte + 1;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(count));
        ForwardList<int>.Node oldFirstNode = fl.FirstNode;
        ForwardList<int>.Node? oldNextNode = fl.FirstNode.NextNode;
        ForwardList<int>.Node result = fl.RemoveFirstNode();
        TestInvariant(fl);
        Ensure(result == oldFirstNode);
        Ensure(result.NextNode == null);
        Ensure(fl.IsEmpty || fl.FirstNode == oldNextNode);
        Ensure(fl.GetCount() == count - 1);
    }

    void TestInsert()
    {
        //Insert(T, Node)
        int item = SampleFactory.NextInt;
        int count = SampleFactory.NextByte + 1;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(count));
        ForwardList<int>.Node prevNode = fl.Nodes.Shuffle().First();
        ForwardList<int>.Node? oldNextNode = prevNode.NextNode;
        ForwardList<int>.Node result = fl.Insert(item, prevNode);
        TestInvariant(fl);
        Ensure(prevNode.NextNode == result);
        Ensure(result.Item == item);
        Ensure(result.NextNode == oldNextNode);
        Ensure(fl.GetCount() == count + 1);

        //Insert(IEnumerable<T>, Node)
        int[] items = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        count = SampleFactory.NextByte + 1;
        fl = new ForwardList<int>(SampleFactory.CreateInts().Take(count));
        prevNode = fl.Nodes.Shuffle().First();
        oldNextNode = prevNode.NextNode;
        ForwardList<int>.Node? result1 = fl.Insert(items.AsEnumerable(), prevNode);
        TestInvariant(fl);
        Ensure(result1 == null == (items.Length == 0));
        Ensure(result1 == null || result1.Item == items[^1]);
        Ensure(result1 == null || prevNode.NextNode!.Item == items[0]);
        Ensure(fl.GetCount() == count + items.Length);
        Ensure(result1 == null || fl.Nodes.SkipWhile(n => n != prevNode.NextNode)
                                           .Select(n => n.Item)
                                           .Take(items.Length)
                                           .SequenceEqual(items));

        //Insert(ReadOnlySpan<T>, Node)
        items = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        count = SampleFactory.NextByte + 1;
        fl = new ForwardList<int>(SampleFactory.CreateInts().Take(count));
        prevNode = fl.Nodes.Shuffle().First();
        oldNextNode = prevNode.NextNode;
        result1 = fl.Insert(items.AsReadOnlySpan(), prevNode);
        TestInvariant(fl);
        Ensure(result1 == null == (items.Length == 0));
        Ensure(result1 == null || result1.Item == items[^1]);
        Ensure(result1 == null || prevNode.NextNode!.Item == items[0]);
        Ensure(fl.GetCount() == count + items.Length);
        Ensure(result1 == null || fl.Nodes.SkipWhile(n => n != prevNode.NextNode)
                                           .Select(n => n.Item)
                                           .Take(items.Length)
                                           .SequenceEqual(items));

        //Insert(Node, Node)
        int nodesCount = SampleFactory.NextByte + 1;
        ForwardList<int>.Node node = new ForwardList<int>(SampleFactory.CreateInts().Take(nodesCount)).FirstNode;
        ForwardList<int>.Node?[] nodes = EnumerableEx.Emit(n => n?.NextNode, node, null).ToArray();
        count = SampleFactory.NextByte + 1;
        fl = new(SampleFactory.CreateInts().Take(count));
        prevNode = fl.Nodes.Shuffle().First();
        oldNextNode = prevNode.NextNode;
        result = fl.Insert(node, prevNode);
        TestInvariant(fl);
        Ensure(result.NextNode == oldNextNode);
        Ensure(prevNode.NextNode == node);
        Ensure(fl.GetCount() == count + nodes.Length);
        Ensure(fl.Nodes.SkipWhile(n => n != prevNode.NextNode)
            .TakeWhile(n => n != result)
            .Append(result)
            .SequenceEqual(nodes));
    }

    void TestPrepend()
    {
        //Prepend(T)
        int item = SampleFactory.NextInt;
        ForwardList<int> fl = new(SampleFactory.CreateInts().Take(SampleFactory.NextByte));
        ForwardList<int>.Node? oldFirstNode = fl.IsEmpty ? null : fl.FirstNode;
        fl.Prepend(item);
        TestInvariant(fl);
        Ensure(!fl.IsEmpty);
        Ensure(fl.FirstNode.Item == item);
        Ensure(fl.FirstNode.NextNode == oldFirstNode);

        //Prepend(IEnumerable< T >)
        var items = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToList();
        int count = SampleFactory.NextByte;
        fl = new(SampleFactory.CreateInts().Take(count));
        oldFirstNode = fl.IsEmpty ? null : fl.FirstNode;
        ForwardList<int>.Node? result = fl.Prepend(items);
        TestInvariant(fl);
        Ensure(result == null == !items.Any());
        Ensure(result == null || result.Item == items.Last());
        Ensure(result == null || fl.FirstNode.Item == items.First());
        Ensure(result == null || result.NextNode == oldFirstNode);
        Ensure(fl.GetCount() == count + items.Count);
        Ensure(fl.Take(items.Count).SequenceEqual(items));

        //Prepend(ReadOnlySpan<T>)
        int[] array = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        count = SampleFactory.NextByte;
        fl = new(SampleFactory.CreateInts().Take(count));
        oldFirstNode = fl.IsEmpty ? null : fl.FirstNode;
        result = fl.Prepend(array.AsReadOnlySpan());
        TestInvariant(fl);
        Ensure(result == null == (array.Length == 0));
        Ensure(result == null || result.Item == array[^1]);
        Ensure(result == null || fl.FirstNode.Item == array[0]);
        Ensure(fl.GetCount() == count + array.Length);
        Ensure(result == null || result.NextNode == oldFirstNode);
        Ensure(fl.Take(array.Length).SequenceEqual(array));

        // Prepend(Node)
        int nodesCount = SampleFactory.NextByte + 1;
        ForwardList<int> fl1 = new(SampleFactory.CreateInts().Take(nodesCount));
        ForwardList<int>.Node node = fl1.FirstNode;
        count = SampleFactory.NextByte;
        fl = new(SampleFactory.CreateInts().Take(count));
        oldFirstNode = fl.IsEmpty ? null : fl.FirstNode;
        ForwardList<int>.Node?[] nodes = EnumerableEx.Emit(n => n?.NextNode, node, null).ToArray();
        result = fl.Prepend(node);
        TestInvariant(fl);
        Ensure(fl.FirstNode == node);
        Ensure(result.NextNode == (count == 0 ? null : oldFirstNode));
        Ensure(fl.GetCount() == count + nodesCount);
        Ensure(fl.Nodes.TakeWhile(n => n != result)
                       .Append(result)
                       .SequenceEqual(nodes));
    }

    void TestLocate()
    {
        //Locate(T, Func<T, T, bool>?)
        Func<int, int, bool> eql = (a, b) => Math.Abs(a) == Math.Abs(b);
        int item = SampleFactory.NextSByte;
        ForwardList<int> fl = new(SampleFactory.CreateSBytes().Select(sb => (int)sb).Take(SampleFactory.NextByte));
        ForwardList<int>.Node? result = fl.Locate(item, eql);
        Ensure(!fl.IsEmpty || result == null);
        Ensure(result != null == fl.Contains(item, eql));
        Ensure(result == null || eql(result.Item, item));
        Ensure(result != null || !fl.Any(e => eql(e, item)));
        Ensure(result == null || fl.Contains(result));

        //Locate(Func<T, bool>)
        result = fl.Locate(n => n < 0);
        Ensure(result == null || fl.Contains(result));
        Ensure(result == null || result.Item < 0);
        Ensure(result != null || !fl.Any(n => n < 0));
        Ensure(result == null || !fl.Nodes.TakeWhile(nd => nd != result).Any(nd => nd.Item < 0));

        //Locate(T, IForwardListNode<T>, Func<T, T, bool>?)
        fl = new(SampleFactory.CreateSBytes().Select(sb => (int)sb).Take(SampleFactory.NextByte + 1));
        ForwardList<int>.Node startNode = fl.Nodes.Shuffle().First();
        result = fl.Locate(item, startNode, eql);
        Ensure(result == null || fl.Contains(item, eql));
        Ensure(result == null || eql(result.Item, item));
        Ensure(result == null || fl.Contains(result));
        Ensure(result == null || result.IsReachableFrom(startNode));
        Ensure(result == null || startNode == result || !startNode.IsReachableFrom(result));

        //Locate(IForwardListNode<T>, Func<T, bool>)
        result = fl.Locate(startNode, n => n < 0);
        Ensure(result == null || fl.Contains(result));
        Ensure(result == null || result.Item < 0);
        Ensure(result != null || !fl.Nodes.SkipWhile(nd => nd != startNode).Any(nd => nd.Item < 0));
        Ensure(result == null || !fl.Nodes.SkipWhile(nd => nd != startNode).TakeWhile(nd => nd != result).Any(nd => nd.Item < 0));
    }

    void TestLocateForward()
    {
        //LocateForward(T, IForwardListNode<T>, Func<T, T, bool>?)
        int item = SampleFactory.NextSByte;
        Func<int, int, bool> eql = (a, b) => Math.Abs(a) == Math.Abs(b);
        ForwardList<int> fl = new(SampleFactory.CreateInts(sbyte.MinValue, sbyte.MaxValue).Take(SampleFactory.NextByte + 1));
        ForwardList<int>.Node startNode = fl.Nodes.Shuffle().First();
        ForwardList<int>.Node result = fl.LocateForward(item, startNode, eql);
        Ensure(fl.Contains(result));
        Ensure(result.NextNode == null || eql(result.NextNode.Item, item));
        Ensure(result.IsReachableFrom(startNode));
        Ensure(result == startNode || !startNode.IsReachableFrom(result));

        //LocateForward(IForwardListNode<T>, Func<T, bool>)
        result = fl.LocateForward(startNode, n => n > 0);
        Ensure(fl.Contains(result));
        Ensure(result.IsReachableFrom(startNode));
        Ensure(result.NextNode == null || result.NextNode.Item > 0);
        Ensure(result.NextNode != null || !fl.Nodes.SkipWhile(nd => nd != startNode).Skip(1).Any(nd => nd.Item > 0));
        Ensure(result.NextNode == null || !fl.Nodes.SkipWhile(nd => nd != startNode.NextNode).
            TakeWhile(nd => nd != result.NextNode).
            Any(nd => nd.Item > 0));

        //LocateForward(T, Func<T, T, bool>)
        item = SampleFactory.NextSByte;
        result = fl.LocateForward(item, eql);
        Ensure(fl.Contains(result));
        Ensure(result.NextNode == null || eql(result.NextNode.Item, item));
        Ensure(result.NextNode != null || eql(fl.FirstNode.Item, item) || !fl.Contains(item, eql));
        Ensure(fl.Nodes.TakeWhile(n => n != result).Skip(1).All(n => !eql(n.Item, item)));

        //LocateForward(Func<T, bool>)
        result = fl.LocateForward(n => n > 0);
        Ensure(fl.Contains(result));
        Ensure(result.NextNode == null || result.NextNode.Item > 0);
        Ensure(result.NextNode != null || !fl.Nodes.Skip(1).Any(nd => nd.Item > 0));
    }

    void TestContains()
    {
        //Contains(T, Func<T, T, bool>?)
        Ensure(!new ForwardList<int>().Contains(SampleFactory.NextInt));

        ForwardList<int> fl = new(SampleFactory.CreateInts(0).Take(SampleFactory.NextByte));
        Ensure(!fl.Contains(-SampleFactory.NextByte - 1));

        Func<int, int, bool> eql = (a, b) => Math.Abs(a) == Math.Abs(b);
        Ensure(fl.All(n => fl.Contains(-n, eql)));

        //Contains(IForwardListNode<T>)
        ForwardList<int>.Node node = new(SampleFactory.NextInt);
        Ensure(!new ForwardList<int>().Contains(node));
        Ensure(!fl.Contains(node));
        Ensure(fl.IsEmpty || !node.IsReachableFrom(fl.FirstNode));

        //Contains(Func<T, bool>)
        Ensure(!fl.Contains(n => n < 0));

        if (!fl.IsEmpty)
        {
            int item = fl.Shuffle().First();
            Ensure(fl.Contains(n => n == item));
        }
    }

    void TestConstruction()
    {
        //ForwardList()
        ForwardList<int> fl = new();
        TestInvariant(fl);
        Ensure(fl.IsEmpty);

        //ForwardList(T)
        int item = SampleFactory.NextInt;
        fl = new(item);
        TestInvariant(fl);
        Ensure(fl.GetCount() == 1);
        Ensure(fl.FirstNode.Item == item);

        //ForwardList(IEnumerable<T>)
        List<int> list = new(SampleFactory.CreateInts().Take(SampleFactory.NextByte));
        fl = new(list);
        TestInvariant(fl);
        Ensure(fl.GetCount() == list.Count);
        Ensure(fl.SequenceEqual(list));

        //ForwardList(ReadOnlySpan<T>)
        int[] array = SampleFactory.CreateInts().Take(SampleFactory.NextByte).ToArray();
        fl = new(array.AsReadOnlySpan());
        TestInvariant(fl);
        Ensure(fl.GetCount() == array.Length);
        Ensure(fl.SequenceEqual(array));

        //ForwardList(Node)
        if (array.Length > 0)
        {
            ForwardList<int> fl1 = new(fl.FirstNode);
            TestInvariant(fl1);
            Ensure(fl1.FirstNode == fl.FirstNode);
            Ensure(fl1.SequenceEqual(array));
        }

        //ForwardList(ForwardList<T>, Func<T, T>?)
        ForwardList<int> fl2 = new(fl);
        TestInvariant(fl2);
        Ensure(fl2.GetCount() == fl.GetCount());
        Ensure(fl2.SequenceEqual(fl));

        Func<string, string> clone = s => new string(s);
        ForwardList<string> fl3 = new(SampleFactory.CreateStrings().Take(SampleFactory.NextByte));
        ForwardList<string> fl4 = new(fl3, clone);
        Ensure(fl4.GetCount() == fl3.GetCount());
        Ensure(fl4.SequenceEqual(fl3));
    }

    void TestNode()
    {
        int item = SampleFactory.NextInt;
        ForwardList<int>.Node node = new(item);
        Ensure(node.Item == item);
        Ensure(node.NextNode == null);

        int item2 = SampleFactory.NextInt;
        ForwardList<int>.Node node2 = new(item2, node);
        Ensure(node2.Item == item2);
        Ensure(node2.NextNode == node);
        Ensure(node.IsReachableFrom(node2));
        Ensure(!node2.IsReachableFrom(node));
    }
}

