using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public LayerMask LayerMask;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsInlineOfSight(Vector3 start, Vector3 end)
    {
        
        var dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, LayerMask);
    }


}
