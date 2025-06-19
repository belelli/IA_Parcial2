using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
public Transform[] Waypoints; 
    public float Speed = 2f; 
    public float WaitTimeInEachWP = 1f; 

    private int _currentIndex = 0;
    private bool _isPatrolling = false;

    void Start()
    {
        
        StartCoroutine(MoveBetweenWPs());
    }

    IEnumerator MoveBetweenWPs()
    {
        _isPatrolling = true;

        while (_isPatrolling)
        {
            Transform nextWaypoint = Waypoints[_currentIndex];


            while (Vector3.Distance(transform.position, nextWaypoint.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextWaypoint.position, Speed * Time.deltaTime);
                yield return null; 
            }

            
            yield return new WaitForSeconds(WaitTimeInEachWP); //espoera en cada punto (opcional)

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
