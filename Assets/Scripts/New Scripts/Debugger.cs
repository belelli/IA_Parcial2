using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public NewNode A, B;
    public List<NewNode> Nodes = new List<NewNode>();
    public Path path;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Nodes = path.CalculateBFS(A, B);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < Nodes.Count-1; i++)
        {
            
            Gizmos.DrawLine(Nodes[i].transform.position, Nodes[i+1].transform.position);
        }
    }
}
