using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
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


    public List<Node> Dijkstra(Node start, Node end)
    {

        //var frontier = new Queue<Node>();
        var frontier = new PriorityQueue();
        //frontier.Enqueue(start);
        frontier.Enqueue(start, 0); // nodo Start a la Queue / costo de 0


        var cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);
        
        var costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0f);



        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == end)
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

                float newCost = costSoFar[current] + item.Cost; //costo del nodo actual + costo del vecino
               



                if (!costSoFar.ContainsKey(item))
                {
                    frontier.Enqueue(item, newCost);
                    costSoFar.Add(item, newCost);
                    cameFrom.Add(item, current);
                }
                else
                {
                    if (newCost < costSoFar[item])
                    {
                        costSoFar[item] = newCost;
                        frontier.Enqueue(item, newCost);
                        cameFrom[item] = current;
                    }
                }
            }

        }

        return new List<Node>();

    }

    public List<Node> Astar(Node start, Node end)
    {

        //var frontier = new Queue<Node>();
        var frontier = new PriorityQueue();
        //frontier.Enqueue(start);
        frontier.Enqueue(start, 0); // nodo Start a la Queue / costo de 0


        var cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);

        var costSoFar = new Dictionary<Node, float>();
        costSoFar.Add(start, 0f);



        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == end)
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

                float newCost = costSoFar[current] + item.Cost; //costo del nodo actual + costo del vecino
                float priority = newCost + Vector3.Distance(item.transform.position, end.transform.position); //prioridad de la Queue, suma del costo y la distancia al nodo final



                if (!costSoFar.ContainsKey(item))
                {
                    costSoFar.Add(item, newCost);
                    frontier.Enqueue(item, priority);
                    cameFrom.Add(item, current);
                }
                else
                {
                    if (newCost < costSoFar[item])
                    {
                        costSoFar[item] = newCost;
                        frontier.Enqueue(item, priority);
                        cameFrom[item] = current;
                    }
                }
            }

        }

        return new List<Node>();

    }


}