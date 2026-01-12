# Cartesian Tree Implementation

A C# implementation of a Cartesian Tree data structure using the stack-based construction algorithm.

## Overview

This project implements a Cartesian Tree - a binary tree derived from a sequence of numbers that satisfies:
1. **Min-heap property**: Each node's value is less than or equal to its children
2. **BST index property**: An inorder traversal yields elements in their original sequence order

## Prerequisites

- .NET 8.0 SDK or later
- Linux/macOS/Windows operating system

## How to Compile

Navigate to the project directory and build the project:

```bash
cd AdvancedAlgoAssignment1
dotnet build
```

## How to Run

### Run the Main Program

Execute the program with examples:

```bash
dotnet run
```

This will run several examples demonstrating:
- Standard Cartesian Tree construction
- Larger arrays
- Range Minimum Queries
- Edge cases (sorted and reverse-sorted arrays)

### Run Tests

Execute the test suite:

```bash
dotnet test
```

## Project Structure

```
AdvancedAlgoAssignment1/
├── CartesianTree.cs        # Main implementation
├── CartesianTreeTests.cs   # NUnit test suite
├── Program.cs              # Entry point
└── README.md               # This file
```

## Example Output

```
Example 1: Standard Cartesian Tree
Root: val=5, idx=0
    L--- val=10, idx=1
        L--- None
        R--- val=28, idx=4
            L--- val=30, idx=3
                L--- val=40, idx=2
...

Inorder traversal of the constructed tree :
5 10 40 30 28
```
