using System;
using NUnit.Framework;
using SimpleDatabaseEngine;

namespace SimpleDatabaseEngineTests
{
    class BPlusTreeTests
    {
        [Test]
        public void AddMaxNumberOfKeysToRoot()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(3);

            Assert.AreEqual(true, tree.Root.IsLeaf);
            Assert.AreEqual(3, tree.Root.Keys[0]);
            Assert.AreEqual(5, tree.Root.Keys[1]);
            Assert.AreEqual(null, tree.Root.NextLeaf);
            Assert.AreEqual(null, tree.Root.PreviousLeaf);
            Assert.AreEqual(null, tree.Root.Parent);
        }

        [Test]
        public void SingleSplitRoot()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(3);
            tree.TryAddKeyToTree(10);

            Assert.AreEqual(false, tree.Root.IsLeaf);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].Keys[0]);
            Assert.AreEqual(5, tree.Root.Children[1].Keys[0]);
            Assert.AreEqual(10, tree.Root.Children[1].Keys[1]);
            Assert.AreEqual(tree.Root, tree.Root.Children[0].Parent);
            Assert.AreEqual(tree.Root, tree.Root.Children[1].Parent);
            Assert.AreEqual(null, tree.Root.Children[0].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[1], tree.Root.Children[0].NextLeaf);
            Assert.AreEqual(null, tree.Root.Children[1].NextLeaf);
            Assert.AreEqual(tree.Root.Children[0], tree.Root.Children[1].PreviousLeaf);
        }

        [Test]
        public void MultipleSplit()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(3);
            tree.TryAddKeyToTree(10);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);

            Assert.AreEqual(false, tree.Root.IsLeaf);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(5, tree.Root.Children[0].Keys[0]);
            Assert.AreEqual(15, tree.Root.Children[1].Keys[0]);
            Assert.AreEqual(false, tree.Root.Children[0].IsLeaf);
            Assert.AreEqual(false, tree.Root.Children[1].IsLeaf);
            Assert.AreEqual(3, tree.Root.Children[0].Children[0].Keys[0]);
            Assert.AreEqual(3, tree.Root.Children[0].Children[0].Keys[0]);
            Assert.AreEqual(10, tree.Root.Children[1].Children[0].Keys[0]);
            Assert.AreEqual(15, tree.Root.Children[1].Children[1].Keys[0]);
            Assert.AreEqual(20, tree.Root.Children[1].Children[1].Keys[1]);
            Assert.AreEqual(null, tree.Root.Children[0].Children[0].PreviousLeaf);
            Assert.AreEqual(true, tree.Root.Children[0].Children[0].IsLeaf);
            Assert.AreEqual(true, tree.Root.Children[0].Children[1].IsLeaf);
            Assert.AreEqual(true, tree.Root.Children[1].Children[0].IsLeaf);
            Assert.AreEqual(true, tree.Root.Children[1].Children[1].IsLeaf);
            Assert.AreEqual(tree.Root.Children[0].Children[0], tree.Root.Children[0].Children[1].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[0].Children[1], tree.Root.Children[0].Children[0].NextLeaf);
            Assert.AreEqual(tree.Root.Children[0].Children[1], tree.Root.Children[1].Children[0].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[1].Children[0], tree.Root.Children[0].Children[1].NextLeaf);
            Assert.AreEqual(tree.Root.Children[1].Children[0], tree.Root.Children[1].Children[1].PreviousLeaf);
            Assert.AreEqual(null, tree.Root.Children[1].Children[1].NextLeaf);
        }

        [Test]
        public void TryAddDuplicateKeyToTree()
        {
            var tree = new BPlusTree(3, 5);
            Assert.AreEqual(false, tree.TryAddKeyToTree(5));
        }

        [Test]
        public void FindLeafWithKey()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(3);
            tree.TryAddKeyToTree(10);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);
            tree.TryAddKeyToTree(12);
            tree.TryAddKeyToTree(14);
            var foundNode = tree.FindLeafWithKey(20, tree.Root);
            Assert.AreEqual(foundNode, tree.Root.Children[1].Children[2]);
        }

        [Test]
        public void FindLeafToAdd()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(3);
            tree.TryAddKeyToTree(10);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);
            tree.TryAddKeyToTree(12);
            tree.TryAddKeyToTree(14);
            var foundNode = tree.FindLeafToAdd(25, tree.Root);
            Assert.AreEqual(15, foundNode.Keys[0]);
            Assert.AreEqual(20, foundNode.Keys[1]);
        }

        [Test]
        public void DeleteFromTreeWithMergeBiggerChild()
        {
            var tree = new BPlusTree(3, 10);
            tree.TryAddKeyToTree(5);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);
            tree.TryAddKeyToTree(25);
            tree.DeleteKey(10);

            Assert.AreEqual(2, tree.Root.Keys.Count);
            Assert.AreEqual(15, tree.Root.Keys[0]);
            Assert.AreEqual(20, tree.Root.Keys[1]);
            Assert.AreEqual(5, tree.Root.Children[0].Keys[0]);
            Assert.AreEqual(15, tree.Root.Children[1].Keys[0]);
            Assert.AreEqual(20, tree.Root.Children[2].Keys[0]);
            Assert.AreEqual(25, tree.Root.Children[2].Keys[1]);
            Assert.AreEqual(null, tree.Root.Children[0].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[1], tree.Root.Children[0].NextLeaf);
            Assert.AreEqual(tree.Root.Children[0], tree.Root.Children[1].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[2], tree.Root.Children[1].NextLeaf);
            Assert.AreEqual(tree.Root.Children[1], tree.Root.Children[2].PreviousLeaf);
            Assert.AreEqual(null, tree.Root.Children[2].NextLeaf);
        }

        [Test]
        public void DeleteFromTreeWithMergeMediumChild()
        {
            var tree = new BPlusTree(3, 10);
            tree.TryAddKeyToTree(5);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);
            tree.TryAddKeyToTree(25);
            tree.TryAddKeyToTree(6);
            tree.TryAddKeyToTree(7);
            tree.DeleteKey(5);
            tree.DeleteKey(7);

            Assert.AreEqual(15, tree.Root.Keys[0]);
            Assert.AreEqual(10, tree.Root.Children[0].Keys[0]);
            Assert.AreEqual(20, tree.Root.Children[1].Keys[0]);
            Assert.AreEqual(6, tree.Root.Children[0].Children[0].Keys[0]);
            Assert.AreEqual(10, tree.Root.Children[0].Children[1].Keys[0]);
            Assert.AreEqual(15, tree.Root.Children[1].Children[0].Keys[0]);
            Assert.AreEqual(20, tree.Root.Children[1].Children[1].Keys[0]);
            Assert.AreEqual(25, tree.Root.Children[1].Children[1].Keys[1]);
        }

        [Test]
        public void EmptyTreeTest()
        {
            var tree = new BPlusTree(3, 5);
            tree.TryAddKeyToTree(10);
            tree.TryAddKeyToTree(15);
            tree.TryAddKeyToTree(20);
            tree.TryAddKeyToTree(25);
            tree.TryAddKeyToTree(30);
            tree.TryAddKeyToTree(35);
            tree.TryAddKeyToTree(40);
            tree.TryAddKeyToTree(45);
            tree.TryAddKeyToTree(50);
            tree.TryAddKeyToTree(55);
            tree.TryAddKeyToTree(41);
            tree.TryAddKeyToTree(42);
            tree.DeleteKey(50);
            tree.DeleteKey(55);
            tree.DeleteKey(42);
            tree.DeleteKey(41);
            tree.DeleteKey(40);
            tree.DeleteKey(45);
            tree.DeleteKey(30);
            tree.DeleteKey(35);
            tree.DeleteKey(20);
            tree.DeleteKey(25);
            tree.DeleteKey(15);
            tree.DeleteKey(10);
            tree.DeleteKey(5);
            Assert.AreEqual(0, tree.Root.Keys.Count);
            Assert.AreEqual(0, tree.Root.Children.Count);
        }
    }
}
