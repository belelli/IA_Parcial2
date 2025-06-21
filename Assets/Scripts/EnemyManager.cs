using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<EnemyPatrol> Enemies = new List<EnemyPatrol>();
    public static Action OnPlayerDetected;
    public Transform PlayerTransform;
    public Node ClosestNodeToPlayer;

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
        OnPlayerDetected += UpdateClosestNodeToPlayer;

    }
    
    void UpdateClosestNodeToPlayer()
    {
        Debug.Log("actualizo nodo mas cercano al jugador desde el manager");
        ClosestNodeToPlayer = GridManager.instance.GetClosestNode(PlayerTransform);
    }
    
    
}
