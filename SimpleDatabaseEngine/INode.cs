using System.Collections.Generic;

namespace SimpleDatabaseEngine
{
    public interface INode
    {
        List<int> Keys { get; set; }
        Node Parent { get; set; }
        bool IsLeaf { get; set; }
        List<Node> Children { get; set; }

        bool TryAddKeyToNode(int key);
        void AddChildInCorrectOrder(Node child);
        void ReplaceValueInParent(int key, int newKey);
        void DeleteValueInParent(int key);
        bool IsFull();
        void RemoveKeysFromIndex(int index);
        void RemoveChildrenFromIndex(int index);
        void SetParentForChildren();
        Node FindNextNode();
        int FindIndexOfNode();
        Node FindPreviousNode();
        void SplitNode(ref Node root, ref int median);
    }
}
