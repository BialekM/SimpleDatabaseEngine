using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class Node
    {
        public List<int> Keys = new List<int>();
        public Node Parent;
        public bool IsLeaf = true;
        public List<Node> Children = new List<Node>();
        public Node NextLeaf;
        public Node PreviousLeaf;

        public Node AppendChild(List<Node> children, List<int> keys, Node parent, bool isLeaf)
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
        public bool TryAddKeyToNode(int key)
        {
            if (Keys.Contains(key))
                return false;
            if (Keys.Count > 0 && key < Keys[0])
            {
                Keys.Insert(0, key);
                return true;
            }
            for (var i = 0; i < Keys.Count - 1; ++i)
            {
                if (key > Keys[i] && key < Keys[i + 1])
                {
                    Keys.Insert(i + 1, key);
                    return true;
                }
            }
            Keys.Add(key);
            return true;
        }

        public void AddChildInCorrectOrder(Node child)
        {
            if (Keys.Count > 0 && child.Keys[0] < Keys[0])
            {
                Children.Insert(0, child);
                return;
            }

            for (var i = 0; i < Keys.Count - 1; ++i)
            {
                if (child.Keys[0] >= Keys[i] && child.Keys[^1] < Keys[i + 1])
                {
                    child.NextLeaf = Children[i + 1];
                    child.PreviousLeaf = child.NextLeaf?.PreviousLeaf;
                    if (child.PreviousLeaf != null) child.PreviousLeaf.NextLeaf = child;
                    if (child.NextLeaf != null) child.NextLeaf.PreviousLeaf = child;
                    Children.Insert(i + 1, child);
                    return;
                }
            }
            Children.Add(child);
        }

        public void ReplaceValueInParent(int key, int newKey)
        {
            if (Parent != null && Parent.Keys.Contains(key))
            {
                Parent.Keys[Parent.Keys.IndexOf(key)] = newKey;
                return;
            }

            Parent?.ReplaceValueInParent(key, newKey);
        }
    }
}
