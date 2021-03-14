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
            if (leaf.KvpList.Contains(key))
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

            for (var i = 0; i < node.KvpList.Count; i++)
            {
                if (key <= node.KvpList[i] && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafToAdd(key, node.Children[i]);
            }
            return key > node.KvpList[^1] ? FindLeafToAdd(key, node.Children[^1]) : node;
        }

        public Node FindLeafWithKey(int key, Node node)
        {
            if (node.IsLeaf)
                return node.KvpList.BinarySearch(key) == -1 ? null : node;

            for (var i = 0; i < node.KvpList.Count; i++)
            {
                if (key < node.KvpList[i] && !node.IsLeaf || i == node.Children.Count)
                    return FindLeafWithKey(key, node.Children[i]);
            }

            return FindLeafWithKey(key, node.Children[^1]);
        }

        private void BalanceTree(Node node)
        {
            if (node == Root)
            {
                if (node.KvpList.Count == 0)
                    Root = node.Children[0];

                return;
            }

            var nextSibling = node.FindNextNode();
            var previousSibling = node.FindPreviousNode();

            if (node.Parent != null && node.Parent.KvpList.Count == node.Parent.Children.Count - 1 && node.KvpList.Count > 0)
            {
                if (node != Root)
                    BalanceTree(node.Parent);
                return;
            }

            switch (node.KvpList.Count < _minNumberOfKey)
            {
                case true when nextSibling != null && nextSibling.KvpList.Count > _minNumberOfKey:
                {
                    var keyToMove = nextSibling.KvpList[0];
                    var childToMove = nextSibling.Children[0];
                    node.Parent.KvpList.Add(nextSibling.KvpList[0]);
                    node.TryAddKeyToNode(node.Parent.KvpList[0]);
                    node.Parent.KvpList.Remove(node.Parent.KvpList[0]);
                    node.AddChildInCorrectOrder(childToMove);
                    childToMove.Parent = node;

                    nextSibling.KvpList.Remove(keyToMove);
                    nextSibling.Children.Remove(childToMove);
                    break;
                }
                case true when previousSibling != null && previousSibling.KvpList.Count > _minNumberOfKey:
                {
                    var keyToMove = previousSibling.KvpList[^1];
                    var childToMove = previousSibling.Children[^1];
                    node.Parent.TryAddKeyToNode(previousSibling.KvpList[^1]);
                    node.KvpList.Add(node.Parent.KvpList[^1]);
                    node.Parent.KvpList.Remove(node.Parent.KvpList[^1]);
                    node.AddChildInCorrectOrder(childToMove);
                    childToMove.Parent = node;

                    previousSibling.KvpList.Remove(keyToMove);
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
                    nodeToMerge.KvpList.AddRange(nodeToMergeNextSibling.KvpList);
                    nodeToMerge.Parent.Children.Remove(nodeToMergeNextSibling);

                    if (nodeToMerge.KvpList.Count < nodeToMerge.Children.Count - 1)
                    {
                        if (nodeToMerge.Parent.KvpList.Count > 0)
                        {
                            var parentKeyIndex = nodeToMerge.FindIndexOfNode();
                            var parentKey = nodeToMerge.Parent.KvpList[parentKeyIndex];
                            nodeToMerge.TryAddKeyToNode(parentKey);
                            nodeToMerge.Parent.KvpList.RemoveAt(parentKeyIndex);
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
                leaf.KvpList.Remove(key);
                return;
            }
            //check if leaf is edge left
            var edgeLeftCase = (leaf.Parent.Children[0] == leaf);

            var firstChildKey = leaf.Parent.Children[0].KvpList[0];
            leaf.KvpList.Remove(key);

            if (leaf.KvpList.Count >= _minNumberOfKey)
            {
                leaf.ReplaceValueInParent(key, leaf.KvpList[0]);
                return;
            }

            switch (leaf.KvpList.Count < _minNumberOfKey)
            {
                case true when leaf.NextLeaf != null && leaf.NextLeaf.KvpList.Count > _minNumberOfKey && leaf.NextLeaf.Parent == leaf.Parent:
                {
                    var keyToMove = leaf.NextLeaf.KvpList[0];
                    leaf.KvpList.Add(leaf.NextLeaf.KvpList[0]);
                    leaf.NextLeaf.KvpList.Remove(keyToMove);
                    leaf.ReplaceValueInParent(keyToMove, leaf.NextLeaf.KvpList[0]);
                    break;
                }
                case true when leaf.PreviousLeaf != null && leaf.PreviousLeaf.KvpList.Count > _minNumberOfKey && leaf.PreviousLeaf.Parent == leaf.Parent:
                {
                    var keyToMove = leaf.PreviousLeaf.KvpList[^1];
                    leaf.KvpList.Add(leaf.PreviousLeaf.KvpList[^1]);
                    leaf.PreviousLeaf.KvpList.Remove(keyToMove);
                    leaf.ReplaceValueInParent(key, keyToMove);
                    break;
                }
                default:
                {
                    var nodeToMerge = leaf;
                    
                    if (leaf.NextLeaf?.Parent != leaf.Parent) //if right sibling does not exist
                        nodeToMerge = leaf.PreviousLeaf;

                    nodeToMerge.Children.AddRange(nodeToMerge.NextLeaf.Children);
                    nodeToMerge.KvpList.AddRange(nodeToMerge.NextLeaf.KvpList);

                    //nodeToMerge.DeleteValueInParent(key);

                    if (nodeToMerge?.NextLeaf  != null && nodeToMerge?.NextLeaf?.KvpList.Count != 0 && edgeLeftCase)
                        nodeToMerge.Parent.KvpList.Remove(nodeToMerge.NextLeaf.KvpList[0]);

                    if (!edgeLeftCase)
                        nodeToMerge.Parent.KvpList.Remove(key);

                    var nodeToDelete = nodeToMerge.NextLeaf;
                    nodeToMerge.NextLeaf = nodeToMerge.NextLeaf.NextLeaf;

                    if (nodeToMerge.NextLeaf != null)
                        nodeToMerge.NextLeaf.PreviousLeaf = nodeToMerge;

                    nodeToMerge.Parent.Children.Remove(nodeToDelete);

                    nodeToMerge.ReplaceValueInParent(firstChildKey, nodeToMerge.Parent.Children[0].KvpList[0]);
                    if (nodeToMerge != Root)
                        BalanceTree(nodeToMerge.Parent);
                    break;
                }
            }
        }
    }
}
