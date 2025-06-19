using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [SerializeField] float _speed;
    Pathfinding _path;
    public bool walking = false;
    public Node currentNodeDestination;
    public int currentIndex = 0;
    float _minDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        GridDebugger.pathReady += StartTrail;
    }

    // Update is called once per frame
    void Update()
    {
        if (walking)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentNodeDestination.transform.position, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, currentNodeDestination.transform.position) < _minDistance)
            {
                
                currentIndex++;
                if(currentIndex > GridDebugger.instance.path.Count)
                {
                    currentIndex = 0;
                    walking = false;
                }

                currentNodeDestination = GridDebugger.instance.path[currentIndex];

            }
        }
    }

    public void StartTrail()
    {
        walking = true;
        Debug.Log("start trail");
        currentIndex = 0;
        currentNodeDestination = GridDebugger.instance.path[currentIndex];
        Debug.Log("tiene" + GridDebugger.instance.path.Count);
        
    }

    public void SetInitialPosition(Node node)
    {
        transform.position = new Vector3(node.transform.position.x, transform.position.y, node.transform.position.z);
    }






}
