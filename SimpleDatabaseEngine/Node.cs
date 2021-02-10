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

        public Node CreateChild(int parrentPosition, Node parent, bool nodeWithLessValue)
        {
            Node node = new Node
            {
                Parent = parent,
                isLeaf = true
            };
            if (nodeWithLessValue)
            {
                for (int i = 0; i <= parrentPosition; i++)
                {
                    node.TryAddKeyToNode(Keys[i]);
                }
            }
            else
            {
                for (int i = parrentPosition + 1; i < Keys.Count; i++)
                {
                    node.TryAddKeyToNode(Keys[i]);
                }
            }

            return node;
        }

        public bool isFull()
        {
            return Keys.Count >= 3;
        }

        //Add key in correct order + null check
        public bool TryAddKeyToNode(int key)
        {
            var result = false;
            if (Keys.Count == 0)
            {
                Keys.Add(key);
                return true;
            }
            else
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] >= key)
                    {
                        if (Keys[i] == key)
                        {
                            result = false;
                            break;
                        }
                        else
                        {
                            Keys.Insert(i, key);
                            result = true;
                            break;
                        }
                    }
                    else if (i == Keys.Count - 1)
                    {
                        Keys.Add(key);
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }
    }
}
