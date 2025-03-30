# interface IForwardListNode\<T>
```csharp
//ver 1
interface IForwardListNode<T>
{
  ref readonly T Item {get;}
  IForwardListNode<T>? NextNode {get;}
  bool IsReachableFrom(IForwardListNode<T> node);
}
```
## Invariant
```csharp
Invariant
{
  this.IsReachableFrom(this);
}
```
## IsReachableFrom(IForwardListNode\<T>)
```csharp
bool IsReachableFrom(IForwardListNode<T> node)
{
  require
  {
    node != null;
  }
  ensure
  {
    !Result || node == this || !node.IsReachableFrom(this); //recursive!!! 
  }
}
```
# interface IReadOnlyForwardList\<T>
```csharp
//ver1
interface IReadOnlyForwardList<T>: IEnumerable<T>
{
  bool IsEmpty {get;}
  IForwardListNode<T> FirstNode {get;}
  IEnumerable<IForwardListNode<T>> Nodes {get;}
  int GetCount();
  bool Contains(T item, Func<T, T, bool>? eqls = null);
  bool Contains(Func<T, bool> predicate);
  bool Contains(IForwardListNode<T> node);
  IForwardListNode<T>? Locate(T item, Func<T, T, bool>? eqls = null);
  IForwardListNode<T>? Locate(Func<T, bool> predicate);  
  IForwardListNode<T>? Locate(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eqls = null);
  IForwardListNode<T>? Locate(IForwardListNode<T> startNode, Func<T, bool> predicate);  
  IForwardListNode<T> LocateForward(T item, Func<T, T, bool>? eqls = null);
  IForwardListNode<T> LocateForward(Func<T, bool> predicate);  
  IForwardListNode<T> LocateForward(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eqls = null);
  IForwardListNode<T> LocateForward(IForwardListNode<T> startNode, Func<T, bool> predicate);
}
```
## Invariant
```csharp
Invariant
{
  IsEmpty == !this.Any();
  IsEmpty == !Nodes.Any();
  Nodes.All(n => n.IsReachableFrom(FirstNode));
  GetCount() == Nodes.Count();
  Nodes.All(n => Contains(n));
  this.All(item => Contains(item));
  this.All(item => Locate(item) != null);
  IsEmpty || this.Skip(1).All(item => Locate(item, FirstNode) != null);
  IsEmpty || this.Skip(1).All(item => LocateFoward(item).NextNode != null);
  IsEmpty || this.Skip(1).All(item => LocateFoward(item, FirstNode).NextNode != null);
}
```
## Contains(Func\<T, bool>)
```csharp
bool Contains(Func<T, bool> predicate)
{
  require
  {
    predicate != null;
  }
}
```
## Contains(IForwardListNode\<T>)
```csharp
bool Contains(IForwardListNode<T> node)
{
  require
  {
    node != null;
  }
  ensure
  {
    IsEmpty || Result == node.IsReachable(FirstNode);
  }
}
```
## FirstNode
```csharp
IForwardListNode<T> FirstNode
{
  get
  {
    require
    {
      !IsEmpty;
    }
  }
}
```
## Locate(Func\<T, bool>)
```csharp
IForwardListNode<T>? Locate(Func<T, bool> predicate)
{
  require
  {
    predicate != null;
  }
  ensure
  {
    Result == null || Contains(Result);
    Result == null || predicate(Result.Item);
    Result != null || !this.Any(e => predicate(e));
    Result == null || !Nodes.TakeWhile(e => e != Result).Any(e => predicate(e.Item));
  }
}
```
## Locate(IForwardListNode\<T>, Func\<T, bool>)
```csharp
IForwardListNode<T>? Locate(IForwardListNode<T> startNode, Func<T, bool> predicate)
{
  require
  {
    startNode != null;
    Contains(startNode);
  }
  ensure
  {
    Result == null || Contains(Result);
    Result == null || predicate(Result.Item);
    Result != null || !Nodes.SkipWhile(nd => nd != result).Any(nd => predicate(nd.Item));
    Result == null || !Nodes.SkipWhile(nd => nd != startNode).TakeWhile(nd => nd != Result).Any(nd => predicate(nd.Item));    
  }
}
```
## Locate(T, Func\<T, T, bool>?)
```csharp
IForwardListNode<T>? Locate(T item, Func<T, T, bool>? eqls = null)
{
  ensure
  {
    !IsEmpty || Result == null;
    Result == null || Contains(Result);
    (Result != null) == Contains(item, eqls);
    Result == null || Result.Item == item;
    Result != null || !this.Any(e => e == item);
    Result == null || !Nodes.TakeWhile(nd => nd != Result).Any(nd => nd.Item == item);
  }
}
```
## Locate(T, IForwardListNode\<T>, Func\<T, T, bool>?)
```csharp
IForwardListNode<T>? Locate(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eqls = null)
{
  require
  {
    startNode != null;
    Contains(startNode);
  }
  ensure
  {
    Result == null || Contains(Result);
    Result == null || Contains(item, eqls);
    Result == null || Result.Item == item;
    Result == null || Result.IsReachableFrom(startNode);
    Result == null || !Nodes.SkipWhile(nd => nd != startNode).TakeWhile(nd => nd != Result).Any(nd => nd.Item == item);
  }
}
```
## LocateForward(Func\<T, bool>)
```csharp
IForwardListNode<T> LocateForward(Func<T, bool> predicate)
{
  require
  {
    !IsEmpty;
    predicate != null;
  }
  ensure
  {
    Contains(Result);
    Result.NextNode == null || predicate(Result.NextNode.Item);
    Result.NextNode != null || !Nodes.Skip(1).Any(nd => predicate(nd.Item));
  }
}
```
## LocateForward(IForwardListNode\<T>, Func\<T, bool>)
```csharp
IForwardListNode<T> LocateForward(IForwardListNode<T> startNode, Func<T, bool> predicate)
{
  require
  {
    startNode != null;
    Contains(startNode);
    predicate != null;
  }
  ensure
  {
    Contains(Result);
    Result.IsReachableFrom(startNode);
    Result.NextNode == null || predicate(Result.NextNode.Item);
    Result.NextNode != null || !Nodes.SkipWhile(nd => nd != startNode).Skip(1).Any(nd => predicate(nd.Item));
    Result.NextNode == null || !Nodes.SkipWhile(nd => nd != startNode.NextNode).TakeWhile(nd => nd != Result.NextNode).Any(nd => predicate(nd.Item));
  }
}
```
## LocateForward(T, Func\<T, T, bool>?)
```csharp
IForwardListNode<T> LocateForward(T item, Func<T, T, bool>? eqls = null)
{
  require
  {
    !IsEmpty;
  }
  ensure
  {
    Contains(Result);
    Result.NextNode == null || Result.NextNode.Item == item;
    Result.NextNode != null || FirstNode.Item == item || !Contains(item,eqls);
    Nodes.TakeWhile(n => n != Result).Skip(1).All(n => n.Item != item);
  }
}
```
## LocateForward(T, IForwardListNode\<T>, Func\<T, T, bool>?)
```csharp
IForardListNode<T> LocateForward(T item, IForwardListNode<T> startNode, Func<T, T, bool>? eqls = null)
{
  require
  {
    startNode != null;
    Contains(startNode);
  }
  ensure
  {
    Contains(Result);
    Result.NextNode == null || Result.NextNode.Item == item;
    Result.IsReachableFrom(startNode);    
    Result.NextNode == null || !Nodes.SkipWhile(nd => nd != startNode).TakeWhile(nd => nd != Result.NextNode).Any(nd => nd.Item == item);
  }
}
```
# class ForwardList\<T>.Node
```csharp
//ver 1
class Node: IForwardListNode<T>
{
  public Node(T item, Node? nextNode);
  public ref T Item {get;}
  public Node? NextNode {get;}
  public bool IsReachableFrom(IForwardListNode<T> node);
}
```
## Node(T, Node?)
```csharp
public Node(T item, Node? nextNode)
{
  ensure
  {
    Item == item;
    NextNode == nextNode;
  }
}
```
## IsReachableFrom(IForwardListNode\<T>)
```csharp
public bool IsReachableFrom(IForwardListNode<T> node)
{
  require
  {
    node != null;
  }
}
```
# class ForwardList\<T>
```csharp
//ver 1
class ForwardList<T>: IReadOnlyForwardList<T>
{
  public ForwardList();
  public ForwardList(T item);
  public ForwardList(IEnumerable<T> items);
  public ForwardList(ReadOnlySpan<T> items);
  public ForwardList(Node node);
  public ForwardList(ForwardList<T> other, Func<T, T>? clone = null);
  public bool IsEmpty {get;}
  public Node FirstNode {get;}
  public IEnumerable<Node> Nodes {get;}
  public IEnumerator<T> GetEnumerator();
  public int GetCount();
  public bool Contains(T item, Func<T, T, bool>? eqls = null);
  public bool Contains(Func<T, bool> predicate);
  public bool Contains(IForwardListNode<T> node);
  public Node? Locate(T item, Func<T, T, bool> eqls = null);
  public Node? Locate(Func<T, bool> predicate); 
  public Node? Locate(T item, IForwardListNode<T> startNode, Func<T, T, bool> eqls = null);
  public Node? Locate(IForwardListNode<T> startNode, Func<T, bool> predicate);
  public Node LocateForward(T item, Func<T, T, bool> eqls = null);
  public Node LocateForward(Func<T, bool> predicate); 
  public Node LocateForward(T item, IForwardListNode<T> startNode, Func<T, T, bool> eqls = null);
  public Node LocateForward(IForwardListNode<T> startNode, Func<T, bool> predicate);
  public void Prepend(T item);
  public Node? Prepend(IEnumerable<T> items);
  public Node? Prepend(ReadOnlySpan<T> items);
  public Node Prepend(Node node);
  public Node Insert(T item, Node prevNode);
  public Node? Insert(IEnumerable<T> items, Node prevNode);
  public Node? Insert(ReadOnlySpan<T> items, Node prevNode);
  public Node Insert(Node node, Node prevNode);
  public Node RemoveFirstNode();
  public void Remove(Node node, int count = 1);
  public Node? RemoveForward(Node node, int count = 1);
  public Node? StripForward(Node node);
  public void Clear();
}
```
## ForwardList()
```csharp
public ForwardList()
{
  ensure
  {
    IsEmpty;    
  }
}
```
## ForwardList(ForwardList\<T>, Func\<T, T>?)
```csharp
public ForwardList(ForwardList<T> other, Func<T, T>? clone = null)
{
  require
  {
    other != null;
  }
  ensure
  {
    GetCount() == other.GetCount();
    clone != null || this.SequenceEqual(other);
  }
}
```
## ForwardList(IEnumerable\<T>)
```csharp
public ForwardList(IEnumerable<T> items)
{
  require
  {
    items != null;
  }
  ensure
  {
    GetCount() == items.Count();
    this.SequenceEqual(items);
  }
  throws
  {
    OverflowException;
  }
}
```
## ForwardList(Node)
```csharp
public ForwardList(Node node)
{
  require
  {
    node != null;
  }
  ensure
  {
    !IsEmpty;
    FirstNode == node;
  }
}
```
## ForwardList(ReadOnlySpan\<T>)
```csharp
public ForwardList(ReadOnlySpan<T> items)
{
  ensure
  {
    GetCount() == items.Length;
    items.ToArray().SequenceEqual(this);
  }
}
```
## ForwardList(T)
```csharp
public ForwardList(T item)
{
  ensure
  {
    GetCount() == 1;
    FirstNode.Item == item;
  }
}
```
## Clear()
```csharp
public void Clear()
{
  ensure
  {
    IsEmpty;
  }
}
```
## Insert(IEnumerable\<T>, Node)
```csharp
public Node? Insert(IEnumerable<T> items, Node prevNode)
{
  require
  {
    items != null;
    prevNode != null;
    Contains(prevNode);
    GetCount() <= int.MaxValue - items.Count();
  }
  ensure
  {
    (Result == null) == !items.Any();
    Result == null || Result.Item == items.Last();
    Result == null || prevNode.NextNode.Item == items.First();
    Result == null || Result.NextNode == old prevNode.NextNode;
    GetCount() == old GetCount() + items.Count();
    Result == null || Nodes.
      SkipWhile(n => n != prevNode.NextNode).
      Take(items.Count()).
      Select(n => n.Item).
      SequenceEqual(items);
  }
}
```
## Insert(Node, Node)
```csharp
public Node Insert(Node node, Node prevNode)
{
  require
  {
    node != null;
    !Nodes.Last().Reachable(node);
    prevNode != null;
    Contains(prevNode);
    GetCount() <= int.MaxValue - EnumerableEx.Emit(n => n.NextNode, node, null).Count();
  }
  ensure
  {
    Result.NextNode == old prevNode.NextNode;
    prevNode.NextNode == node;
    GetCount() == old GetCount() + EnumerableEx.Emit(n => n.NextNode, old node, null).Count();
    Nodes.SkipWhile(n => n != prevNode.NextNode).
      TakeWhile(n => n != Result).
      Append(Result).
      SequenceEqual(EnumerableEx.Emit(n => n.NextNode, old node, null));
  }
}
```
## Insert(ReadOnlySpan\<T>, Node)
```csharp
public Node? Insert(ReadOnlySpan<T> items, Node prevNode)
{
  require
  {
    prevNode != null;
    Contains(prevNode);
    GetCount() <= int.MaxValue - items.Length;
  }
  ensure
  {
    (Result == null) == items.IsEmpty;
    Result == null || prevNode.NextNode.Item == items[0];
    Result == null || Result.Item == items[^1];    
    GetCount() == old GetCount() + items.Length;
    Result == null || Nodes.SkipWhile(n => n != prevNode.NextNode).
      Take(items.Length).
      Select(n => n.Item).
      SequenceEqual(items.ToArray());
  }
}
```
## Insert(T, Node)
```csharp
public Node Insert(T item, Node prevNode)
{
  require
  {
    prevNode != null;
    Contains(prevNode);
    GetCount() < int.MaxValue;
  }
  ensure
  {
    prevNode.NextNode == Result;
    Result.Item == item;
    Result.NextNode == old prevNode.NextNode;
    GetCount() == old GetCount() + 1;
  }
}
```
## Prepend(IEnumerable\<T>)
```csharp
public Node? Prepend(IEnumerable<T> items)
{
  require
  {
    items != null;
    GetCount() <= int.MaxValue - items.Count();
  }
  ensure
  {
    (Result == null) == !items.Any();
    Result == null || Result.Item == items.Last();
    Result == null || FirstNode.Item == items.First();
    Result == null || old IsEmpty || Result.NextNode == old FirstNode;
    Result == null || old !IsEmpty || Result.NextNode == null;
    GetCount() == old GetCount() + items.Count();
    this.Take(items.Count()).SequenceEqual(items);
  }
}
```
## Prepend(Node)
```csharp
public Node Prepend(Node node)
{
  require
  {
    node != null;
    IsEmpty || !Nodes.Last().IsReachable(node);
    EnumerableEx.Emit(n => n.NextNode, node, null).Count() <= int.MaxValue - GetCount();
  }
  ensure
  {
    FirstNode == node;
    Result.NextNode == old IsEmpty ? null : old FirstNode;
    GetCount() == old GetCount() + Nodes.TakeWhile(n => n != Result).Count() + 1; 
    Nodes.TakeWhile(n => n != Result).Append(Result).SequenceEqual(EnumerableEx.Emit(n => n.NextNode, old node, null));
  }
}
```
## Prepend(ReadOnlySpan\<T>)
```csharp
public Node? Prepend(ReadOnlySpan<T> items)
{
  require
  {
    GetCount() <= int.MaxValue - items.Length;
  }
  ensure
  {
    (Result == null) == items.IsEmpty;
    Result == null || Result.Item == items[^1];
    Result == null || FirstNode.Item == items[0];    
    GetCount() == old GetCount() + items.Length;
    Result == null || Result.NextNode == old IsEmpty? null : old FirstNode;
    this.Take(items.Length).SequenceEqual(items.ToArray());
  }
}
```
## Prepend(T)
```csharp
public void Prepend(T item)
{
  require
  {
    GetCount() < Int.MaxValue;
  }
  ensure
  {
    !IsEmpty;
    FirstNode.Item == item;
    GetCount() == 1 || FirstNode.NextNode == old FirstNode;
    GetCount() != 1 || FirstNode.NextNode == null;
  }
}
```
## Remove(Node, int)
```csharp
public void Remove(Node node, int count = 1)
{
  require
  {
    node != null;
    Contains(node);
    0 <= count <= Nodes.SkipWhile(n => n != node).Count();
  }
  ensure
  {
    GetCount() == old GetCount() - count;
    count == 0 || !Contains(node);
    count == 0 || EnumerableEx.Emit(n => n.NextNode, node, null).All(n => !Contains(n));
    count == 0 || EnumerableEx.Emit(n => n.NextNode, node, null).Count() == count;
  }
}
```
## RemoveFirstNode()
```csharp
public Node RemoveFirstNode()
{
  require
  {
    !IsEmpty;
  }
  ensure
  {
    Result == old FirstNode;
    Result.NextNode == null;
    IsEmpty || FirstNode == old FirstNode.NextNode;
    GetCount() == old GetCount() - 1;
  }
}
```
## RemoveForward(Node, int)
```csharp
public Node? RemoveForward(Node node, int count = 1)
{
  require
  {
    node != null;
    Contains(node);
    node.NextNode != null || count == 0;
    0 <= count < Nodes.SkipWhile(n => n != node).Count();
  }
  ensure
  {
    (Result == null) == (count == 0);
    count == 0 || Result == old node.NextNode;
    Result == null || node.NextNode = old Nodes.SkipWhile(n => n != Result).Take(count).NextNode;
    GetCount() == old GetCount() - count;
    EnumerableEx.Emit(n => n.NextNode, Result, null).All(n => !Contains(n));
  }
}
```
## StripForward(Node)
```csharp
public Node? StripForward(Node node)
{
  require
  {
    node != null;
    Contains(node);
  }
  ensure
  {
    Result == old node.NextNode;
    node.NextNode == null;
    Result == null || !Contains(Result);
    GetCount() == old GetCount() - EnumerableEx.Emit(n => n.NextNode, Result, null).Count();
    EnumerableEx.Emit(n => n.NextNode, Result, null).All(n => !Contains(n));
  }
}
```