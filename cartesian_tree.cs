using System;
using System.Collections.Generic;

public class TreeNode
{
    public int Value { get; set; }
    public int Index { get; set; }
    public TreeNode Left { get; set; }
    public TreeNode Right { get; set; }

    public TreeNode(int value, int index)
    {
        Value = value;
        Index = index;
        Left = null;
        Right = null;
    }
}

public class CartesianTree
{
    /*
     * Cartesian Tree Implementation
     *
     * Properties:
     * 1. Binary Search Tree w.r.t array indices (in-order traversal preserves input order)
     * 2. Min-Heap w.r.t values
     *
     * Time Complexity:
     * - Construction: O(n)
     * - Traversals: O(n)
     * - RMQ query (this implementation): O(h), worst-case O(n)
     *
     * Space Complexity: O(n)
     */

    private readonly int[] _array;
    public TreeNode Root { get; private set; }

    public CartesianTree(int[] array)
    {
        _array = array;
        Root = array.Length > 0 ? BuildCartesianTree() : null;
    }

    private TreeNode BuildCartesianTree()
    {
        Stack<TreeNode> stack = new Stack<TreeNode>();

        for (int i = 0; i < _array.Length; i++)
        {
            TreeNode current = new TreeNode(_array[i], i);
            TreeNode lastPopped = null;

            // Maintain monotonic increasing stack
            while (stack.Count > 0 && stack.Peek().Value > current.Value)
            {
                lastPopped = stack.Pop();
            }

            // Current becomes right child of stack top
            if (stack.Count > 0)
            {
                stack.Peek().Right = current;
            }

            // Last popped becomes left child of current
            if (lastPopped != null)
            {
                current.Left = lastPopped;
            }

            stack.Push(current);
        }

        // Root is bottom of the stack
        while (stack.Count > 1)
        {
            stack.Pop();
        }

        return stack.Pop();
    }

    // ---------- Traversals ----------

    public List<(int value, int index)> InOrder()
    {
        List<(int, int)> result = new List<(int, int)>();
        InOrder(Root, result);
        return result;
    }

    private void InOrder(TreeNode node, List<(int, int)> result)
    {
        if (node == null) return;

        InOrder(node.Left, result);
        result.Add((node.Value, node.Index));
        InOrder(node.Right, result);
    }

    public List<(int value, int index)> PreOrder()
    {
        List<(int, int)> result = new List<(int, int)>();
        PreOrder(Root, result);
        return result;
    }

    private void PreOrder(TreeNode node, List<(int, int)> result)
    {
        if (node == null) return;

        result.Add((node.Value, node.Index));
        PreOrder(node.Left, result);
        PreOrder(node.Right, result);
    }

    // ---------- Visualisation ----------

    public void PrintTree()
    {
        PrintTree(Root, 0, "Root: ");
    }

    private void PrintTree(TreeNode node, int level, string prefix)
    {
        if (node == null) return;

        Console.WriteLine(
            new string(' ', level * 4) +
            prefix +
            $"val={node.Value}, idx={node.Index}"
        );

        if (node.Left != null || node.Right != null)
        {
            if (node.Left != null)
                PrintTree(node.Left, level + 1, "L--- ");
            else
                Console.WriteLine(new string(' ', (level + 1) * 4) + "L--- None");

            if (node.Right != null)
                PrintTree(node.Right, level + 1, "R--- ");
            else
                Console.WriteLine(new string(' ', (level + 1) * 4) + "R--- None");
        }
    }

    // ---------- Range Minimum Query ----------

    public int? GetRangeMinimum(int left, int right)
    {
        TreeNode lca = FindLca(Root, left, right);
        return lca?.Value;
    }

    private TreeNode FindLca(TreeNode node, int left, int right)
    {
        if (node == null) return null;

        if (left <= node.Index && node.Index <= right)
            return node;

        if (node.Index > right)
            return FindLca(node.Left, left, right);
        else
            return FindLca(node.Right, left, right);
    }

    // ---------- Verification ----------

    public (bool allOk, bool heapOk, bool bstOk) VerifyProperties()
    {
        bool heapOk = CheckHeap(Root);
        bool bstOk = CheckBstIndices(Root, int.MinValue, int.MaxValue);
        return (heapOk && bstOk, heapOk, bstOk);
    }

    private bool CheckHeap(TreeNode node)
    {
        if (node == null) return true;

        if (node.Left != null && node.Left.Value < node.Value)
            return false;
        if (node.Right != null && node.Right.Value < node.Value)
            return false;

        return CheckHeap(node.Left) && CheckHeap(node.Right);
    }

    private bool CheckBstIndices(TreeNode node, int minIdx, int maxIdx)
    {
        if (node == null) return true;

        if (node.Index <= minIdx || node.Index >= maxIdx)
            return false;

        return
            CheckBstIndices(node.Left, minIdx, node.Index) &&
            CheckBstIndices(node.Right, node.Index, maxIdx);
    }
}
