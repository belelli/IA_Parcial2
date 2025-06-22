using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{   //PATRULLAJE
    public Transform[] Waypoints; 
    public float PatrolSpeed = 15f; 
    public float WaitTimeInEachWP = 0f; 

    private int _currentWaypointIndex = 0;
    private bool _isPatrolling = false;
    
    //FOV
    
    [SerializeField] public Transform PlayerTargetForFOV;
    [SerializeField] private float _detectionRadius;
    [SerializeField] float _detectionAngle;
    public Node NodeClosestToTarget;
    

    //PATH
    public float _walkToPlayerNodeSpeed = 20;
    [SerializeField] private List<Node> _pathToPlayer = new List<Node>();
    private bool _isWalkingToPlayerNode = false;
    
    private int _currentTargettedNodeIndex = 0;

    public Node NodeClosestToMe;
    //[SerializeField] private Grid _grid;


    void Start()
    {
        EnemyManager.OnPlayerDetected += EnemyDetectionAction;
        StartPatrollingState();
    }

    void StartPatrollingState()
    {
        _isPatrolling = true;
        _isWalkingToPlayerNode = false;
        StopAllCoroutines();
        StartCoroutine(CycleBetweenWPs());
        Debug.Log("Empieza la patrulla");
    }

    
    IEnumerator CycleBetweenWPs() //PATRULLAJE
    {
        //_isPatrolling = true;

        while (_isPatrolling)
        {
            Transform currentWaypoint = Waypoints[_currentWaypointIndex];
            
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
   
                if (FieldOfView(PlayerTargetForFOV))
                {
                    Debug.Log("FOV vio al player");
                    EnemyManager.instance.NotifyPlayerDetected(PlayerTargetForFOV);
                    yield break; //Aca corto corutina
                }
                
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, PatrolSpeed * Time.deltaTime);
                transform.forward = currentWaypoint.position - transform.position;
                yield return null; 
            }

            yield return new WaitForSeconds(WaitTimeInEachWP); //espoera en cada punto. por ahora en 0

            _currentWaypointIndex++;

            if (_currentWaypointIndex >= Waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }
        }
    }

    
    
    public void EnemyDetectionAction(Node detectedPlayerNode)
    // Cuando el enemigo detecta al player, se ejecuta esta funcion
    //para todas las corutinas
    //calcula el nodo mas cerano a mi.
    //arranca la corutina para caminar hasta el nodo del jugador
    {
        _isPatrolling = false;
        _isWalkingToPlayerNode = false;
        StopAllCoroutines();
        Debug.Log("el nodo mas cerca al player es " +detectedPlayerNode.name);
        detectedPlayerNode.GetComponent<MeshRenderer>().material.color = Color.blue; 
        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        _pathToPlayer = Path.instance.CalculateBFS(NodeClosestToMe, detectedPlayerNode);
        //Hasta aca joya

        StopAllCoroutines();
        StartCoroutine(WalkPathToPlayerNode(detectedPlayerNode));

    }



    IEnumerator WalkPathToPlayerNode(Node finalTargetNode)
    {
        _isWalkingToPlayerNode = true;
        _currentTargettedNodeIndex = 0;

        while (_isWalkingToPlayerNode && _currentTargettedNodeIndex < _pathToPlayer.Count)
        {
            Node currentTargettedNode = _pathToPlayer[_currentTargettedNodeIndex];

            Vector3 nodePosition = currentTargettedNode.transform.position;

            while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
            {

                //if (FieldOfView(PlayerTargetForFOV))
                //{
                //    EnemyManager.instance.NotifyPlayerDetected(PlayerTargetForFOV);

                //    yield break;
                //}

                transform.position = Vector3.MoveTowards(transform.position, nodePosition, _walkToPlayerNodeSpeed * Time.deltaTime);
                transform.forward = nodePosition - transform.position;
                
                yield return null;
            }
            
            Debug.Log("el enemigo " + gameObject.name + " llego al nodo " + currentTargettedNode.name);
            _currentTargettedNodeIndex++;
        }


        _isWalkingToPlayerNode = false;

        //bancar un segundo
        yield return new WaitForSeconds(1f);

        StartPatrollingState();


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
    }
    
    
}
