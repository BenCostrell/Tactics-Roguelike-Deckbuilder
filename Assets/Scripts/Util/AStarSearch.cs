using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class AStarSearch
{
    public static float Heuristic(MapTile a, MapTile b)
    {
        return Vector2.Distance(
            new Vector2(a.coord.x, a.coord.y),
            new Vector2(b.coord.x, b.coord.y));
    }

    public static List<MapTile> ShortestPath(MapTile start, MapTile goal, GridObject gridObject)
    {
        List<MapTile> path = new List<MapTile>();
        Dictionary<MapTile, MapTile> cameFrom = new Dictionary<MapTile, MapTile>();
        Dictionary<MapTile, float> costSoFar = new Dictionary<MapTile, float>();
        MapTile estimatedClosestMapTile = start;

        PriorityQueue<MapTile> frontier = new PriorityQueue<MapTile>();
        frontier.Enqueue(start, 0);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            MapTile current = frontier.Dequeue();
            if (Heuristic(current, goal) < Heuristic(estimatedClosestMapTile, goal))
            {
                estimatedClosestMapTile = current;
            }
            if (current == goal) break;
            foreach (MapTile next in current.neighbors)
            {
                if (gridObject.IsTilePassable(next))
                {
                    float newCost;
                    newCost = costSoFar[current] + 1;
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

        }
        MapTile pathNode = estimatedClosestMapTile;
        while (pathNode != start)
        {
            path.Add(pathNode);
            pathNode = cameFrom[pathNode];
        }

        path.Reverse();

        return path;
    }

    public static List<MapTile> FindAllAvailableGoals(MapTile start, int movesAvailable, 
        GridObject gridObject, bool raw = false, int minRange = 0)
    {
        List<MapTile> availableGoals = new List<MapTile>();
        if (movesAvailable == 0) return availableGoals;
        Dictionary<MapTile, MapTile> cameFrom = new Dictionary<MapTile, MapTile>();
        Dictionary<MapTile, int> costSoFar = new Dictionary<MapTile, int>();

        Queue<MapTile> frontier = new Queue<MapTile>();
        frontier.Enqueue(start);
        cameFrom[start] = start;
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            MapTile current = frontier.Dequeue();
            if (costSoFar[current] <= movesAvailable)
            {
                if (current != start && Coord.Distance(current.coord, start.coord) >= minRange)
                    availableGoals.Add(current);
                if (gridObject.IsTilePassable(current) || raw)
                {
                    foreach (MapTile next in current.neighbors)
                    {
                        if (gridObject.IsTilePassable(next) || raw)
                        {
                            int newCost;
                            newCost = costSoFar[current] + 1;

                            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                            {
                                costSoFar[next] = newCost;
                                frontier.Enqueue(next);
                                cameFrom[next] = current;
                            }
                        }
                    }
                }
            }
        }
        return availableGoals;
    }

}

public class PriorityQueue<T>
{
    public List<PrioritizedItem<T>> elements = new List<PrioritizedItem<T>>();

    public int Count { get { return elements.Count; } }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new PrioritizedItem<T>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].priority < elements[bestIndex].priority) bestIndex = i;
        }

        T bestItem = elements[bestIndex].item;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}

public class PrioritizedItem<T>
{
    public T item;
    public float priority;
    public PrioritizedItem(T item_, float priority_)
    {
        item = item_;
        priority = priority_;
    }
}
