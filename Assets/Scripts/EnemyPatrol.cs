using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] Waypoints; 
    public float Speed = 2f; 
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



    //[SerializeField] private Grid _grid;


    void Start()
    {
        StartCoroutine(CycleBetweenWPs());
        EnemyManager.OnPlayerDetected += EnemyDetectionAction;
        
    }

    IEnumerator CycleBetweenWPs()
    {
        _isPatrolling = true;

        while (_isPatrolling)
        {
            Transform currentWaypoint = Waypoints[_currentIndex];
            
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
   
                if (FieldOfView(Target))
                {
                    Debug.Log("ACA TAAAAA");
                    NodeClosestToTarget = GridManager.instance.GetClosestNode(Target);
                    NodeClosestToTarget.GetComponent<MeshRenderer>().material.color = Color.red;
                    EnemyManager.OnPlayerDetected?.Invoke();
                }
                
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, Speed * Time.deltaTime);
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
        StopAllCoroutines(); 
    }


    public void EnemyDetectionAction()
    {
        Target = EnemyManager.instance.PlayerTransform;
        Debug.Log("Enemy Detecton Action!");
        StopPatrolling();
        NodeClosestToMe = GridManager.instance.GetClosestNode(transform);
        NodeClosestToMe.GetComponent<MeshRenderer>().material.color = Color.blue;
        //
        path = Path.instance.CalculateBFS(NodeClosestToMe, NodeClosestToTarget);
        foreach (Node n in path)
        {
            n.GetComponent<MeshRenderer>().material.color = Color.blue;
            Debug.Log(n.name);
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
