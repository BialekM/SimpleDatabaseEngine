using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class NodeBuilder
    {
        private readonly Node _node;
        public NodeBuilder()
        {
            _node = new Node();
        }

        public NodeBuilder SetKeys(SortedDictionary<int, string> KvpList)
        {
            _node.KeyValueDictionary = KvpList;
            return this;
        }

        public NodeBuilder SetChildren(List<Node> children)
        {
            _node.Children = children;
            return this;
        }

        public NodeBuilder SetIsLeaf(bool isLeaf)
        {
            _node.IsLeaf = isLeaf;
            return this;
        }

        public NodeBuilder SetParent(Node parent)
        {
            _node.Parent = parent;
            return this;
        }

        public Node GetNode()
        {
            return _node;
        }
    }
}
