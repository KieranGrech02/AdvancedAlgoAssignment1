using System;
using System.Collections.Generic;
using System.Linq;

public class TreeNode
{
    public int Value { get; set; }
    public int Index { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }

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
    public int[] _array { get; }
    public TreeNode? Root { get; set; }

    public CartesianTree(int[] array)
    {
        _array = array;
        Root = array.Length > 0 ? BuildCartesianTree() : null;
    }

    // Build Stack-Based Cartesian Tree
    private TreeNode? BuildCartesianTree()
    {
        // empty array edge case
        if (_array.Length == 0) return null;

        // Stack maintains nodes in increasing order by value
        Stack<TreeNode> stack = new Stack<TreeNode>();
        TreeNode? root = null;

        // Process each element left to right
        for (int idx = 0; idx < _array.Length; idx++)
        {
            // Create new node for current element
            TreeNode current = new TreeNode(_array[idx], idx);
            TreeNode? lastPopped = null;

            // Pop all nodes with value >= current (maintain min-heap property)
            // These nodes will become the left subtree of current
            while (stack.Count > 0 && stack.Peek().Value >= current.Value)
            {
                lastPopped = stack.Pop();
            }

            // If stack not empty, current becomes right child of stack top
            // (maintains BST index property: parent index < current index)
            if (stack.Count > 0)
            {
                stack.Peek().Right = current;
            }
            else
            {
                // Stack is empty, so current has smallest value seen so far
                // It becomes the new root
                root = current;
            }

            // All popped nodes become left child of current
            // (maintains index property: they all come before current)
            if (lastPopped != null)
            {
                current.Left = lastPopped;
            }

            // Push current node to maintain stack invariant
            stack.Push(current);
        }

        return root;
    }

    // Traversals
    public List<(int value, int index)> InOrder()
    {
        List<(int, int)> result = new List<(int, int)>();
        InOrder(Root, result);
        return result;
    }

    private void InOrder(TreeNode? node, List<(int, int)> result)
    {
        if (node == null) return;
        InOrder(node.Left, result);  // goto left subtree
        result.Add((node.Value, node.Index));  // goto current node
        InOrder(node.Right, result);  // goto right subtree
    }

    public List<(int value, int index)> PreOrder()
    {
        List<(int, int)> result = new List<(int, int)>();
        PreOrder(Root, result);
        return result;
    }

    // Used in testing
    private void PreOrder(TreeNode? node, List<(int, int)> result)
    {
        if (node == null) {
            return;
        }
        result.Add((node.Value, node.Index));  // goto current node first
        PreOrder(node.Left, result);  // goto left subtree
        PreOrder(node.Right, result);  // goto right subtree
    }

    // Print Tree for a nicer output of the code
    public void PrintTree()
    {
        PrintTree(Root, 0, "Root: ");
    }

    private void PrintTree(TreeNode? node, int level, string prefix)
    {
        if (node == null) return;

        Console.WriteLine(new string(' ', level * 4) + prefix + $"val={node.Value}, idx={node.Index}");

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

    // Range Minimum Query
    public int? GetRangeMinimum(int left, int right)
    {
        TreeNode? lca = FindLca(Root, left, right);
        return lca?.Value;
    }

    private TreeNode? FindLca(TreeNode? node, int left, int right)
    {
        if (node == null) return null;

        // If current node's index is in range, LCA (lowest common ancestor)
        // Due to min-heap property, node has the minimum value in the range
        if (left <= node.Index && node.Index <= right)
            return node;

        // Range is entirely to the left
        if (node.Index > right)
            return FindLca(node.Left, left, right);
        else
            // Range is entirely to the right
            return FindLca(node.Right, left, right);
    }

    // Verify (Used in tests)
    public (bool allOk, bool heapOk, bool bstOk) VerifyProperties()
    {
        bool heapOk = CheckHeap(Root);
        bool bstOk = CheckBstIndices(Root, int.MinValue, int.MaxValue);
        return (heapOk && bstOk, heapOk, bstOk);
    }

    private bool CheckHeap(TreeNode? node)
    {
        if (node == null) {
            return true;
        }
        // Parent must be <= both children (min-heap property)
        if (node.Left != null && node.Left.Value < node.Value) {
            return false;
        }
        if (node.Right != null && node.Right.Value < node.Value) {
            return false;
        }
        // Recursively check all subtrees
        return CheckHeap(node.Left) && CheckHeap(node.Right);
    }

    private bool CheckBstIndices(TreeNode? node, int minIdx, int maxIdx)
    {
        if (node == null) {
            return true;
        }
        // Node's index must be within valid range (BST property on indices)
        if (node.Index <= minIdx || node.Index >= maxIdx) {
            return false;
        }
        // Left subtree indices < node index < right subtree indices
        return CheckBstIndices(node.Left, minIdx, node.Index) && CheckBstIndices(node.Right, node.Index, maxIdx);
    }

    // Print Tree in standard format
    public void LogTree(string description)
    {
        Console.WriteLine("\n" + description);
        PrintTree();
        Console.WriteLine("\nInorder traversal of the constructed tree :");
        var inorder = InOrder();
        Console.WriteLine(string.Join(" ", inorder.Select(t => t.value)));
    }
}

// Runner
public class CartesianTreeRunner
{
    public static void Run()
    {
        // Example 1
        int[] arr1 = { 5, 10, 40, 30, 28 };
        CartesianTree ct1 = new CartesianTree(arr1);
        ct1.LogTree("Example 1: Standard Cartesian Tree");

        // Example 2 Larger array
        int[] arr2 = { 9, 3, 7, 1, 8, 12, 10, 20, 15, 18, 5 };
        CartesianTree ct2 = new CartesianTree(arr2);
        ct2.LogTree("Example 2: Stack-based Construction");

        // Example 3 Range Minimum Queries
        int[] arr3 = { 3, 2, 6, 1, 9, 7, 8 };
        CartesianTree ct3 = new CartesianTree(arr3);
        Console.WriteLine("\n=== Example 3: Range Minimum Queries ===");
        var queries = new[] { (0, 3), (2, 5), (1, 6), (0, 6) };
        foreach (var (l, r) in queries)
        {
            int? result = ct3.GetRangeMinimum(l, r);
            int actualMin = arr3[l..(r + 1)].Min();
            Console.WriteLine($"RMQ({l}, {r}) = {result} (expected: {actualMin})");
        }

        // Example 4 Edge Cases
        int[] sorted = { 1, 2, 3, 4, 5 };
        int[] reverseSorted = { 5, 4, 3, 2, 1 };
        CartesianTree ctSorted = new CartesianTree(sorted);
        CartesianTree ctReverse = new CartesianTree(reverseSorted);
        ctSorted.LogTree("Example 4a: Sorted Array (Right-skewed)");
        ctReverse.LogTree("Example 4b: Reverse Sorted Array (Left-skewed)");
    }
}