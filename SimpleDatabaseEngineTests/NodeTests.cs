using NUnit.Framework;
using SimpleDatabaseEngine;
using System;
using System.Collections.Generic;
using System.Text;

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
            Node node = new Node();
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
        public void AddDuplicateKeyToNode()
        {
            BPlusTree tree = new BPlusTree(3, 5);
            tree.AddKeyToTree(15);
            tree.AddKeyToTree(25);
            tree.AddKeyToTree(35);
            tree.AddKeyToTree(45);
            //tree.Root.TryAddKeyToNode(15);
            //tree.Root.TryAddKeyToNode(25);
            //tree.SplitNode(tree.Root);
            //var node = tree.GoToLeaf(7, tree.Root);
            //var node1 = tree.GoToLeaf(16, tree.Root);
            Console.WriteLine("kaszanka");
        }
    }
}
