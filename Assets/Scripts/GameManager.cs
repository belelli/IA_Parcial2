using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public LayerMask LayerMask;
    //public Transform PlayerTransform;

    public Transform EnemyTransform;
    // Start is called before the first frame update
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

    private void Update()
    {
        // if (IsInlineOfSight(EnemyTransform.position, PlayerTransform.position))
        // {
        //     Debug.Log("Esta In Line of sight");
        // }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(PlayerTransform.position, EnemyTransform.position);
    // }
    
}
