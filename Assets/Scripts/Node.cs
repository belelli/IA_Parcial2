using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    
    int _x, _y;
    Grid _grid;

    List<Node> _neighbors = new List<Node>();
    
    public List <Node> Neighbors
    {
        get
        {
            if (_neighbors.Count > 0) return _neighbors;

            var left = _grid.GetNode(_x - 1, _y);
            if (left != null) 
                _neighbors.Add(left);

            var right = _grid.GetNode(_x + 1, _y);
            if (right != null) 
                _neighbors.Add(right);

            var down = _grid.GetNode(_x, _y - 1);
            if (down != null)  
                _neighbors.Add(down);

            var up = _grid.GetNode(_x, _y + 1);
            if (up != null)  
                _neighbors.Add(up);

            return _neighbors;
        }
    }

    public void Initialize(Grid grid, int x, int y)
    {
        _x = x;
        _y = y;
        _grid = grid;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("CLICL");
            GetComponent<MeshRenderer>().material.color = Color.green;
            foreach (var item in Neighbors)
            {
                item.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        

    }
}
