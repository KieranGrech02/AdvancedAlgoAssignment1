using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class CartesianTreeTests
{
    private void LogTreeInOrder(CartesianTree tree, string message = "")
    {
        var inorder = tree.InOrder().Select(x => x.value);
        TestContext.WriteLine($"{message} InOrder: [{string.Join(", ", inorder)}]");
    }

    private void LogTreePreOrder(CartesianTree tree, string message = "")
    {
        var preorder = tree.PreOrder().Select(x => $"({x.value}, {x.index})");
        TestContext.WriteLine($"{message} PreOrder: [{string.Join(", ", preorder)}]");
    }

    [Test]
    public void Construction_EmptyArray_ProducesNullRoot()
    {
        CartesianTree tree = new CartesianTree(Array.Empty<int>());
        TestContext.WriteLine("Testing empty array construction...");
        Assert.That(tree.Root, Is.Null);
    }

    [Test]
    public void Construction_SingleElementArray_CreatesSingleNode()
    {
        CartesianTree tree = new CartesianTree(new[] { 42 });
        LogTreeInOrder(tree, "Single element array");
        Assert.That(tree.Root, Is.Not.Null);
        Assert.That(tree.Root!.Value, Is.EqualTo(42));
        Assert.That(tree.Root.Index, Is.EqualTo(0));
        Assert.That(tree.Root.Left, Is.Null);
        Assert.That(tree.Root.Right, Is.Null);
    }

    [Test]
    public void InOrderTraversal_PreservesOriginalArrayOrder()
    {
        int[] input = { 5, 2, 8, 1, 6 };
        CartesianTree tree = new CartesianTree(input);
        LogTreeInOrder(tree, "Checking in-order traversal");
        var inorder = tree.InOrder().Select(x => x.value).ToArray();
        Assert.That(inorder, Is.EqualTo(input));
    }

    [Test]
    public void VerifyProperties_MinHeapProperty_HoldsForSimpleArray()
    {
        int[] input = { 4, 3, 2, 1 };
        CartesianTree tree = new CartesianTree(input);
        var (_, heapOk, _) = tree.VerifyProperties();
        LogTreePreOrder(tree, "Min-heap check:");
        Assert.That(heapOk, Is.True);
    }

    [Test]
    public void VerifyProperties_BstIndexProperty_HoldsForRandomArray()
    {
        int[] input = { 7, 1, 5, 3, 9, 2 };
        CartesianTree tree = new CartesianTree(input);
        var (_, _, bstOk) = tree.VerifyProperties();
        LogTreePreOrder(tree, "BST index check:");
        Assert.That(bstOk, Is.True);
    }

    [Test]
    public void VerifyProperties_AllPropertiesHold_ForTypicalInput()
    {
        int[] input = { 3, 1, 4, 0, 2 };
        CartesianTree tree = new CartesianTree(input);
        var (allOk, heapOk, bstOk) = tree.VerifyProperties();
        LogTreePreOrder(tree, "Full property verification:");
        Assert.That(allOk, Is.True);
        Assert.That(heapOk, Is.True);
        Assert.That(bstOk, Is.True);
    }

    [Test]
    public void Construction_StrictlyIncreasingArray_ProducesRightSkewedTree()
    {
        int[] input = { 1, 2, 3, 4, 5 };
        CartesianTree tree = new CartesianTree(input);
        LogTreePreOrder(tree, "Right-skewed tree:");

        TreeNode? current = tree.Root;
        for (int i = 0; i < input.Length; i++)
        {
            Assert.That(current, Is.Not.Null, $"Node should exist at index {i}");
            Assert.That(current!.Value, Is.EqualTo(input[i]));
            Assert.That(current.Left, Is.Null);
            if (i < input.Length - 1)
                Assert.That(current.Right, Is.Not.Null, $"Right child should exist at index {i}");
            current = current.Right;
        }
        Assert.That(current, Is.Null, "No node should exist after last element");
    }

    [Test]
    public void Construction_StrictlyDecreasingArray_ProducesLeftSkewedTree()
    {
        int[] input = { 5, 4, 3, 2, 1 };
        CartesianTree tree = new CartesianTree(input);
        LogTreePreOrder(tree, "Left-skewed tree:");

        TreeNode? current = tree.Root;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            Assert.That(current, Is.Not.Null, $"Node should exist at index {i}");
            Assert.That(current!.Value, Is.EqualTo(input[i]));
            Assert.That(current.Right, Is.Null);
            if (i > 0)
                Assert.That(current.Left, Is.Not.Null, $"Left child should exist at index {i}");
            current = current.Left;
        }
        Assert.That(current, Is.Null, "No node should exist after last element");
    }

    [Test]
    public void Construction_WithDuplicateValues_StillSatisfiesProperties()
    {
        int[] input = { 2, 1, 1, 3, 1 };
        CartesianTree tree = new CartesianTree(input);
        LogTreePreOrder(tree, "Tree with duplicates:");
        var (allOk, _, _) = tree.VerifyProperties();
        Assert.That(allOk, Is.True);
    }

    [Test]
    public void RangeMinimumQuery_ReturnsCorrectMinimum()
    {
        int[] input = { 4, 2, 6, 1, 5, 3 };
        CartesianTree tree = new CartesianTree(input);
        int? min = tree.GetRangeMinimum(1, 4);
        TestContext.WriteLine($"RMQ(1, 4) = {min}");
        Assert.That(min, Is.EqualTo(1));
    }

    [Test]
    public void RangeMinimumQuery_SingleElementRange_ReturnsThatElement()
    {
        int[] input = { 9, 7, 5, 3 };
        CartesianTree tree = new CartesianTree(input);
        int? min = tree.GetRangeMinimum(2, 2);
        TestContext.WriteLine($"RMQ(2, 2) = {min}");
        Assert.That(min, Is.EqualTo(5));
    }

    [Test]
    public void RandomArrays_VerifyPropertiesHold()
    {
        Random rng = new Random(12345);
        for (int t = 0; t < 50; t++)
        {
            int size = rng.Next(1, 50);
            int[] input = Enumerable.Range(0, size)
                                    .Select(_ => rng.Next(0, 100))
                                    .ToArray();
            CartesianTree tree = new CartesianTree(input);
            var (allOk, _, _) = tree.VerifyProperties();
            TestContext.WriteLine($"Test {t}: size={size}, allOk={allOk}");
            Assert.That(allOk, Is.True);
        }
    }
}
