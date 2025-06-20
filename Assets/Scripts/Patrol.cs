using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Patrol : MonoBehaviour
{
    public Transform[] Waypoints; 
    public float Speed = 2f; 
    public float WaitTimeInEachWP = 1f; 

    private int _currentIndex = 0;
    private bool _isPatrolling = false;
    
    public EnemyFOV Fov;

    [SerializeField] private Grid _grid;

    void Start()
    {
        StartCoroutine(MoveBetweenWPs());
    }

    IEnumerator MoveBetweenWPs()
    {
        _isPatrolling = true;

        while (_isPatrolling)
        {
            Transform currentWaypoint = Waypoints[_currentIndex];
            Fov.Target = currentWaypoint;
            
            while (Vector3.Distance(transform.position, currentWaypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, Speed * Time.deltaTime);
                transform.forward = currentWaypoint.position - transform.position;
                bool WaypointIsInFOV = Fov.FieldOfView(currentWaypoint);
                Debug.Log("el current WP "+ _currentIndex+" "+WaypointIsInFOV);
                //var closestNode = _grid.GetClosestNode(transform);
                //closestNode.GetComponent<MeshRenderer>().material.color = Color.red;
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
}
