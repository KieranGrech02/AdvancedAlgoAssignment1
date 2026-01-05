using NUnit.Framework;
using System;
using System.Linq;

[TestFixture]
public class CartesianTreeTests
{
    // ---------- Construction Tests ----------

    [Test]
    public void Construction_EmptyArray_ProducesNullRoot()
    {
        CartesianTree tree = new CartesianTree(Array.Empty<int>());
        Assert.IsNull(tree.Root);
    }

    [Test]
    public void Construction_SingleElementArray_CreatesSingleNode()
    {
        CartesianTree tree = new CartesianTree(new[] { 42 });

        Assert.NotNull(tree.Root);
        Assert.AreEqual(42, tree.Root.Value);
        Assert.AreEqual(0, tree.Root.Index);
        Assert.IsNull(tree.Root.Left);
        Assert.IsNull(tree.Root.Right);
    }

    // ---------- Inorder Property Tests ----------

    [Test]
    public void InOrderTraversal_PreservesOriginalArrayOrder()
    {
        int[] input = { 5, 2, 8, 1, 6 };
        CartesianTree tree = new CartesianTree(input);

        var inorder = tree.InOrder().Select(x => x.value).ToArray();

        Assert.AreEqual(input, inorder,
            "In-order traversal must match original array order");
    }

    // ---------- Heap Property Tests ----------

    [Test]
    public void VerifyProperties_MinHeapProperty_HoldsForSimpleArray()
    {
        int[] input = { 4, 3, 2, 1 };
        CartesianTree tree = new CartesianTree(input);

        var (_, heapOk, _) = tree.VerifyProperties();

        Assert.IsTrue(heapOk, "Min-heap property must hold");
    }

    // ---------- BST (Index) Property Tests ----------

    [Test]
    public void VerifyProperties_BstIndexProperty_HoldsForRandomArray()
    {
        int[] input = { 7, 1, 5, 3, 9, 2 };
        CartesianTree tree = new CartesianTree(input);

        var (_, _, bstOk) = tree.VerifyProperties();

        Assert.IsTrue(bstOk, "BST property w.r.t indices must hold globally");
    }

    // ---------- Combined Property Tests ----------

    [Test]
    public void VerifyProperties_AllPropertiesHold_ForTypicalInput()
    {
        int[] input = { 3, 1, 4, 0, 2 };
        CartesianTree tree = new CartesianTree(input);

        var (allOk, heapOk, bstOk) = tree.VerifyProperties();

        Assert.IsTrue(allOk);
        Assert.IsTrue(heapOk);
        Assert.IsTrue(bstOk);
    }

    // ---------- Edge Case Shape Tests ----------

    [Test]
    public void Construction_StrictlyIncreasingArray_ProducesRightSkewedTree()
    {
        int[] input = { 1, 2, 3, 4, 5 };
        CartesianTree tree = new CartesianTree(input);

        TreeNode current = tree.Root;
        for (int i = 0; i < input.Length; i++)
        {
            Assert.NotNull(current);
            Assert.AreEqual(input[i], current.Value);
            Assert.IsNull(current.Left);
            current = current.Right;
        }
    }

    [Test]
    public void Construction_StrictlyDecreasingArray_ProducesLeftSkewedTree()
    {
        int[] input = { 5, 4, 3, 2, 1 };
        CartesianTree tree = new CartesianTree(input);

        TreeNode current = tree.Root;
        for (int i = input.Length - 1; i >= 0; i--)
        {
            Assert.NotNull(current);
            Assert.AreEqual(input[i], current.Value);
            Assert.IsNull(current.Right);
            current = current.Left;
        }
    }

    // ---------- Duplicate Values ----------

    [Test]
    public void Construction_WithDuplicateValues_StillSatisfiesProperties()
    {
        int[] input = { 2, 1, 1, 3, 1 };
        CartesianTree tree = new CartesianTree(input);

        var (allOk, _, _) = tree.VerifyProperties();

        Assert.IsTrue(allOk,
            "Tree with duplicate values must still satisfy heap and index properties");
    }

    // ---------- RMQ Tests ----------

    [Test]
    public void RangeMinimumQuery_ReturnsCorrectMinimum()
    {
        int[] input = { 4, 2, 6, 1, 5, 3 };
        CartesianTree tree = new CartesianTree(input);

        int? min = tree.GetRangeMinimum(1, 4);

        Assert.AreEqual(1, min);
    }

    [Test]
    public void RangeMinimumQuery_SingleElementRange_ReturnsThatElement()
    {
        int[] input = { 9, 7, 5, 3 };
        CartesianTree tree = new CartesianTree(input);

        int? min = tree.GetRangeMinimum(2, 2);

        Assert.AreEqual(5, min);
    }

    // ---------- Stress / Randomised Tests ----------

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

            Assert.IsTrue(allOk,
                "Randomised input must preserve Cartesian Tree invariants");
        }
    }
}
