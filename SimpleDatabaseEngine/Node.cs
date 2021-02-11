using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class Node
    {
        public List<int> Keys = new List<int>();
        public Node Parent;
        public bool isLeaf = true;
        public List<Node> Children = new List<Node>();
        public int maxNumberOfChild;
        public int minNumberOfChild;
        public int maxNumberOfKeys;
        public int minNumberOfKeys;

        public Node()
        {

        }
        public Node(int value, Node parrent, int treeOrder)
        {
            Parent = parrent;
            Keys.Add(value);
            maxNumberOfChild = treeOrder;
            minNumberOfChild = treeOrder / 2;
            maxNumberOfKeys = treeOrder - 1;
            minNumberOfKeys = (treeOrder / 2) - 1;
            isLeaf = true;
        }

        public Node CreateChild(List<Node> children, List<int> keys, Node parent, bool isLeaf)
        {
            Node node = new Node
            {
                Parent = parent,
                isLeaf = isLeaf,
                Children = children,
                Keys = keys
            };

            foreach (var child in node.Children)
            {
                child.Parent = node;
            }

            return node;
        }

        public bool isFull()
        {
            return (Keys.Count >= 3);
        }

        //Add key in correct order + null check
        public void AddKeyToLeaf(int key  )
        {
            if (Keys.Count > 0 && key < Keys[0])
            {
                Keys.Insert(0, key);
                return;
            }
            for (int i = 0; i < Keys.Count -1; ++i)
            {
                if( key> Keys[i] && key < Keys[i + 1])
                {
                    Keys.Insert(i + 1, key);
                    return;
                }
            }
            Keys.Add(key);
        }
    }
}
