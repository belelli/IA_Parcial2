using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewGrid : MonoBehaviour
{
    public static NewGrid instance;
    public List<NewNode> AllNodes = new List<NewNode>();

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
        AllNodes = GetComponentsInChildren<NewNode>().ToList();
                       
        
        
        
    }

    private void Start()
    {
        AssignAllNeighbors();    
    }


    public List<NewNode> GetNeighbors(NewNode currentNode)
    {
        var neighbors = new List<NewNode>();
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
            node._neighbors = GetNeighbors(node);
            Debug.Log($"[NewGrid] Nodo {node.name} tiene {node._neighbors.Count} vecinos.");
        }
    }
    
    

    


}
