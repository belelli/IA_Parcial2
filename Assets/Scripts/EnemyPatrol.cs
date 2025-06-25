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
    [SerializeField] private bool _isPatrolling = false;
    
    //FOV
    
    [SerializeField] public Transform PlayerTargetForFOV;
    [SerializeField] private float _detectionRadius;
    [SerializeField] float _detectionAngle;
    public Node NodeClosestToTarget;
    

    //PATH
    public float _walkToPlayerNodeSpeed = 20;
    [SerializeField] private List<Node> _pathToPlayer = new List<Node>();
    [SerializeField] private List<Node> _pathToFirstWP = new List<Node>();
    [SerializeField] private bool _isWalkingToPlayerNode = false;
    
    private int _currentTargettedNodeIndex = 0;

    public Node NodeClosestToMe;
    //[SerializeField] private Grid _grid;


    //Chase
    [SerializeField] private bool _isChasingPlayer = false;
    [SerializeField] private bool _isReturningToPatrol = false; 
    private Coroutine _chaseCoroutine;

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
        StartCoroutine(PatrolBetweenWPs());
        Debug.Log("Empieza la patrulla");
    }

    
    IEnumerator PatrolBetweenWPs() //PATRULLAJE
    {
        while (_isPatrolling)
        {
            Transform currentWaypoint = Waypoints[_currentWaypointIndex];
            
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
   
                if (FieldOfView(PlayerTargetForFOV))
                {
                    Debug.Log("FOV vio al player");
                    EnemyManager.instance.NotifyPlayerDetected(PlayerTargetForFOV, this);
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



    public void EnemyDetectionAction(Node detectedPlayerNode, EnemyPatrol detector)
    // Cuando el enemigo detecta al player, se ejecuta esta funcion
    //calcula el nodo mas cerano a mi.
    //arranca la corutina para caminar hasta el nodo mas cercano al jugador
    {
        _isPatrolling = false;
        if(this == detector)
        {
            //if (_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
            _isChasingPlayer = true;
            _chaseCoroutine = StartCoroutine(ChasePlayer());
        }
        else
        {
            NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
            _pathToPlayer = Path.instance.Astar(NodeClosestToMe, detectedPlayerNode);
            StartCoroutine(WalkPathToPlayerNode(detectedPlayerNode));
        }



    }

    IEnumerator ChasePlayer()
    {
        while (_isChasingPlayer)
        {
            if (!FieldOfView(PlayerTargetForFOV))
            {
                // Perdió de vista al player, vuelve a patrullar
                _isChasingPlayer = false;
                StartPatrollingState();
                yield break;
            }

            Vector3 playerPos = PlayerTargetForFOV.position;
            transform.position = Vector3.MoveTowards(transform.position, playerPos, _walkToPlayerNodeSpeed * Time.deltaTime);
            transform.forward = playerPos - transform.position;
            yield return null;
        }
    }



    //IEnumerator WalkPathToPlayerNode(Node finalTargetNode) //Usa Pathfinding para ir al nodo mas cercano al player
    //{
    //    _isWalkingToPlayerNode = true;
    //    _currentTargettedNodeIndex = 0;

    //    while (_isWalkingToPlayerNode && _currentTargettedNodeIndex < _pathToPlayer.Count -1)
    //    {
    //        Node currentTargettedNode = _pathToPlayer[_currentTargettedNodeIndex];

    //        Vector3 nodePosition = currentTargettedNode.transform.position;

    //        while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
    //        {

    //            transform.position = Vector3.MoveTowards(transform.position, nodePosition, _walkToPlayerNodeSpeed * Time.deltaTime);
    //            transform.forward = nodePosition - transform.position;

    //            yield return null;
    //        }

    //        Debug.Log("el enemigo " + gameObject.name + " llego al nodo " + currentTargettedNode.name);
    //        _currentTargettedNodeIndex++;
    //    }


    //    _isWalkingToPlayerNode = false;

    //    //bancar un segundo
    //    yield return new WaitForSeconds(1f);

    //    // Si no ve al jugador, vuelve al primer waypoint usando pathfinding
    //    if (!FieldOfView(PlayerTargetForFOV))
    //    {
    //        yield return StartCoroutine(ReturnToFirstWaypoint());
    //    }



    //}
    IEnumerator WalkPathToPlayerNode(Node finalTargetNode) //Usa Pathfinding para ir al nodo mas cercano al player
    {
        _isWalkingToPlayerNode = true;
        _currentTargettedNodeIndex = 0;
        int index = 0;

        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        _pathToPlayer = Path.instance.Astar(NodeClosestToMe, finalTargetNode);

        while (_isWalkingToPlayerNode && index < _pathToPlayer.Count)
        {
            Node currentTargettedNode = _pathToPlayer[index];

            Vector3 nodePosition = currentTargettedNode.transform.position;

            while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
            {

                transform.position = Vector3.MoveTowards(transform.position, nodePosition, _walkToPlayerNodeSpeed * Time.deltaTime);
                transform.forward = nodePosition - transform.position;

                yield return null;
            }

            
            index++;
        }


        _isWalkingToPlayerNode = false;

        //bancar un segundo
        yield return new WaitForSeconds(1f);

        // Si no ve al jugador, vuelve al primer waypoint usando pathfinding
        if (!FieldOfView(PlayerTargetForFOV))
        {
            yield return StartCoroutine(ReturnToFirstWaypoint());
        }



    }

    private IEnumerator ReturnToFirstWaypoint()
    {
        _isReturningToPatrol = true;
        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        Node firstWaypointNode = GridManager.instance.GetClosestNode(Waypoints[0]);
        _pathToFirstWP = Path.instance.Astar(NodeClosestToMe, firstWaypointNode);

        //_isWalkingToPlayerNode = true;
        //_currentTargettedNodeIndex = 0;
        var index = 0;


        while (_isReturningToPatrol && index < _pathToFirstWP.Count)
        {
            Node currentTargettedNode = _pathToFirstWP[index];
            Vector3 nodePosition = currentTargettedNode.transform.position;

            while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nodePosition, PatrolSpeed * Time.deltaTime);
                transform.forward = nodePosition - transform.position;
                yield return null;
            }
            index++;
        }

        _isReturningToPatrol = false;
        _currentWaypointIndex = 0; // Empieza patrulla desde el primer waypoint
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
