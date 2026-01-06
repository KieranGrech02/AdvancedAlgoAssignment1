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

    // Build Cartesian Tree (Index-array method)
    private TreeNode? BuildCartesianTree()
    {
        int n = _array.Length;
        int[] parent = new int[n];
        int[] leftchild = new int[n];
        int[] rightchild = new int[n];

        for (int i = 0; i < n; i++)
        {
            parent[i] = -1;
            leftchild[i] = -1;
            rightchild[i] = -1;
        }

        int root = 0;

        for (int i = 1; i < n; i++)
        {
            int last = i - 1;
            rightchild[i] = -1;

            while (_array[last] >= _array[i] && last != root)
                last = parent[last];

            if (_array[last] >= _array[i])
            {
                parent[root] = i;
                leftchild[i] = root;
                root = i;
            }
            else if (rightchild[last] == -1)
            {
                rightchild[last] = i;
                parent[i] = last;
                leftchild[i] = -1;
            }
            else
            {
                parent[rightchild[last]] = i;
                leftchild[i] = rightchild[last];
                rightchild[last] = i;
                parent[i] = last;
            }
        }

        parent[root] = -1;
        return BuildTreeUtil(root, parent, leftchild, rightchild);
    }

    private TreeNode? BuildTreeUtil(int rootIdx, int[] parent, int[] leftchild, int[] rightchild)
    {
        if (rootIdx == -1) return null;

        TreeNode node = new TreeNode(_array[rootIdx], rootIdx);
        node.Left = BuildTreeUtil(leftchild[rootIdx], parent, leftchild, rightchild);
        node.Right = BuildTreeUtil(rightchild[rootIdx], parent, leftchild, rightchild);

        return node;
    }

    // Stack-Based Cartesian Tree
    public TreeNode? BuildStackBased()
    {
        if (_array.Length == 0) return null;

        Stack<TreeNode> stack = new Stack<TreeNode>();
        TreeNode? root = null;

        for (int idx = 0; idx < _array.Length; idx++)
        {
            TreeNode current = new TreeNode(_array[idx], idx);
            TreeNode? lastPopped = null;

            while (stack.Count > 0 && stack.Peek().Value >= current.Value)
            {
                lastPopped = stack.Pop();
            }

            if (stack.Count > 0)
            {
                stack.Peek().Right = current;
            }
            else
            {
                root = current;
            }

            if (lastPopped != null)
            {
                current.Left = lastPopped;
            }

            stack.Push(current);
        }

        while (root != null && root.Left != null)
            root = root.Left;

        return root ?? (stack.Count > 0 ? stack.Peek() : null);
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

    private void PreOrder(TreeNode? node, List<(int, int)> result)
    {
        if (node == null) return;
        result.Add((node.Value, node.Index));
        PreOrder(node.Left, result);
        PreOrder(node.Right, result);
    }

    // Print Tree
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

        if (left <= node.Index && node.Index <= right)
            return node;

        if (node.Index > right)
            return FindLca(node.Left, left, right);
        else
            return FindLca(node.Right, left, right);
    }

    // Verify Properties
    public (bool allOk, bool heapOk, bool bstOk) VerifyProperties()
    {
        bool heapOk = CheckHeap(Root);
        bool bstOk = CheckBstIndices(Root, int.MinValue, int.MaxValue);
        return (heapOk && bstOk, heapOk, bstOk);
    }

    private bool CheckHeap(TreeNode? node)
    {
        if (node == null) return true;
        if (node.Left != null && node.Left.Value < node.Value) return false;
        if (node.Right != null && node.Right.Value < node.Value) return false;
        return CheckHeap(node.Left) && CheckHeap(node.Right);
    }

    private bool CheckBstIndices(TreeNode? node, int minIdx, int maxIdx)
    {
        if (node == null) return true;
        if (node.Index <= minIdx || node.Index >= maxIdx) return false;
        return CheckBstIndices(node.Left, minIdx, node.Index) && CheckBstIndices(node.Right, node.Index, maxIdx);
    }

    // Logging for Uni Submission
    public void LogTree(string description)
    {
        Console.WriteLine("\n=== " + description + " ===");
        Console.WriteLine("Original array: [" + string.Join(", ", _array) + "]\n");

        Console.WriteLine("Tree structure (Root at top, L=left, R=right):");
        PrintTree();

        Console.WriteLine("\nIn-order traversal:");
        var inorder = InOrder();
        Console.WriteLine("[" + string.Join(", ", inorder.Select(t => t.value)) + "]");

        Console.WriteLine("\nPre-order traversal:");
        Console.WriteLine("[" + string.Join(", ", inorder.Select(t => $"({t.value}, idx={t.index})")) + "]");

        var (allOk, heapOk, bstOk) = VerifyProperties();
        Console.WriteLine("\nProperty Verification:");
        Console.WriteLine($"- Min-heap property: {(heapOk ? "Pass" : "Fail")}");
        Console.WriteLine($"- BST index property: {(bstOk ? "Pass" : "Fail")}");
        Console.WriteLine($"- Cartesian Tree valid: {(allOk ? "Yes" : "No")}");
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

        // Example 2: Stack-based vs Index-array
        int[] arr2 = { 9, 3, 7, 1, 8, 12, 10, 20, 15, 18, 5 };
        CartesianTree ctIndex = new CartesianTree(arr2);
        CartesianTree ctStack = new CartesianTree(arr2) { Root = new CartesianTree(arr2).BuildStackBased() };
        ctIndex.LogTree("Example 2: Index-array Construction");
        ctStack.LogTree("Example 2: Stack-based Construction");

        // Example 3: Range Minimum Queries
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

        // Example 4: Edge Cases
        int[] sorted = { 1, 2, 3, 4, 5 };
        int[] reverseSorted = { 5, 4, 3, 2, 1 };
        CartesianTree ctSorted = new CartesianTree(sorted);
        CartesianTree ctReverse = new CartesianTree(reverseSorted);
        ctSorted.LogTree("Example 4a: Sorted Array (Right-skewed)");
        ctReverse.LogTree("Example 4b: Reverse Sorted Array (Left-skewed)");
    }
}