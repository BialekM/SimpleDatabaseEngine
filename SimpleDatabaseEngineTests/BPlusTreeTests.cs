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
            var foundNode = tree.FindLeafWithKey(20,tree.Root);
            Assert.AreEqual(foundNode,tree.Root.Children[1].Children[2]);
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
            Assert.AreEqual(15, foundNode.Keys[0] );
            Assert.AreEqual(20, foundNode.Keys[1]);
        }

    }
}
