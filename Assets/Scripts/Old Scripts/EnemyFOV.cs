using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    [SerializeField] public Transform Target;
    [SerializeField] private float _detectionRadius;
    [SerializeField] float _detectionAngle;
    public Node NodeClosestToTarget;

    private List<Node> _currentPath; // Para almacenar el camino generado
    private Node _enemyCurrentNode; // El nodo actual del enemigo


    void Update()
    {
        if (FieldOfView(Target))
        {
            Debug.Log("ACA TAAAAA");
            NodeClosestToTarget = GridManager.instance.GetClosestNode(Target);
            NodeClosestToTarget.GetComponent<MeshRenderer>().material.color = Color.blue;
            //EnemyManager.OnPlayerDetected?.Invoke();
        }
        else
        {
            Debug.Log("NOPE");
        }

        if (FieldOfView(Target))
        {
            
        }
        
        
    }

    public bool FieldOfView(Transform target)
    {
        var distance = Vector3.Distance(transform.position, target.position);
        
        var dir = target.position - transform.position;
        if (distance <= _detectionRadius)
        {
            if (Vector3.Angle(transform.forward, dir) <= _detectionAngle *0.5f)
            {
                return GameManager.instance.IsInlineOfSight(transform.position, target.position);
            }
        }
        
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, Target.position);
    }
}
