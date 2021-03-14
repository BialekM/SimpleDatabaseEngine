﻿using System.Collections.Generic;
using System.Linq;

namespace SimpleDatabaseEngine
{
    public class Node
    {
        private const int TreeOrder = 3;
        public SortedList<int, string> KvpList = new SortedList<int, string>();
        public Node Parent { get; set; }
        public bool IsLeaf { get; set; } = true;
        public List<Node> Children { get; set; } = new List<Node>();
        public Node NextLeaf { get; set; }
        public Node PreviousLeaf { get; set; }

        //Add key in correct order + null check
        public bool TryAddKeyToNode(int key)
        {
            if (KvpList.Keys.Contains(key))
                return false;
            if (KvpList.Any() && key < KvpList.Keys[0])
            {
                KvpList.Keys.Insert(0, key);
                return true;
            }
            for (var i = 0; i < KvpList.Count - 1; ++i)
            {
                if (key > KvpList.Keys[i] && key < KvpList.Keys[i + 1])
                {
                    KvpList.Keys.Insert(i + 1, key);
                    return true;
                }
            }
            KvpList.Keys.Add(key);
            return true;
        }

        public void AddChildInCorrectOrder(Node child)
        {
            if (KvpList.Any() && child.KvpList.Keys[0] < KvpList.Keys[0])
            {
                Children.Insert(0, child);
                return;
            }

            for (var i = 0; i < KvpList.Count - 1; ++i)
            {
                if (child.KvpList.Keys[0] >= KvpList.Keys[i] && child.KvpList.Keys[^1] < KvpList.Keys[i + 1])
                {
                    Children.Insert(i + 1, child);
                    return;
                }
            }
            Children.Add(child);
        }

        public void ReplaceValueInParent(int key, int newKey)
        {
            if (Parent != null && Parent.KvpList.Keys.Contains(key))
            {
                Parent.KvpList.Keys[Parent.KvpList.Keys.IndexOf(key)] = newKey;
                return;
            }

            Parent?.ReplaceValueInParent(key, newKey);
        }

        public void DeleteValueInParent(int key)
        {
            if (Parent != null && Parent.KvpList.Keys.Contains(key))
            {
                Parent.KvpList.Remove(key);
                return;
            }

            Parent?.DeleteValueInParent(key);
        }

        public bool IsFull()
        {
            return KvpList.Count >= TreeOrder;
        }

        public void RemoveKeysFromIndex(int index)
        {
            for(int i = index; i < KvpList.Count - index; i++)
                KvpList.RemoveAt(i);
        }

        public void RemoveChildrenFromIndex(int index)
        {
            Children.RemoveRange(index, Children.Count - index);
        }

        public void SetParentForChildren()
        {
            foreach (var child in this.Children)
                child.Parent = this;
        }

        public Node FindNextNode()
        {
            var indexOfNode = Parent.Children.IndexOf(this);
            return Parent.Children.Count > indexOfNode + 1 ? Parent.Children[indexOfNode + 1] : null;
        }

        public int FindIndexOfNode()
        {
            return Parent?.Children.IndexOf(this) ?? -1;
        }

        public Node FindPreviousNode()
        {
            var indexOfNode = Parent.Children.IndexOf(this);
            return indexOfNode > 0 ? Parent.Children[indexOfNode - 1] : null;
        }

        public void SplitNode(ref Node root, ref int median)
        {
            var listOfChildrenForBigger = new List<Node>();
            var isNewRoot = false;
            
            //left child is old node
            var newParentKey = KvpList.Keys[median];

            //if node is node rewrite the median key to parent
            //otherwise skip it
            var keysForBigger = IsLeaf
                ? GetRangeForSortedList(median, KvpList.Keys.Count - median, KvpList)
                : GetRangeForSortedList(median + 1, KvpList.Count - median - 1, KvpList);

            Node parent;

            //root case
            if (Parent == null)
            {
                parent = new Node();
                root = parent;
                isNewRoot = true;
                Parent = parent;
            }
            else
                parent = Parent;



            //Add key from median to parent
            parent.TryAddKeyToNode(newParentKey);
            
            //Remove Keys from left node
            RemoveKeysFromIndex(median);

            
            if (Children.Count > 0)
            {
                //Create list of keys for BiggerLeaf
                listOfChildrenForBigger = Children.GetRange(median + 1, Children.Count - median - 1);
                //Remove Children from left node
                RemoveChildrenFromIndex(median + 1);
            }

            //create biggerNode
            var biggerNode = new NodeBuilder()
                .SetChildren(listOfChildrenForBigger)
                .SetIsLeaf(IsLeaf)
                .SetKeys(keysForBigger)
                .SetParent(parent)
                .GetNode();
            biggerNode.SetParentForChildren();

            //Set previous and next leaf
            if (IsLeaf)
            {
                var nextLeaf = NextLeaf;
                NextLeaf = biggerNode;
                if (nextLeaf != null) nextLeaf.PreviousLeaf = biggerNode;

                biggerNode.PreviousLeaf = this;
                biggerNode.NextLeaf = nextLeaf;
            }

            //Special case when parent is just created new root
            if (parent == root && isNewRoot)
                parent.Children.Add(this);

            //Add Bigger Child
            parent.AddChildInCorrectOrder(biggerNode);

            //Parent have children - can't be node
            parent.IsLeaf = false;

            //Check if parent should be split again
            if (parent.IsFull())
                parent.SplitNode(ref root, ref median);

        }

        private SortedList<int,string> GetRangeForSortedList(int from,int count, SortedList<int, string> sortedList)
        {
            SortedList<int, string> newList = new SortedList<int, string>();
            for(int i = 0; i < count; i++)            
                newList.Add(sortedList.Keys[i + from], sortedList.Values[i + from]);
            return newList;
        }
    }
}
