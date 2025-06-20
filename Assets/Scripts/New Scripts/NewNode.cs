using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewNode : MonoBehaviour
{
    public List<NewNode> _neighbors = new List<NewNode>();

    private void Start()
    {
        //_neighbors = NewGrid.instance.GetNeighbors(this);
    }
}
