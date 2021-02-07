using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class Node
    {
        private List<int> Keys = new List<int>();
        private Node Parent;
        private bool isLeaf = true;
        private List<Node> Children;
        private int maxNumberOfChild;
        private int minNumberOfChild;
        private int maxNumberOfKeys;
        private int minNumberOfKeys;


        public Node(int value, Node parrent, int treeOrder)
        {
            Parent = parrent;
            Keys.Add(value);
            maxNumberOfChild = treeOrder;
            minNumberOfChild = treeOrder / 2;
            maxNumberOfKeys = treeOrder - 1;
            minNumberOfKeys = (treeOrder / 2) - 1;

        }
    }
}
