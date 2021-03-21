using System.Linq;
using NUnit.Framework;
using SimpleDatabaseEngine;

namespace SimpleDatabaseEngineTests
{
    class BPlusTreeTests
    {
        [Test]
        public void AddMaxNumberOfKeysToRoot()
        {
            var tree = new BPlusTree(5, "5");
            tree.TryAppendElementToTree(3, "3");

            Assert.AreEqual(true, tree.Root.IsLeaf);
            Assert.AreEqual(3, tree.Root.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(5, tree.Root.KeyValueDictionary.ElementAt(1).Key);
            Assert.AreEqual(null, tree.Root.NextLeaf);
            Assert.AreEqual(null, tree.Root.PreviousLeaf);
            Assert.AreEqual(null, tree.Root.Parent);
        }

        [Test]
        public void SingleSplitRoot()
        {
            var tree = new BPlusTree(5, "5");
            tree.TryAppendElementToTree(3, "3");
            tree.TryAppendElementToTree(10, "10");

            Assert.AreEqual(false, tree.Root.IsLeaf);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(5, tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(10, tree.Root.Children[1].KeyValueDictionary.ElementAt(1).Key);
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
            var tree = new BPlusTree(5, "5");
            tree.TryAppendElementToTree(3, "3");
            tree.TryAppendElementToTree(10, "10");
            tree.TryAppendElementToTree(15, "15");
            tree.TryAppendElementToTree(20, "20");

            Assert.AreEqual(false, tree.Root.IsLeaf);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(5, tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(15, tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(false, tree.Root.Children[0].IsLeaf);
            Assert.AreEqual(false, tree.Root.Children[1].IsLeaf);
            Assert.AreEqual(3, tree.Root.Children[0].Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(3, tree.Root.Children[0].Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(10, tree.Root.Children[1].Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(15, tree.Root.Children[1].Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(20, tree.Root.Children[1].Children[1].KeyValueDictionary.ElementAt(1).Key);
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
            var tree = new BPlusTree(5, "test");
            Assert.AreEqual(false, tree.TryAppendElementToTree(5,"5"));
        }

        [Test]
        public void FindLeafWithKey()
        {
            var tree = new BPlusTree(5, "5");
            tree.TryAppendElementToTree(3, "3");
            tree.TryAppendElementToTree(10, "10");
            tree.TryAppendElementToTree(15, "15");
            tree.TryAppendElementToTree(20, "20");
            tree.TryAppendElementToTree(12, "12");
            tree.TryAppendElementToTree(14, "14");
            var foundNode = tree.FindLeafWithKey(20, tree.Root);
            Assert.AreEqual(foundNode, tree.Root.Children[1].Children[2]);
        }

        [Test]
        public void FindLeafToAdd()
        {
            var tree = new BPlusTree(5, "test");
            tree.TryAppendElementToTree(3, "3");
            tree.TryAppendElementToTree(10, "10");
            tree.TryAppendElementToTree(15, "15");
            tree.TryAppendElementToTree(20, "20");
            tree.TryAppendElementToTree(12, "12");
            tree.TryAppendElementToTree(14, "14");
            var foundNode = tree.FindLeafToAdd(25, tree.Root);
            Assert.AreEqual(15, foundNode.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(20, foundNode.KeyValueDictionary.ElementAt(1).Key);
        }

        [Test]
        public void DeleteFromTreeWithMergeBiggerChild()
        {
            var tree = new BPlusTree(10, "test");
            tree.TryAppendElementToTree(5,"5");
            tree.TryAppendElementToTree(15,"15");
            tree.TryAppendElementToTree(20,"20");
            tree.TryAppendElementToTree(25, "20");
            tree.Delete(10);

            Assert.AreEqual(2, tree.Root.KeyValueDictionary.Count);
            Assert.AreEqual(15, tree.Root.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(null, tree.Root.KeyValueDictionary.ElementAt(0).Value);
            Assert.AreEqual(20, tree.Root.KeyValueDictionary.ElementAt(1).Key);
            Assert.AreEqual(null, tree.Root.KeyValueDictionary.ElementAt(1).Value);
            Assert.AreEqual(5, tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(15, tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(20, tree.Root.Children[2].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(25, tree.Root.Children[2].KeyValueDictionary.ElementAt(1).Key);
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
            var tree = new BPlusTree(10, "10");
            tree.TryAppendElementToTree(5, "5");
            tree.TryAppendElementToTree(15, "15");
            tree.TryAppendElementToTree(20, "20");
            tree.TryAppendElementToTree(25, "25");
            tree.TryAppendElementToTree(6, "6");
            tree.TryAppendElementToTree(7, "7");
            tree.Delete(5);
            tree.Delete(7);

            Assert.AreEqual(15, tree.Root.KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(10, tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(20, tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(6, tree.Root.Children[0].Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(10, tree.Root.Children[0].Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(15, tree.Root.Children[1].Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(20, tree.Root.Children[1].Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual(25, tree.Root.Children[1].Children[1].KeyValueDictionary.ElementAt(1).Key);
        }

        [Test]
        public void EmptyTreeTest()
        {
            var tree = new BPlusTree(5, "5");
            tree.TryAppendElementToTree(10, "10");
            tree.TryAppendElementToTree(15,"15");
            tree.TryAppendElementToTree(20, "20");
            tree.TryAppendElementToTree(25, "25");
            tree.TryAppendElementToTree(30, "30");
            tree.TryAppendElementToTree(35, "35");
            tree.TryAppendElementToTree(40, "40");
            tree.TryAppendElementToTree(45, "45");
            tree.TryAppendElementToTree(50, "50");
            tree.TryAppendElementToTree(55, "55");
            tree.TryAppendElementToTree(41, "41");
            tree.TryAppendElementToTree(42, "42");
            tree.Delete(50);
            tree.Delete(55);
            tree.Delete(42);
            tree.Delete(41);
            tree.Delete(40);
            tree.Delete(45);
            tree.Delete(30);
            tree.Delete(35);
            tree.Delete(20);
            tree.Delete(25);
            tree.Delete(15);
            tree.Delete(10);
            tree.Delete(5);
            Assert.AreEqual(0, tree.Root.KeyValueDictionary.Count);
            Assert.AreEqual(0, tree.Root.Children.Count);
        }

        [Test]
        public void AppendKeyWithValuesToTreeTest()
        {
            var tree = new BPlusTree(5, "test");
            tree.TryAppendElementToTree(3, "test1");
            tree.TryAppendElementToTree(10, "test2");

            Assert.AreEqual(false, tree.Root.IsLeaf);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual("test1", tree.Root.Children[0].KeyValueDictionary.ElementAt(0).Value);
            Assert.AreEqual(5, tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Key);
            Assert.AreEqual("test", tree.Root.Children[1].KeyValueDictionary.ElementAt(0).Value);
            Assert.AreEqual(10, tree.Root.Children[1].KeyValueDictionary.ElementAt(1).Key);
            Assert.AreEqual("test2", tree.Root.Children[1].KeyValueDictionary.ElementAt(1).Value);
            Assert.AreEqual(tree.Root, tree.Root.Children[0].Parent);
            Assert.AreEqual(tree.Root, tree.Root.Children[1].Parent);
            Assert.AreEqual(null, tree.Root.Children[0].PreviousLeaf);
            Assert.AreEqual(tree.Root.Children[1], tree.Root.Children[0].NextLeaf);
            Assert.AreEqual(null, tree.Root.Children[1].NextLeaf);
            Assert.AreEqual(tree.Root.Children[0], tree.Root.Children[1].PreviousLeaf);
            Assert.AreEqual(null, tree.Root.KeyValueDictionary.ElementAt(0).Value);
        }
    }
}
