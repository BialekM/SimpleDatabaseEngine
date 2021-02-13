using System;
using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public class BPlusTree
    {
        private readonly int _treeOrder;
        private int _maxNumberOfKey;
        private int _minNumberOfKey;
        private int _minimumNumberOfChild;
        public Node Root = new Node();
        public BPlusTree(int treeOrder, int key)
        {
            _treeOrder = treeOrder;
            _minNumberOfKey = (int)Math.Ceiling((float)_treeOrder / 2) - 1;
            _maxNumberOfKey = _treeOrder - 1;
            _minimumNumberOfChild = _treeOrder / 2;
            Root.AddKeyToLeaf(key);
        }

        public void SplitNode(Node node)
        {
            var listOfChildrenForBigger = new List<Node>();
            var isNewRoot = false;
            Node parent;
            
            //root case
            if (node.Parent == null) 
            {
                parent = new Node();
                Root = parent;
                isNewRoot = true;
                node.Parent = parent;
            }
            else
            {
                parent = node.Parent;
            }            

            //left child is old node
            var median = GetMedian();
            var newParentKey = node.Keys[median];
            //if node is leaf rewrite the median key to parent
            //otherwise skip it
            var keysForBigger = node.IsLeaf
                ? node.Keys.GetRange(median, node.Keys.Count - median)
            : node.Keys.GetRange(median + 1, node.Keys.Count - median - 1);
            
            parent.AddKeyToLeaf(newParentKey);
            //Remove Keys from left leaf
            RemoveKeys(median, node);
            
            //Create list for BiggerLeaf + Remove children from left (old node)
            if (node.Children.Count > 0)
            {
                listOfChildrenForBigger = node.Children.GetRange(median + 1, node.Children.Count - median -1);
                //Remove Keys from left leaf
                RemoveChildren(median + 1, node);
            }

            //Create Child for bigger Node and set Parent to children
            var biggerNode = parent.CreateChild(listOfChildrenForBigger, keysForBigger, parent, node.IsLeaf);

            //Set right parent for leaf
            if (node.IsLeaf)
            {
                node.RightSibling = biggerNode;
                node.RightSibling.LeftSibling = node;
            }

            //special case when parent is just created new root
            if (parent == Root && isNewRoot) 
                parent.Children.Add(node);

            //Add Right Child
            parent.Children.Add(biggerNode);

            //Parent have children - can't be leaf
            parent.IsLeaf = false;

            //check if parent should be split again
            if (IsFull(parent))
            {
                SplitNode(parent);
            }
                
        }

        private void RemoveKeys(int index, Node node)
        {
            var newKeys = new List<int>();
            for(var i = 0; i < index; i++)
            {
                newKeys.Add(node.Keys[i]);
            }
            node.Keys = newKeys;
        }

        private void RemoveChildren(int index, Node node)
        {
            var newChildren = new List<Node>();
            for(var i = 0; i < index; i++)
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
            var leaf = FindLeafToAdd(key, Root);
            leaf.AddKeyToLeaf(key);
            if (IsFull(leaf))
                SplitNode(leaf);              
        }

        private bool IsFull(Node node)
        {
            return node.Keys.Count >= _treeOrder;
        }

        public Node FindLeafToAdd(int key, Node node)
        {
            if (node.IsLeaf)
            {
                return node;
            }

            for (var i = 0; i < node.Keys.Count; i++)
            {
                if (key <= node.Keys[i] && !node.IsLeaf || i == node.Children.Count)
                {
                    return FindLeafToAdd(key, node.Children[i]);
                }
            }
            return key > node.Keys[^1] ? FindLeafToAdd(key, node.Children[^1]) : node;
        }

        public Node FindNodeWithKeysToDelete(int key, Node node)
        {
            if (node.IsLeaf)
            {
                if (node.Keys.BinarySearch(key) == -1)
                    return null;
                return node;
            }
            for (var i = 0; i < node.Keys.Count; i++)
            {
                if (key < node.Keys[i] && !node.IsLeaf || i == node.Children.Count)
                {
                    return FindLeafToAdd(key, node.Children[i]);
                }
            }

            return FindNodeWithKeysToDelete(key, node.Children[^1]);
        }

        public void DeleteKey(int key)
        {
            var leaf = FindNodeWithKeysToDelete(key, Root);
            var startIndex = leaf.Keys.IndexOf(key);
            leaf.Keys.Remove(key);
            if (leaf.Keys.Count < _minNumberOfKey &&
                leaf.RightSibling != null &&
                leaf.RightSibling.Keys.Count > _minNumberOfKey)
            {
                var keyToMove = leaf.RightSibling.Keys[0];
                leaf.Keys.Add(leaf.RightSibling.Keys[0]);
                leaf.RightSibling.Keys.Remove(keyToMove);
                leaf.Parent.Keys.Remove(keyToMove);
                leaf.Parent.Keys.Add(leaf.RightSibling.Keys[0]);
            }
            else if (leaf.RightSibling == null && startIndex == 0)
            {
                leaf.Parent.Keys.Remove(key);
                leaf.Parent.Keys.Add(leaf.Keys[0]);
            }else if (leaf.Keys.Count == 0 && leaf.RightSibling != null && leaf.RightSibling.Keys.Count > 0)
            {
                leaf.LeftSibling.RightSibling = leaf.RightSibling;
                leaf.Parent.Children.Remove(leaf);
                var keyToMove = leaf.RightSibling.Keys[0];
                leaf = leaf.RightSibling;
                leaf.Parent.Keys.Remove(keyToMove);
                leaf.Parent.Parent.Keys[^1] = keyToMove;
            }
            else
            {

            }
        }

    }
}
