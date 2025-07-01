using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    public List<Node> AllNodes = new List<Node>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        AllNodes = GetComponentsInChildren<Node>().ToList();
    }

    private void Start()
    {
        AssignAllNeighbors();    
        
    }


    public List<Node> GetNeighbors(Node currentNode)
    {
        var neighbors = new List<Node>();
        foreach (var node in AllNodes)
        {
            if (node == currentNode) continue;
            //if(node._neighbors.Contains(currentNode)) continue;
            if (GameManager.instance.IsInlineOfSight(currentNode.transform.position, node.transform.position))
            {
                neighbors.Add(node);
            }
        }
        return neighbors;
    }

    void AssignAllNeighbors()
    {
        foreach (var node in AllNodes)
        {
            node.Neighbors = GetNeighbors(node);
        }
    }

    public Node GetClosestNode(Transform target)
    {
        Debug.Log("busco closest node");
        var minDistance = float.MaxValue;
        Node closestNode = null;
        foreach (var node in AllNodes)
        {
            var newDistance = Vector3.Distance(node.transform.position, target.position);
            if (newDistance < minDistance)
            {
                minDistance = newDistance;
                closestNode = node;
            }
        }
        //Debug.Log("en el metodo se encuentra " + closestNode.name);
        return closestNode;
    }

    


}
