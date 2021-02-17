using System.Collections.Generic;
using NUnit.Framework;
using SimpleDatabaseEngine;

namespace SimpleDatabaseEngineTests
{
    class NodeTests
    {

        [Test]
        public void AddKeysToNodeExpectCorrectOrder()
        {
            var node = new Node();
            node.TryAddKeyToNode(5);
            node.TryAddKeyToNode(3);
            node.TryAddKeyToNode(10);
            node.TryAddKeyToNode(11);
            node.TryAddKeyToNode(2);
            node.TryAddKeyToNode(21);
            Assert.AreEqual(2, node.Keys[0]);
            Assert.AreEqual(3, node.Keys[1]);
            Assert.AreEqual(5, node.Keys[2]);
            Assert.AreEqual(10, node.Keys[3]);
            Assert.AreEqual(11, node.Keys[4]);
            Assert.AreEqual(21, node.Keys[5]);
        }

        [Test]
        public void TryAddDuplicateKeyToNode()
        {
            var tree = new BPlusTree(3, 5);
            Assert.AreEqual(false,tree.TryAddKeyToTree(5));
        }

        [Test]
        public void AppendChild()
        {
            var node = new Node { Keys = new List<int>(6) };
            var children = new List<Node> { new Node { Keys = new List<int> { 7 } } };
            var keys = new List<int> { 8 };
            var parent = new Node { Keys = new List<int> { 5 } };
            var newNode = node.AppendChild(children, keys, parent, true);

            Assert.AreEqual(true, newNode.Children[0].IsLeaf);
            Assert.AreEqual(8, newNode.Keys[0]);
            Assert.AreEqual(7, newNode.Children[0].Keys[0]);
            Assert.AreEqual(5, newNode.Parent.Keys[0]);
            Assert.AreEqual(newNode, newNode.Children[0].Parent);
        }

        [Test]
        public void AppendChildrenInCorrectOrder()
        {
            var parent = new Node { Keys = new List<int> { 5 } };
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 11 } });
            parent.TryAddKeyToNode(10);
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 16 } });
            parent.TryAddKeyToNode(15);
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 4 } });
            parent.TryAddKeyToNode(25);
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 26 } });
            parent.TryAddKeyToNode(35);
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 17 } });
            parent.AddChildInCorrectOrder(new Node { Keys = new List<int> { 36 } });

            Assert.AreEqual(4,parent.Children[0].Keys[0]);
            Assert.AreEqual(11, parent.Children[1].Keys[0]);
            Assert.AreEqual(16, parent.Children[2].Keys[0]);
            Assert.AreEqual(17, parent.Children[3].Keys[0]);
            Assert.AreEqual(26, parent.Children[4].Keys[0]);
            Assert.AreEqual(36, parent.Children[5].Keys[0]);
        }

        [Test]
        public void ReplaceValueInParent()
        {
            var node = new Node {Parent = new Node {Keys = new List<int> {10}}};
            node.ReplaceValueInParent(10,11);
            Assert.AreEqual(11,node.Parent.Keys[0]);
        }
    }
}
