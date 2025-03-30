namespace easyLib.ADT.Lists;

partial class ForwardList<T>
{
    public sealed class Node : IForwardListNode<T>
    {
        public Node(T item, Node? nextNode = null)
        {
            m_item = item;
            NextNode = nextNode;
        }

        public ref T Item => ref m_item;
        public Node? NextNode { get; internal set; }

        public bool IsReachableFrom(IForwardListNode<T> node)
        {
            require(node != null);

            while (node != this && node.NextNode != null)
                node = node.NextNode;

            return node == this;
        }

        ref readonly T IForwardListNode<T>.Item => ref Item;
        IForwardListNode<T>? IForwardListNode<T>.NextNode => NextNode;

        //private:
        T m_item;
    }
}
