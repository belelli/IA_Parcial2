using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] Waypoints; 
    public float PatrolSpeed = 2f; 
    public float WaitTimeInEachWP = 1f; 

    private int _currentIndex = 0;
    private bool _isPatrolling = false;
    
    //public EnemyFOV Fov;
    
    [SerializeField] public Transform Target;
    [SerializeField] private float _detectionRadius;
    [SerializeField] float _detectionAngle;
    public Node NodeClosestToTarget;
    public Node NodeClosestToMe;
    [SerializeField] List<Node> path = new List<Node>();

    //
    private bool _isWalkingToPlayerNode = false;
    public float _walkToPlayerNodeSpeed;
    private int _currentPathIndex = 0;


    //[SerializeField] private Grid _grid;


    void Start()
    {
        StartPatrolling();
        EnemyManager.OnPlayerDetected += EnemyDetectionAction;
        
    }

    void StartPatrolling()
    {
        _isPatrolling = true;
        StopAllCoroutines();
        StartCoroutine(CycleBetweenWPs());
    }

    IEnumerator CycleBetweenWPs()
    {
        //_isPatrolling = true;

        while (_isPatrolling)
        {
            Transform currentWaypoint = Waypoints[_currentIndex];
            
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
   
                if (FieldOfView(Target))
                {
                    Debug.Log("FOV vio al player");
                    EnemyManager.OnPlayerDetected?.Invoke();
                    yield break;
                }
                
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, PatrolSpeed * Time.deltaTime);
                transform.forward = currentWaypoint.position - transform.position;
                //bool WaypointIsInFOV = Fov.FieldOfView(currentWaypoint);
                yield return null; 
            }

            yield return new WaitForSeconds(WaitTimeInEachWP); //espoera en cada punto. por ahora en 0

            _currentIndex++;

            if (_currentIndex >= Waypoints.Length)
            {
                _currentIndex = 0;
            }
        }
    }


    public void StopPatrolling()
    {
        _isPatrolling = false;
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

    public void EnemyDetectionAction()
    {
        StopPatrolling();
        PathToPlayerSetup();
        StartCoroutine(WalkPathToPlayerNode());
    }

    IEnumerator WalkPathToPlayerNode()
    {
        _isWalkingToPlayerNode = true;
        _currentPathIndex = 0;

        while (_isWalkingToPlayerNode && _currentPathIndex < path.Count)
        {
            var currentNodeInPath = path[_currentPathIndex];

            Vector3 nodePosition = currentNodeInPath.transform.position;

            while (Vector3.Distance(transform.position, nodePosition) > 0.1f)
            {

                if (FieldOfView(EnemyManager.instance.PlayerTransform))
                {
                    //_isWalkingToPlayerNode = false;
                    //Debug.Log("FOV vio al player mientras caminaba a su nodo");
                    //EnemyManager.OnPlayerDetected?.Invoke();
                    //_isWalkingToPlayerNode = false;

                    yield break;
                }

                transform.position = Vector3.MoveTowards(transform.position, nodePosition, _walkToPlayerNodeSpeed * Time.deltaTime);
                transform.forward = nodePosition - transform.position;
                //bool WaypointIsInFOV = Fov.FieldOfView(currentWaypoint);
                yield return null;
            }
            //Aca llegaa al currentNodeinOPath

            _currentPathIndex++;


                
        }


        _isWalkingToPlayerNode = false;


        
    }

    private void PathToPlayerSetup()
    {
        //Path Setup
        Target = EnemyManager.instance.PlayerTransform;
        NodeClosestToTarget = EnemyManager.instance.ClosestNodeToPlayer;
        Debug.Log("Enemy Detecton Action!");
       
        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        NodeClosestToMe.GetComponent<MeshRenderer>().material.color = Color.blue;
        path = Path.instance.CalculateBFS(NodeClosestToMe, NodeClosestToTarget);
        foreach (Node n in path)
        {
            n.GetComponent<MeshRenderer>().material.color = Color.blue;
            Debug.Log(n.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, Target.position);
    }
    
    
}
