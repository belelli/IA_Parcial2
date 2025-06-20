using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewNode : MonoBehaviour
{
    public List<NewNode> Neighbors = new List<NewNode>();
    public bool Blocked;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            Blocked = true;
            GetComponent<MeshRenderer>().material.color = Blocked ? Color.black : Color.white;
        }
    }
    private void OnDrawGizmos()
    {   
        Gizmos.color = Color.green;
        foreach (NewNode neighbor in Neighbors)
        {
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
            //Gizmos.DrawSphere(transform.position, 0.1f);
        }
        
    }
}
