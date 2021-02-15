﻿using System.Collections.Generic;
using System.Linq;

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

        public Node CreateChild(List<Node> children, List<int> keys, Node parent, bool isLeaf)
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
        public void AddKeyToLeaf(int key  )
        {
            if (Keys.Count > 0 && key < Keys[0])
            {
                Keys.Insert(0, key);
                return;
            }
            for (var i = 0; i < Keys.Count -1; ++i)
            {
                if( key> Keys[i] && key < Keys[i + 1])
                {
                    Keys.Insert(i + 1, key);
                    return;
                }
            }
            Keys.Add(key);
        }

        public void AddChildInCorrectOrder(Node child)
        {
            if (Keys.Count > 0 && child.Keys[0] < Keys[0])
            {
                Children.Insert(0,child);
                return;
            }
            for (var i = 0; i < Keys.Count - 1; ++i)
            {
                if (child.Keys[0] >= Keys[i] && child.Keys[^1] < Keys[i + 1])
                {

                    //Children[i].NextLeaf = child;
                    //child.PreviousLeaf = Children[i];
                    //child.NextLeaf = Children[i + 1];
                    //if (Children[i] != null)
                    //{
                        child.NextLeaf = Children[i + 1];
                        child.PreviousLeaf = child.NextLeaf.PreviousLeaf;
                        child.PreviousLeaf.NextLeaf = child;
                        child.NextLeaf.PreviousLeaf = child;
                    //}
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
