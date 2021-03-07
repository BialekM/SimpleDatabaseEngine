using System;

namespace SimpleDatabaseEngine
{
    public class BPlusTree
    {
        public Node Root;

        private readonly int _minNumberOfKey;
        private int _median;

        public BPlusTree(int key)
        {
            const int treeOrder = 3;
            _median = (int)Math.Ceiling((float)treeOrder / 2) - 1;
            _minNumberOfKey = _median;
            Root = new Node();
            Root.TryAddKeyToNode(key);
        }

        public bool TryAddKeyToTree(int key)
        {
            var leaf = FindLeafToAdd(key, Root);
            if (leaf.Keys.Contains(key))
                return false;

            leaf.TryAddKeyToNode(key);
            if (leaf.IsFull())
                leaf.SplitNode(ref Root, ref _median);
            return true;
        }

        public Node FindLeafToAdd(int key, Node node)
        {
            if (node.IsLeaf)
                return node;

            for (var i = 0; i < node.Keys.Count; i++)
            {
                if (key <= node.Keys[i] && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafToAdd(key, node.Children[i]);
            }
            return key > node.Keys[^1] ? FindLeafToAdd(key, node.Children[^1]) : node;
        }

        public Node FindLeafWithKey(int key, Node node)
        {
            if (node.IsLeaf)
                return node.Keys.BinarySearch(key) == -1 ? null : node;

            for (var i = 0; i < node.Keys.Count; i++)
            {
                if (key < node.Keys[i] && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafWithKey(key, node.Children[i]);
            }

            return FindLeafWithKey(key, node.Children[^1]);
        }

        private void BalanceTree(Node node)
        {
            if (node == Root)
            {
                if (node.Keys.Count == 0)
                    Root = node.Children[0];

                return;
            }

            var nextSibling = node.FindNextNode();
            var previousSibling = node.FindPreviousNode();

            if (node.Parent != null && node.Parent.Keys.Count == node.Parent.Children.Count - 1 && node.Keys.Count > 0)
            {
                if (node != Root)
                    BalanceTree(node.Parent);
                return;
            }

            switch (node.Keys.Count < _minNumberOfKey)
            {
                case true when nextSibling != null && nextSibling.Keys.Count > _minNumberOfKey:
                {
                    var keyToMove = nextSibling.Keys[0];
                    var childToMove = nextSibling.Children[0];
                    node.Parent.Keys.Add(nextSibling.Keys[0]);
                    node.TryAddKeyToNode(node.Parent.Keys[0]);
                    node.Parent.Keys.Remove(node.Parent.Keys[0]);
                    node.AddChildInCorrectOrder(childToMove);
                    childToMove.Parent = node;

                    nextSibling.Keys.Remove(keyToMove);
                    nextSibling.Children.Remove(childToMove);
                    break;
                }
                case true when previousSibling != null && previousSibling.Keys.Count > _minNumberOfKey:
                {
                    var keyToMove = previousSibling.Keys[^1];
                    var childToMove = previousSibling.Children[^1];
                    node.Parent.TryAddKeyToNode(previousSibling.Keys[^1]);
                    node.Keys.Add(node.Parent.Keys[^1]);
                    node.Parent.Keys.Remove(node.Parent.Keys[^1]);
                    node.AddChildInCorrectOrder(childToMove);
                    childToMove.Parent = node;

                    previousSibling.Keys.Remove(keyToMove);
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
                    nodeToMerge.Keys.AddRange(nodeToMergeNextSibling.Keys);
                    nodeToMerge.Parent.Children.Remove(nodeToMergeNextSibling);

                    if (nodeToMerge.Keys.Count < nodeToMerge.Children.Count - 1)
                    {
                        if (nodeToMerge.Parent.Keys.Count > 0)
                        {
                            var parentKeyIndex = nodeToMerge.FindIndexOfNode();
                            var parentKey = nodeToMerge.Parent.Keys[parentKeyIndex];
                            nodeToMerge.TryAddKeyToNode(parentKey);
                            nodeToMerge.Parent.Keys.RemoveAt(parentKeyIndex);
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

        public void DeleteKey(int key)
        {
            var leaf = FindLeafWithKey(key, Root);
            if (leaf == Root)
            {
                leaf.Keys.Remove(key);
                return;
            }
            //check if leaf is edge left
            var edgeLeftCase = (leaf.Parent.Children[0] == leaf);

            var firstChildKey = leaf.Parent.Children[0].Keys[0];
            leaf.Keys.Remove(key);

            if (leaf.Keys.Count >= _minNumberOfKey)
            {
                leaf.ReplaceValueInParent(key, leaf.Keys[0]);
                return;
            }

            switch (leaf.Keys.Count < _minNumberOfKey)
            {
                case true when leaf.NextLeaf != null && leaf.NextLeaf.Keys.Count > _minNumberOfKey && leaf.NextLeaf.Parent == leaf.Parent:
                {
                    var keyToMove = leaf.NextLeaf.Keys[0];
                    leaf.Keys.Add(leaf.NextLeaf.Keys[0]);
                    leaf.NextLeaf.Keys.Remove(keyToMove);
                    leaf.ReplaceValueInParent(keyToMove, leaf.NextLeaf.Keys[0]);
                    break;
                }
                case true when leaf.PreviousLeaf != null && leaf.PreviousLeaf.Keys.Count > _minNumberOfKey && leaf.PreviousLeaf.Parent == leaf.Parent:
                {
                    var keyToMove = leaf.PreviousLeaf.Keys[^1];
                    leaf.Keys.Add(leaf.PreviousLeaf.Keys[^1]);
                    leaf.PreviousLeaf.Keys.Remove(keyToMove);
                    leaf.ReplaceValueInParent(key, keyToMove);
                    break;
                }
                default:
                {
                    var nodeToMerge = leaf;
                    
                    if (leaf.NextLeaf?.Parent != leaf.Parent) //if right sibling does not exist
                        nodeToMerge = leaf.PreviousLeaf;

                    nodeToMerge.Children.AddRange(nodeToMerge.NextLeaf.Children);
                    nodeToMerge.Keys.AddRange(nodeToMerge.NextLeaf.Keys);

                    //nodeToMerge.DeleteValueInParent(key);

                    if (nodeToMerge?.NextLeaf  != null && nodeToMerge?.NextLeaf?.Keys.Count != 0 && edgeLeftCase)
                        nodeToMerge.Parent.Keys.Remove(nodeToMerge.NextLeaf.Keys[0]);

                    if (!edgeLeftCase)
                        nodeToMerge.Parent.Keys.Remove(key);

                    var nodeToDelete = nodeToMerge.NextLeaf;
                    nodeToMerge.NextLeaf = nodeToMerge.NextLeaf.NextLeaf;

                    if (nodeToMerge.NextLeaf != null)
                        nodeToMerge.NextLeaf.PreviousLeaf = nodeToMerge;

                    nodeToMerge.Parent.Children.Remove(nodeToDelete);

                    nodeToMerge.ReplaceValueInParent(firstChildKey, nodeToMerge.Parent.Children[0].Keys[0]);
                    if (nodeToMerge != Root)
                        BalanceTree(nodeToMerge.Parent);
                    break;
                }
            }
        }
    }
}
