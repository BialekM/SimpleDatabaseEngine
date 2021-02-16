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
        public void RemoveKeyWhenIsToLittleKeysInLeaf()
        {
            var node = new Node();
            Assert.AreEqual(true,node.TryAddKeyToNode(5));
            Assert.AreEqual(false,node.TryAddKeyToNode(5));
        }
    }
}
