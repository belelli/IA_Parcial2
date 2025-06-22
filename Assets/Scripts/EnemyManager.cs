using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<EnemyPatrol> Enemies = new List<EnemyPatrol>();
    public static Action<Node, EnemyPatrol> OnPlayerDetected;
    public Transform PlayerTransform;

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
        //OnPlayerDetected += UpdateClosestNodeToPlayer;

    }
    


    public void NotifyPlayerDetected(Transform playerDetectedTransform, EnemyPatrol detector)
    {
        Node detectedPlayerNode = GridManager.instance.GetClosestNode(playerDetectedTransform);
        if (detectedPlayerNode != null)
        {
            OnPlayerDetected?.Invoke(detectedPlayerNode, detector);
        }
    }
    
    
}
