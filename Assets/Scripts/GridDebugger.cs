using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    public static GridDebugger instance;
    public Pathfinding pathfinding;
    OldNode _start, _end;
    [SerializeField] Player _player;
    public List<OldNode> path;

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

    public void SetStart(OldNode oldNode)
    {
        if(_start!= null)
        {
            _start.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        _start = oldNode;
        _start.GetComponent<MeshRenderer>().material.color = Color.red;

        _player.SetInitialPosition(oldNode);
        //_player.transform.position = new Vector3(node.transform.position.x, _player.transform.position.y, node.transform.position.z);
    }

    public void SetEnd(OldNode oldNode)
    {
        if (_end != null)
        {
            _end.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        _end = oldNode;
        _end.GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
