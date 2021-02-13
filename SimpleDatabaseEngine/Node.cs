using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class Node
    {
        public List<int> Keys = new List<int>();
        public Node Parent;
        public bool IsLeaf = true;
        public List<Node> Children = new List<Node>();
        public Node RightSibling;
        public Node LeftSibling;

        public Node CreateChild(List<Node> children, List<int> keys, Node parent, bool isLeaf)
        {
            var node = new Node
            {
                Parent = parent,
                IsLeaf = isLeaf,
                Children = children,
                Keys = keys
            };

            foreach (var child in node.Children)
            {
                child.Parent = node;
            }

            return node;
        }

        //Add key in correct order + null check
        public void AddKeyToLeaf(int key  )
        {
            if (Keys.Count > 0 && key < Keys[0])
            {
                Keys.Insert(0, key);
                return;
            }
            for (var i = 0; i < Keys.Count -1; ++i)
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
