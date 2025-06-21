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
    
    

    


}
