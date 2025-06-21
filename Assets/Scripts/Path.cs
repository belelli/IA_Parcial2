using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public static Path instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Node> CalculateBFS(Node start,Node end)
    {

        var frontier = new Queue<Node>();
        frontier.Enqueue(start);
        
        var cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);
        //Hasta aca, tenemos una Queue de nodos, y otro Hashset de nodos. a ambos le agregamos el nodo Start (el primero)


        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if(current == end)
            {
                var path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();

                return path;
            }



            foreach (var item in current.Neighbors) //se pregunta para cada vecino del nodo Current
            {
                if (item.Blocked) continue;
                if (!cameFrom.ContainsKey(item))
                {
                    frontier.Enqueue(item);
                    cameFrom.Add(item, current);
                }
            }

        }

        return new List<Node>();

    }
}