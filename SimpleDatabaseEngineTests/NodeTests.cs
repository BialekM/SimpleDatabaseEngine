using System.Collections.Generic;
using System.Linq;
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
            node.KeyValueDictionary.Add(5, null);
            node.KeyValueDictionary.Add(3, null);
            node.KeyValueDictionary.Add(10, null);
            node.KeyValueDictionary.Add(11, null);
            node.KeyValueDictionary.Add(2, null);
            node.KeyValueDictionary.Add(21, null);
            Assert.AreEqual(2, node.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(3, node.KeyValueDictionary.ElementAt(1).Key);
            Assert.AreEqual(5, node.KeyValueDictionary.ElementAt(2).Key);
            Assert.AreEqual(10, node.KeyValueDictionary.ElementAt(3).Key);
            Assert.AreEqual(11, node.KeyValueDictionary.ElementAt(4).Key);
            Assert.AreEqual(21, node.KeyValueDictionary.ElementAt(5).Key);
        }

        [Test]
        public void TryAddDuplicateKeyToNode()
        {
            var tree = new BPlusTree(5);
            Assert.AreEqual(false, tree.TryAddKeyToTree(5));
        }

        [Test]
        public void AppendChild()
        {
            var node = new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 6, null } } };
            var children = new List<Node> { new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 7, null } } } };
            var keys = new SortedDictionary<int, string> { { 8, null } };
            var parent = new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 5, null } } };
            node = new NodeBuilder().SetChildren(children).SetKeys(keys).SetParent(parent).SetIsLeaf(true).GetNode();
            node.SetParentForChildren();

            Assert.AreEqual(true, node.Children[0].IsLeaf);
            Assert.AreEqual(8, node.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(7, node.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(5, node.Parent.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(node, node.Children[0].Parent);
        }

        [Test]
        public void AppendChildrenInCorrectOrder()
        {
            var parent = new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 5, null } } };
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 11, null } } });
            parent.KeyValueDictionary.Add(10, null);
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 16, null } } });
            parent.KeyValueDictionary.Add(15, null);
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 4, null } } });
            parent.KeyValueDictionary.Add(25, null);
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 26, null } } });
            parent.KeyValueDictionary.Add(35, null);
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 17, null } } });
            parent.AddChildInCorrectOrder(new Node { KeyValueDictionary = new SortedDictionary<int, string> { { 36, null } } });

            Assert.AreEqual(4, parent.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(11, parent.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(16, parent.Children[2].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(17, parent.Children[3].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(26, parent.Children[4].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(36, parent.Children[5].KeyValueDictionary.ElementAt(0).Key);
        }

        [Test]
        public void ReplaceValueInParent()
        {
            var node = new Node { Parent = new Node { KeyValueDictionary = new SortedDictionary<int, string>() { { 10, null } } } };
            node.ReplaceValueInParent(10, 11);
            Assert.AreEqual(11, node.Parent.KeyValueDictionary.ElementAt(0).Key);
        }
    }
}
