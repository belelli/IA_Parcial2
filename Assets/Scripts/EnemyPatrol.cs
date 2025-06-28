using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{


    //PATRULLAJE
    public Transform[] Waypoints;
    public float PatrolSpeed = 15f;
    public float WaitTimeInEachWP = 0f;

    private int _currentWaypointIndex = 0;
    [SerializeField] private bool _isPatrolling = false;

    //FOV

    [SerializeField] public Transform PlayerTargetForFOV;
    [SerializeField] private float _detectionRadius;
    [SerializeField] float _detectionAngle;
    


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

    //corutinas
    private Coroutine _returnToPatrolCoroutine;
    private Coroutine _chaseCoroutine;

    Coroutine _CoroutineActive;




    void Start()
    {
        EnemyManager.OnPlayerDetected += EnemyDetectionAction;
        StartPatrollingState();
    }

    void StartPatrollingState()
    {
        _isPatrolling = true;
        _isWalkingToPlayerNode = false;
        //StopAllCoroutines();
        Debug.Log("Empieza la patrulla" + this.name);

        if (_CoroutineActive != null)
            StopCoroutine(_CoroutineActive);

        _CoroutineActive = StartCoroutine(PatrolBetweenWPs());
    }


    IEnumerator PatrolBetweenWPs() //PATRULLAJE
    {   Debug.Log(this.name+" empieza Corrutina PATROL ");
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
        if (this == detector)
        {
            //if (_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
            _isChasingPlayer = true;
            if (_CoroutineActive != null)
                StopCoroutine(_CoroutineActive);
            _CoroutineActive = StartCoroutine(ChasePlayer());
        }
        else if (!_isWalkingToPlayerNode)
        {
            Debug.Log("Calculo path");
            NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
            _pathToPlayer = Path.instance.Astar(NodeClosestToMe, detectedPlayerNode);

            if (_CoroutineActive != null)
                StopCoroutine(_CoroutineActive);

            _CoroutineActive = StartCoroutine(WalkPathToPlayerNode(detectedPlayerNode));
        }



    }

    IEnumerator ChasePlayer()
    {
        Debug.Log(this.name + " empieza Corrutina CHASE ");
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




    IEnumerator WalkPathToPlayerNode(Node finalTargetNode) //Usa Pathfinding para ir al nodo mas cercano al player
    {
        Debug.Log(this.name + " empieza Corrutina WalkPathToPlayerNode ");
        _isWalkingToPlayerNode = true;
        _currentTargettedNodeIndex = 0;
        int index = 0;

        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);

        _pathToPlayer = Path.instance.Astar(NodeClosestToMe, finalTargetNode);

        while (_isWalkingToPlayerNode && index < _pathToPlayer.Count)
        {
            Node currentTargettedNode = _pathToPlayer[index];

            Vector3 nodePosition = currentTargettedNode.transform.position;

            var t = 0f;
            Vector3 initialPos = transform.position;
            while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
            {
                t += _walkToPlayerNodeSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(initialPos, nodePosition, t);
                transform.forward = nodePosition - transform.position;

                yield return null;
            }


            index++;
        }

        _pathToPlayer.Clear();
        _isWalkingToPlayerNode = false;

        //bancar un segundo
        yield return new WaitForSeconds(1f);

        // Si no ve al jugador, vuelve al primer waypoint usando pathfinding
        if (!FieldOfView(PlayerTargetForFOV))
        {
            //
            if (_isReturningToPatrol)
            {
                //Debug.Log($"{gameObject.name} - Ya está retornando a patrulla, ignora nueva llamada.");
                //yield break;
            }

            if (_CoroutineActive != null)
                StopCoroutine(_CoroutineActive);

            _CoroutineActive = StartCoroutine(ReturnToFirstWaypoint());
            //yield return StartCoroutine(ReturnToFirstWaypoint());
        }



    }

    private IEnumerator ReturnToFirstWaypoint()
    {
        Debug.Log(this.name + " empieza Corrutina ReturnToFirstWaypoint ");
        _isWalkingToPlayerNode = false;
        _isReturningToPatrol = true;
        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        Node firstWaypointNode = GridManager.instance.GetClosestNode(Waypoints[0]);
        _pathToFirstWP = Path.instance.Astar(NodeClosestToMe, firstWaypointNode);

        //_isWalkingToPlayerNode = true;
        //_currentTargettedNodeIndex = 0;
        var index = 0;


        while (_isReturningToPatrol && index < _pathToFirstWP.Count)
        {
            //Debug.Log("el path de " + this.name + "tiene " + _pathToFirstWP.Count + " nodos");
            //Debug.Log("El index de return to patrol de " + this.name + "es " + index);
            Node currentTargettedNode = _pathToFirstWP[index];
            Vector3 nodePosition = currentTargettedNode.transform.position;
            Vector3 initialPos = transform.position;
            var t = 0f;

            while (Vector3.Distance(transform.position, nodePosition) > 0.3f)
            {
                t += PatrolSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(initialPos, nodePosition, t);
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
            if (Vector3.Angle(transform.forward, dir) <= _detectionAngle * 0.5f)
            {
                return GameManager.instance.IsInlineOfSight(transform.position, target.position);
            }
        }

        return false;
    }



}
