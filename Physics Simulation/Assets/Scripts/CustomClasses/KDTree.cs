using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Node
{
    public float[] coordinates;
    public Node left, right;
    public Node[] minNodes;
    public Node[] maxNodes;
    public Transform transform;
    private int k;

    public Node(float[] coordinates, Transform t)
    {
        this.coordinates = coordinates;
        this.k = coordinates.Length;
        this.transform = t;
        this.right = this.left = null;
        minNodes = new Node[this.k];
        maxNodes = new Node[this.k];
        for(int i = 0; i < this.k; i++) minNodes[i] = maxNodes[i] = this;
    }
}

public class KDTree
{
    private Node root = null;
    private int k;
    private (Node, decimal) nearest = (null, decimal.MaxValue);
    private Dictionary<Transform, (float, float)> lookup;
    

    //constructors
    public KDTree (int k)
    {
        this.k = k;
    }

    //public methods:
    public void Insert (Transform t)
    {
        float[] coords = new float[2];
        coords[0] = t.position.x;
        coords[1] = t.position.y;
        lookup.Add(t, (coords[0], coords[1]));
        root = Insert(root, 0, coords, t);
    }

    public void Delete (Transform t)
    {
        lookup.Remove(t);
        float[] coords = new float[2];
        (float, float) val = lookup[t];
        coords[0] = val.Item1;
        coords[1] = val.Item2;
        root = Delete(root, t, coords, 0);
    }

    public void Replace (Transform t)
    {
        Delete(t);
        Insert(t);
    }

    public Transform NearestNeighbour (Vector2 t)
    {

        nearest = (null, decimal.MaxValue);
        float[] coords = new float[2];
        coords[0] = t.x;
        coords[1] = t.y;
        NNSearch_Helper(root, coords, 0, true);
        return nearest.Item1 == null ? null : nearest.Item1.transform;
    }

    //private methods:
    private Node Insert (Node curr, int depth, float [] coordinates, Transform t)
    {
        if (curr == null) return new Node(coordinates, t);
        int cd = depth % k;
        if (curr.coordinates[cd] > coordinates[cd])
        {
            curr.left = Insert(curr.left, depth + 1, coordinates, t);
        }
        else
        {
            curr.right = Insert(curr.right, depth + 1, coordinates, t);
        }
        computeMinMax(curr);
        return curr;
    }

    private Node Delete (Node curr, Transform t, float [] coordinates, int depth)
    {
        if (curr == null) return null;
        int cd = depth % k;
        if (Equal (curr.coordinates, coordinates) && curr.transform == t)
        {
            if (curr.right != null)
            {
                Node minNode = findMin(curr.right, cd);
                Swap(curr, minNode);
                curr.right = Delete(curr.right, t, coordinates, depth + 1);
            }
            else if (curr.left != null)
            {
                Node maxNode = findMax(curr.left, cd);
                Swap(curr, maxNode);
                curr.left = Delete(curr.left, t, coordinates, depth + 1);
            }
            else
            {
                return null;
            }
        }
        else if (curr.coordinates [cd] > coordinates [cd])
        {
            curr.left = Delete(curr.left, t, coordinates, depth + 1);
        }
        else
        {
            curr.right = Delete(curr.right, t, coordinates, depth + 1);
        }
        computeMinMax(curr);
        return curr;
    }

    private void computeMinMax (Node curr)
    {
        if (curr == null || (curr.left == null && curr.right == null)) return;
        if (curr.left == null) TakeMinMaxSingleChild(curr, curr.right);
        else if (curr.right == null) TakeMinMaxSingleChild(curr, curr.left);
        else TakeMinMaxBothChildren(curr);
    }

    private void TakeMinMaxSingleChild (Node curr, Node second)
    {
        for (int i = 0; i < k; i++)
        {
            curr.minNodes[i] = second.minNodes[i].coordinates[i] < curr.coordinates[i] ? second.minNodes[i] : curr;
            curr.maxNodes[i] = second.maxNodes[i].coordinates[i] > curr.coordinates[i] ? second.maxNodes[i] : curr;
        }
    }

    private void TakeMinMaxBothChildren (Node curr)
    {
        for (int i = 0; i < k; i++)
        {
            curr.minNodes[i] = GetMinNode(curr, GetMinNode(curr.left.minNodes[i], curr.right.minNodes[i], i), i);
            curr.maxNodes[i] = GetMaxNode(curr, GetMaxNode(curr.left.maxNodes[i], curr.right.maxNodes[i], i), i);
        }
    }

    private Node GetMinNode (Node n1, Node n2, int cd)
    {
        return n1.coordinates[cd] < n2.coordinates[cd] ? n1 : n2;
    }

    private Node GetMaxNode (Node n1, Node n2, int cd)
    {
        return n1.coordinates[cd] > n2.coordinates[cd] ? n2 : n1;
    }
    

    private Node findMin (Node curr, int cd)
    {
        return curr.minNodes[cd];
    }
        
    private Node findMax (Node curr, int cd)
    {
        return curr.maxNodes[cd];
    }

    private void Swap (Node n1, Node n2)
    {
        for (int i = 0; i < k; i++)
        {
            float temp = n1.coordinates[i];
            n1.coordinates[i] = n2.coordinates[i];
            n2.coordinates[i] = temp;
        }
        Transform temp2 = n1.transform;
        n1.transform = n2.transform;
        n2.transform = temp2;
    }

    private bool Equal (float [] arr1, float [] arr2)
    {
        for (int i = 0; i < k; i++)
        {
            if (!(Mathf.Abs (arr1[i] - arr2[i]) < Util.EPSILON)) return false;
        }
        return true;
    }

    private decimal CalcDis (float [] p1, float [] p2)
    {
        decimal res = 0;
        for (int i = 0; i < p1.Length; i++)
        {
            decimal temp = (decimal)(p1[i] - p2[i]);
            res += temp * temp;
        }
        return res;
    }

    private void NNSearch_Helper (Node curr, float [] target, int depth, bool task) //task: true: go to where you are, false: go back and stuff
    {
        if (curr == null) return;
        int cd = depth % k;

        decimal dis = CalcDis(target, curr.coordinates);
        if (dis < nearest.Item2) nearest = (curr, dis);

        (bool, bool) visited = (false, false);

        if (task)
        {
            if (target [cd] < curr.coordinates[cd])
            {
                NNSearch_Helper(curr.left, target, depth + 1, true);
                visited.Item1 = true;
            }
            else
            {
                NNSearch_Helper(curr.right, target, depth + 1, true);
                visited.Item2 = true;
            }
        }

        //go back always the same.
        decimal diff = (decimal)(target[cd] - curr.coordinates[cd]);
        if (diff * diff < nearest.Item2)
        {
            if (!visited.Item1) NNSearch_Helper(curr.left, target, depth + 1, false);
            if (!visited.Item2) NNSearch_Helper(curr.right, target, depth + 1, false);
        }
    }
}
