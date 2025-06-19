using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    public static GridDebugger instance;
    public Pathfinding pathfinding;
    Node _start, _end;
    [SerializeField] Player _player;
    public List<Node> path;

    private void Awake()
    {
        instance = this;
    }

    public static Action pathReady;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            path = pathfinding.CalculateBFS(_start, _end);

            foreach (var item in path)
            {
                item.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            if (pathReady != null)
            {
                pathReady();
            }
        }
    }

    public void SetStart(Node node)
    {
        if(_start!= null)
        {
            _start.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        _start = node;
        _start.GetComponent<MeshRenderer>().material.color = Color.red;

        _player.SetInitialPosition(node);
        //_player.transform.position = new Vector3(node.transform.position.x, _player.transform.position.y, node.transform.position.z);
    }

    public void SetEnd(Node node)
    {
        if (_end != null)
        {
            _end.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        _end = node;
        _end.GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
