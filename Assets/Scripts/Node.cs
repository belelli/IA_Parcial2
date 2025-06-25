using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> Neighbors = new List<Node>();
    public bool Blocked;
    public float Cost = 1f;
    
    
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 3)
    //    {
    //        Blocked = true;
    //        GetComponent<MeshRenderer>().material.color = Blocked ? Color.black : Color.white;
    //    }
    //}

}
