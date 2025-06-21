using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public List<OldNode> CalculateBFS(OldNode start,OldNode end)
    {
        var frontier = new Queue<OldNode>();
        frontier.Enqueue(start);
        
        var cameFrom = new Dictionary<OldNode, OldNode>();
        cameFrom.Add(start, null);
        //Hasta aca, tenemos una Queue de nodos, y otro Hashset de nodos. a ambos le agregamos el nodo Start (el primero)


        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if(current == end)
            {
                var path = new List<OldNode>();
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

        return new List<OldNode>();

    }
}
