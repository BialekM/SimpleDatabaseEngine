namespace SimpleDatabaseEngine
{
    public class BPlusTree
    {
        private int _treeOrder;
        public Node Root = new Node();
        public BPlusTree(int treeOrder, int key)
        {
            _treeOrder = treeOrder;
            Root.TryAddKeyToNode(key);
        }

        public void InsertRoot(int key)
        {
            Node root = new Node(key, null, _treeOrder);
            root.Keys.Add(key);
            root.minNumberOfChild = 2;
            Root = root;
        }

        public void InsertKey(int key)
        {
            if(Root.Keys.Count <= _treeOrder && Root.isLeaf)
                Root.Keys.Add(key);

            if(Root.Keys.Count > _treeOrder)
            {

            }
        }

        public void SplitNode(Node node)
        {
            if (node.Parent == null) //root case
            {
                node.Parent = new Node();
                Root = node.Parent;
            }
            var medianaPosition = node.Keys.Count / 2;
            node.Parent.TryAddKeyToNode(node.Keys[medianaPosition]);
            node.Parent.Children.Add(node.CreateChild(medianaPosition, node.Parent, true));
            node.Parent.Children.Add(node.CreateChild(medianaPosition, node.Parent, false));
            node.Parent.isLeaf = false;
            if (node.Parent.isFull())
                SplitNode(node.Parent);
        }

        public void AddKeyToTree(int key)
        {
            var leaf = GoToLeaf(key, Root);
            leaf.TryAddKeyToNode(key);
            if (leaf.isFull())
                SplitNode(leaf);              
        }

        public Node GoToLeaf(int key, Node node)
        {
            if (node.isLeaf)
            {
                return node;
            }
            else
            {
                for (int i = 0; i < node.Keys.Count; i++)
                {
                    if ((key <= node.Keys[i] && !node.isLeaf) || i == node.Children.Count)
                    {
                        return GoToLeaf(key, node.Children[i]);
                    }
                }
                if (key > node.Keys[node.Keys.Count - 1])
                {
                    return GoToLeaf(key, node.Children[node.Children.Count - 1]);
                }
            }
            return node;
        }
    }
}
