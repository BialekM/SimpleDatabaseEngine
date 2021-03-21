using System;
using System.Linq;

namespace SimpleDatabaseEngine
{
    public class BPlusTree
    {
        public Node Root;

        private readonly int _minNumberOfKey;
        private int _median;

        public BPlusTree(int key, string value)
        {
            const int treeOrder = 3;
            _median = (int)Math.Ceiling((float)treeOrder / 2) - 1;
            _minNumberOfKey = _median;
            Root = new Node();
            Root.KeyValueDictionary.Add(key, value);
        }

        public bool TryAppendElementToTree(int key, string value)
        {
            var leaf = FindLeafToAdd(key, Root);
            if (leaf.KeyValueDictionary.ContainsKey(key))
                return false;

            leaf.KeyValueDictionary.Add(key, value);
            if (leaf.IsFull())
                leaf.SplitNode(ref Root, ref _median);
            return true;
        }

        public Node FindLeafToAdd(int key, Node node)
        {
            if (node.IsLeaf)
                return node;

            for (var i = 0; i < node.KeyValueDictionary.Count; i++)
            {               
                if (key <= node.KeyValueDictionary.ElementAt(i).Key && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafToAdd(key, node.Children[i]);
            }
            return key > node.KeyValueDictionary.Last().Key ? FindLeafToAdd(key, node.Children[^1]) : node;
        }

        public Node FindLeafWithKey(int key, Node node)
        {
            if (node.IsLeaf)
                return node.KeyValueDictionary.ContainsKey(key) ? node : null;

            for (var i = 0; i < node.KeyValueDictionary.Count; i++)
            {
                if (key < node.KeyValueDictionary.ElementAt(i).Key && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafWithKey(key, node.Children[i]);
            }

            return FindLeafWithKey(key, node.Children[^1]);
        }

        private void BalanceTree(Node node)
        {
            if (node == Root)
            {
                if (node.KeyValueDictionary.Count == 0)
                    Root = node.Children[0];

                return;
            }

            var nextSibling = node.FindNextNode();
            var previousSibling = node.FindPreviousNode();

            if (node.Parent != null && node.Parent.KeyValueDictionary.Count == node.Parent.Children.Count - 1 && node.KeyValueDictionary.Count > 0)
            {
                if (node != Root)
                    BalanceTree(node.Parent);
                return;
            }

            switch (node.KeyValueDictionary.Count < _minNumberOfKey)
            {
                case true when nextSibling != null && nextSibling.KeyValueDictionary.Count > _minNumberOfKey:
                    {
                        var keyToMove = nextSibling.KeyValueDictionary.ElementAt(0).Key;
                        var childToMove = nextSibling.Children[0];
                        node.Parent.KeyValueDictionary.Add(nextSibling.KeyValueDictionary.ElementAt(0).Key, null);
                        node.KeyValueDictionary.Add(node.Parent.KeyValueDictionary.ElementAt(0).Key, null);
                        node.Parent.KeyValueDictionary.Remove(node.Parent.KeyValueDictionary.ElementAt(0).Key);
                        node.AddChildInCorrectOrder(childToMove);
                        childToMove.Parent = node;

                        nextSibling.KeyValueDictionary.Remove(keyToMove);
                        nextSibling.Children.Remove(childToMove);
                        break;
                    }
                case true when previousSibling != null && previousSibling.KeyValueDictionary.Count > _minNumberOfKey:
                    {
                        var keyToMove = previousSibling.KeyValueDictionary.Last().Key;
                        var childToMove = previousSibling.Children[^1];
                        node.Parent.KeyValueDictionary.Add(previousSibling.KeyValueDictionary.Last().Key, null);
                        node.KeyValueDictionary.Add(node.Parent.KeyValueDictionary.Last().Key, node.Parent.KeyValueDictionary.Last().Value);
                        node.Parent.KeyValueDictionary.Remove(node.Parent.KeyValueDictionary.Last().Key);
                        node.AddChildInCorrectOrder(childToMove);
                        childToMove.Parent = node;

                        previousSibling.KeyValueDictionary.Remove(keyToMove);
                        previousSibling.Children.Remove(childToMove);
                        break;
                    }
                default:
                    {
                        var nodeToMerge = node;
                        if (nextSibling?.Parent != node.Parent) //if right sibling does not exist
                            nodeToMerge = previousSibling;

                        var nodeToMergeNextSibling = nodeToMerge.FindNextNode();

                        nodeToMerge.Children.AddRange(nodeToMergeNextSibling.Children);
                        foreach(var kvp in nodeToMergeNextSibling.KeyValueDictionary)
                            nodeToMerge.KeyValueDictionary.Add(kvp.Key, kvp.Value);
                        //nodeToMerge.KvpList.AddRange(nodeToMergeNextSibling.KvpList);
                        nodeToMerge.Parent.Children.Remove(nodeToMergeNextSibling);

                        if (nodeToMerge.KeyValueDictionary.Count < nodeToMerge.Children.Count - 1)
                        {
                            if (nodeToMerge.Parent.KeyValueDictionary.Count > 0)
                            {
                                var parentKeyIndex = nodeToMerge.FindIndexOfNode();
                                var parentKey = nodeToMerge.Parent.KeyValueDictionary.ElementAt(parentKeyIndex).Key;
                                nodeToMerge.KeyValueDictionary.Add(parentKey, null);                                
                                nodeToMerge.Parent.KeyValueDictionary.Remove(nodeToMerge.Parent.KeyValueDictionary.ElementAt(parentKeyIndex).Key);
                            }

                            nodeToMerge.Parent.Children.Remove(nodeToMergeNextSibling);
                        }

                        foreach (var child in nodeToMerge.Children)
                            child.Parent = nodeToMerge;

                        if (nodeToMerge != Root)
                            BalanceTree(nodeToMerge.Parent);
                        break;
                    }
            }
        }

        public void Delete(int key)
        {
            var leaf = FindLeafWithKey(key, Root);
            if (leaf == Root)
            {
                leaf.KeyValueDictionary.Remove(key);
                return;
            }
            //check if leaf is edge left
            var edgeLeftCase = (leaf.Parent.Children[0] == leaf);

            var firstChildKey = leaf.Parent.Children[0].KeyValueDictionary.ElementAt(0).Key;
            leaf.KeyValueDictionary.Remove(key);

            if (leaf.KeyValueDictionary.Count >= _minNumberOfKey)
            {
                leaf.ReplaceValueInParent(key, leaf.KeyValueDictionary.ElementAt(0).Key);
                return;
            }

            switch (leaf.KeyValueDictionary.Count < _minNumberOfKey)
            {
                case true when leaf.NextLeaf != null && leaf.NextLeaf.KeyValueDictionary.Count > _minNumberOfKey && leaf.NextLeaf.Parent == leaf.Parent:
                    {
                        var keyToMove = leaf.NextLeaf.KeyValueDictionary.ElementAt(0).Key;
                        leaf.KeyValueDictionary.Add(leaf.NextLeaf.KeyValueDictionary.ElementAt(0).Key, leaf.NextLeaf.KeyValueDictionary.ElementAt(0).Value);
                        leaf.NextLeaf.KeyValueDictionary.Remove(keyToMove);
                        leaf.ReplaceValueInParent(keyToMove, leaf.NextLeaf.KeyValueDictionary.ElementAt(0).Key);
                        break;
                    }
                case true when leaf.PreviousLeaf != null && leaf.PreviousLeaf.KeyValueDictionary.Count > _minNumberOfKey && leaf.PreviousLeaf.Parent == leaf.Parent:
                    {
                        var keyToMove = leaf.PreviousLeaf.KeyValueDictionary.Last().Key;
                        leaf.KeyValueDictionary.Add(leaf.PreviousLeaf.KeyValueDictionary.Last().Key, leaf.PreviousLeaf.KeyValueDictionary.Last().Value);
                        leaf.PreviousLeaf.KeyValueDictionary.Remove(keyToMove);
                        leaf.ReplaceValueInParent(key, keyToMove);
                        break;
                    }
                default:
                    {
                        var nodeToMerge = leaf;

                        if (leaf.NextLeaf?.Parent != leaf.Parent) //if right sibling does not exist
                            nodeToMerge = leaf.PreviousLeaf;

                        nodeToMerge.Children.AddRange(nodeToMerge.NextLeaf.Children);
                        foreach(var kvp in nodeToMerge.NextLeaf.KeyValueDictionary)                        
                            nodeToMerge.KeyValueDictionary.Add(kvp.Key, kvp.Value);
                        

                        //nodeToMerge.DeleteValueInParent(key);

                        if (nodeToMerge?.NextLeaf != null && nodeToMerge?.NextLeaf?.KeyValueDictionary.Count != 0 && edgeLeftCase)
                            nodeToMerge.Parent.KeyValueDictionary.Remove(nodeToMerge.NextLeaf.KeyValueDictionary.ElementAt(0).Key);

                        if (!edgeLeftCase)
                            nodeToMerge.Parent.KeyValueDictionary.Remove(key);

                        var nodeToDelete = nodeToMerge.NextLeaf;
                        nodeToMerge.NextLeaf = nodeToMerge.NextLeaf.NextLeaf;

                        if (nodeToMerge.NextLeaf != null)
                            nodeToMerge.NextLeaf.PreviousLeaf = nodeToMerge;

                        nodeToMerge.Parent.Children.Remove(nodeToDelete);

                        nodeToMerge.ReplaceValueInParent(firstChildKey, nodeToMerge.Parent.Children[0].KeyValueDictionary.ElementAt(0).Key);
                        if (nodeToMerge != Root)
                            BalanceTree(nodeToMerge.Parent);
                        break;
                    }
            }
        }

        public bool TryReplaceValue(int key, string newValue)
        {
            var node = FindLeafWithKey(key, Root);
            if (node != null)
            {
                node.KeyValueDictionary[key] = newValue;
                return true;
            }
            else
                return false;

        }
    }
}
