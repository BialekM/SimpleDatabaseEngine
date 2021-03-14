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
            Assert.AreEqual(2, node.KvpList[0]);
            Assert.AreEqual(3, node.KvpList[1]);
            Assert.AreEqual(5, node.KvpList[2]);
            Assert.AreEqual(10, node.KvpList[3]);
            Assert.AreEqual(11, node.KvpList[4]);
            Assert.AreEqual(21, node.KvpList[5]);
        }

        [Test]
        public void TryAddDuplicateKeyToNode()
        {
            var tree = new BPlusTree( 5);
            Assert.AreEqual(false,tree.TryAddKeyToTree(5));
        }

        [Test]
        public void AppendChild()
        {
            var node = new Node { KvpList = new List<int>(6) };
            var children = new List<Node> { new Node { KvpList = new List<int> { 7 } } };
            var keys = new List<int> { 8 };
            var parent = new Node { KvpList = new List<int> { 5 } };
            node = new NodeBuilder().SetChildren(children).SetKeys(keys).SetParent(parent).SetIsLeaf(true).GetNode();
            node.SetParentForChildren();

            Assert.AreEqual(true, node.Children[0].IsLeaf);
            Assert.AreEqual(8, node.KvpList[0]);
            Assert.AreEqual(7, node.Children[0].KvpList[0]);
            Assert.AreEqual(5, node.Parent.KvpList[0]);
            Assert.AreEqual(node, node.Children[0].Parent);
        }

        [Test]
        public void AppendChildrenInCorrectOrder()
        {
            var parent = new Node { KvpList = new List<int> { 5 } };
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 11 } });
            parent.TryAddKeyToNode(10);
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 16 } });
            parent.TryAddKeyToNode(15);
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 4 } });
            parent.TryAddKeyToNode(25);
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 26 } });
            parent.TryAddKeyToNode(35);
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 17 } });
            parent.AddChildInCorrectOrder(new Node { KvpList = new List<int> { 36 } });

            Assert.AreEqual(4,parent.Children[0].KvpList[0]);
            Assert.AreEqual(11, parent.Children[1].KvpList[0]);
            Assert.AreEqual(16, parent.Children[2].KvpList[0]);
            Assert.AreEqual(17, parent.Children[3].KvpList[0]);
            Assert.AreEqual(26, parent.Children[4].KvpList[0]);
            Assert.AreEqual(36, parent.Children[5].KvpList[0]);
        }

        [Test]
        public void ReplaceValueInParent()
        {
            var node = new Node {Parent = new Node {KvpList = new List<int> {10}}};
            node.ReplaceValueInParent(10,11);
            Assert.AreEqual(11,node.Parent.KvpList[0]);
        }
    }
}
