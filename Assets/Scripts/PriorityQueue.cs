using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue 
{
    Dictionary<Node, float> _allNodes = new Dictionary<Node, float>();

    public int Count
    {
        get { return _allNodes.Count; }
    }
    public void Enqueue(Node node, float cost)
    {
        if (_allNodes.ContainsKey(node))
        {
            _allNodes[node] = cost;
        }
        else
        {
            _allNodes.Add(node, cost);
        }
    }

    public Node Dequeue()
    {
        if (_allNodes.Count == 0)
            return null;

        Node minCostNode = null;
        float minCost = int.MaxValue;
        foreach (var node in _allNodes)
        {
            if (node.Value < minCost)
            {
                minCost = node.Value;
                minCostNode = node.Key;
            }
        }
        _allNodes.Remove(minCostNode);
        return minCostNode;
    }
}
