using System;
using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class BPlusTree
    {
        private int _treeOrder;
        public Node Root = new Node();
        public BPlusTree(int treeOrder, int key)
        {
            _treeOrder = treeOrder;
            Root.AddKeyToLeaf(key);
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
            var isNewRoot = false;
            Node parrent = null;
            if (node.Parent == null) //root case
            {
                parrent = new Node();
                Root = parrent;
                isNewRoot = true;
                node.Parent = parrent;
            }
            else
            {
                parrent = node.Parent;
            }            

            var median = GetMedian();
            var newParentKey = node.Keys[median];
            parrent.AddKeyToLeaf(newParentKey);
            var keysForBigger = new List<int>();
            if (node.isLeaf)
            {
                keysForBigger = node.Keys.GetRange(median, node.Keys.Count - median);
            }
            else
            {
                keysForBigger = node.Keys.GetRange(median + 1, node.Keys.Count - median - 1);
            }
            RemoveKeys(median, node);
            List<Node> listofChildrenForBigger = new List<Node>();
            if (node.Children.Count > 0)
            {
                listofChildrenForBigger = node.Children.GetRange(median + 1, node.Children.Count - median -1);
                RemoveChildren(median + 1, node);
            }

            var biggerNode = parrent.CreateChild(listofChildrenForBigger, keysForBigger, parrent, node.isLeaf);



            if (parrent == Root && isNewRoot)
            {
                parrent.Children.Add(node);
            }
            parrent.Children.Add(biggerNode);

            parrent.isLeaf = false;

            if (parrent.isFull())
                SplitNode(parrent);
        }

        private void RemoveKeys(int index, Node node)
        {
            var newKeys = new List<int>();
            for(int i = 0; i < index; i++)
            {
                newKeys.Add(node.Keys[i]);
            }
            node.Keys = newKeys;
        }

        private void RemoveChildren(int index, Node node)
        {
            var newChildren = new List<Node>();
            for(int i = 0; i < index; i++)
            {
                newChildren.Add(node.Children[i]);
            }
            node.Children = newChildren;
        }

        private int GetMedian()
        {
            return (int) Math.Ceiling((float) (_treeOrder + 1) / 2) -1;
        }

        public void AddKeyToTree(int key)
        {
            var leaf = FindLeafNode(key, Root);
            leaf.AddKeyToLeaf(key);
            if (leaf.isFull())
                SplitNode(leaf);              
        }

        public Node FindLeafNode(int key, Node node)
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
                        return FindLeafNode(key, node.Children[i]);
                    }
                }
                if (key > node.Keys[node.Keys.Count - 1])
                {
                    return FindLeafNode(key, node.Children[node.Children.Count - 1]);
                }
            }
            return node;
        }
    }
}
