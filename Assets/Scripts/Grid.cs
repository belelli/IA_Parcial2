using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] OldNode _prefab;
    [SerializeField] int _width, _height;
    [SerializeField] float _offSet;
    OldNode[,] _grid;


    // Start is called before the first frame update
    void Awake()
    {
        _grid = new OldNode[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(_prefab, new Vector3(x, 0, y) * _offSet, transform.rotation);
                node.Initialize(this, x, y);
                _grid[x, y] = node;
            }
        }
    }


    public OldNode GetNode(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height) return null;

        return _grid[x, y];
    }
    
    
    
    public OldNode GetClosestNode(Transform currentTransform)
    {
        OldNode closest = null;
        float closestDistance = float.MaxValue;
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = GetNode(x, y);
                if (Vector3.Distance(node.transform.position, currentTransform.position) < closestDistance)
                {
                    closest = node;
                    
                }

            }
        }
        
        return closest;
        
    }

}
