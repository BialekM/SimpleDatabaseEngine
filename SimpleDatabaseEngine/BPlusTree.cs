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
            Root.TryAddKeyToNode(key);
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
            //if node is node rewrite the median key to parent
            //otherwise skip it
            var keysForBigger = node.IsLeaf
                ? node.Keys.GetRange(median, node.Keys.Count - median)
            : node.Keys.GetRange(median + 1, node.Keys.Count - median - 1);
            
            parent.TryAddKeyToNode(newParentKey);
            //Remove Keys from left node
            RemoveKeys(median, node);
            
            //Create list for BiggerLeaf + Remove children from left (old node)
            if (node.Children.Count > 0)
            {
                listOfChildrenForBigger = node.Children.GetRange(median + 1, node.Children.Count - median -1);
                //Remove Keys from left node
                RemoveChildren(median + 1, node);
            }

            //Create Child for bigger Node and set Parent to children
            var biggerNode = parent.CreateChild(listOfChildrenForBigger, keysForBigger, parent, node.IsLeaf);

            //Set right parent for node
            if (node.IsLeaf)
            {
                node.NextLeaf = biggerNode;
                node.NextLeaf.PreviousLeaf = node;
            }

            //special case when parent is just created new root
            if (parent == Root && isNewRoot) 
                parent.Children.Add(node);

            //Add Bigger Child
            parent.AddChildInCorrectOrder(biggerNode);

            //Parent have children - can't be node
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

        public bool TryAddKeyToTree(int key)
        {
            var leaf = FindLeafToAdd(key, Root);
            if (leaf.Keys.Contains(key))
                return false;

            leaf.TryAddKeyToNode(key);
            if (IsFull(leaf))
                SplitNode(leaf);
            return true;
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
                    return FindNodeWithKeysToDelete(key, node.Children[i]);
                }
            }

            return FindNodeWithKeysToDelete(key, node.Children[^1]);
        }

        public void DeleteKey(int key)
        {
            var leaf = FindNodeWithKeysToDelete(key, Root);
            leaf.Keys.Remove(key);
            if (leaf.Keys.Count >= _minNumberOfKey)
            {
                leaf.ReplaceValueInParent(key, leaf.Keys[0]);
                return;
            }
            BalanceNode(leaf);
        }

        public void BalanceNode(Node node)
        {
            if (node.Keys.Count < _minNumberOfKey &&
                node.NextLeaf != null &&
                node.NextLeaf.Keys.Count > _minNumberOfKey
                && node.NextLeaf.Parent == node.Parent)
            {
                var keyToMove = node.NextLeaf.Keys[0];
                node.Keys.Add(node.NextLeaf.Keys[0]);
                node.NextLeaf.Keys.Remove(keyToMove);
                node.ReplaceValueInParent(keyToMove, node.NextLeaf.Keys[0]);
            }
            else if (node.Keys.Count < _minNumberOfKey &&
                     node.PreviousLeaf != null &&
                     node.PreviousLeaf.Keys.Count > _minNumberOfKey &&
                     node.PreviousLeaf.Parent == node.Parent)
            {
                var keyToMove = node.PreviousLeaf.Keys[^1];
                node.Keys.Add(node.PreviousLeaf.Keys[^1]);
                node.PreviousLeaf.Keys.Remove(keyToMove);
            }
            else
            {
                var nodeToMerge = node;
                if (node.NextLeaf?.Parent != node.Parent) //if right sibling does not exist
                {
                    nodeToMerge = node.PreviousLeaf;
                }

                for (var i = 0; i < nodeToMerge.Parent.Keys.Count; i++)
                {
                    if ((nodeToMerge.Keys.Count == 0 || nodeToMerge.Parent.Keys[i] > nodeToMerge.Keys[^1]) &&
                        nodeToMerge.Parent.Keys[i] <= nodeToMerge.PreviousLeaf.Keys[0])
                    {
                        nodeToMerge.Parent.Keys.RemoveAt(i);
                        break;
                    }
                }

                nodeToMerge.Children.AddRange(nodeToMerge.NextLeaf.Children);
                nodeToMerge.Keys.AddRange(nodeToMerge.NextLeaf.Keys);
                var nodeToDelete = nodeToMerge.NextLeaf;
                nodeToMerge.NextLeaf = nodeToMerge.NextLeaf.NextLeaf;

                if (nodeToMerge.NextLeaf != null)
                    nodeToMerge.NextLeaf.PreviousLeaf = nodeToMerge;

                nodeToMerge.Parent.Children.Remove(nodeToDelete);
                if(nodeToMerge != Root)
                    BalanceNode(nodeToMerge.Parent);
            }
        }
    }
}
