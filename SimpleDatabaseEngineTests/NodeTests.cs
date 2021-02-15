using NUnit.Framework;
using SimpleDatabaseEngine;
using System;

namespace SimpleDatabaseEngineTests
{
    class NodeTests
    {
        [SetUp]
        public void Setup()
        { 
        }

        [Test]
        public void AddKeysToNode()
        {
            var node = new Node();
            node.AddKeyToLeaf(5);
            node.AddKeyToLeaf(3);
            node.AddKeyToLeaf(10);
            node.AddKeyToLeaf(11);
            node.AddKeyToLeaf(2);
            node.AddKeyToLeaf(21);
            Assert.AreEqual(2, node.Keys[0]);
            Assert.AreEqual(3, node.Keys[1]);
            Assert.AreEqual(5, node.Keys[2]);
            Assert.AreEqual(10, node.Keys[3]);
            Assert.AreEqual(11, node.Keys[4]);
            Assert.AreEqual(21, node.Keys[5]);
        }

        [Test]
        public void AddDuplicateKeyToNode()
        {
            var tree = new BPlusTree(3, 5);
            tree.AddKeyToTree(15);
            tree.AddKeyToTree(25);
            tree.AddKeyToTree(35);
            tree.AddKeyToTree(45);
            tree.AddKeyToTree(50);
            tree.AddKeyToTree(65);
            tree.AddKeyToTree(10);
            tree.AddKeyToTree(2);
            tree.AddKeyToTree(21);
            tree.DeleteKey(25);
            //tree.SplitNode(tree.Root);
            //var node = tree.GoToLeaf(7, tree.Root);
            //var node1 = tree.GoToLeaf(16, tree.Root);
            Console.WriteLine("kaszanka");
        }

        [Test]
        public void RemoveKeyWhenIsToLittleKeysInLeaf()
        {
            var tree = new BPlusTree(3, 5);
            tree.AddKeyToTree(15);
            tree.AddKeyToTree(20);
            tree.AddKeyToTree(25);
            tree.AddKeyToTree(30);
            tree.AddKeyToTree(35);
            tree.AddKeyToTree(45);
            tree.AddKeyToTree(55);
            tree.DeleteKey(20);
        }
    }
}
