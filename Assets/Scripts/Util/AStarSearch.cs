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
        GridObject gridObject, bool raw = false, int minRange = 0, bool ignoreEnemies = false)
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
                if (gridObject.IsTilePassable(current, raw, ignoreEnemies))
                {
                    foreach (MapTile next in current.neighbors)
                    {
                        if (gridObject.IsTilePassable(next, raw, ignoreEnemies))
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


    public static List<List<MapTile>> GetAllDirectPaths(MapTile start, MapTile target)
    {
        List<List<MapTile>> paths = new List<List<MapTile>>();
        Coord diff = target.coord.Subtract(start.coord);
        MapTile horizontalNeighbor = null;
        MapTile verticalNeighbor = null;
        //Debug.Log("finding path from " + start.coord + " to " + target.coord);
        Dictionary<MapTile, List<List<MapTile>>> pathsFromTiles = 
            new Dictionary<MapTile, List<List<MapTile>>>();
        foreach (MapTile neighbor in start.neighbors)
        {
            int xDiff = neighbor.coord.Subtract(start.coord).x;
            int yDiff = neighbor.coord.Subtract(start.coord).y;
            if (xDiff != 0 && diff.x != 0 && Mathf.Sign(xDiff) == Mathf.Sign(diff.x))
            {
                horizontalNeighbor = neighbor;
            }
            if (yDiff != 0 && diff.y != 0 && Mathf.Sign(yDiff) == Mathf.Sign(diff.y))
            {
                verticalNeighbor = neighbor;
            }
        }
        if (horizontalNeighbor == target || verticalNeighbor == target)
        {
            paths.Add(new List<MapTile>() { start, target });
            return paths;
        }
        List<MapTile> promisingNeighbors = new List<MapTile>();
        if (horizontalNeighbor != null)
        {
            promisingNeighbors.Add(horizontalNeighbor);
        }
        if (verticalNeighbor != null)
        {
            promisingNeighbors.Add(verticalNeighbor);
        }

        foreach (MapTile promisingNeighbor in promisingNeighbors)
        {
            List<List<MapTile>> pathsFromNeighbor;
            if (!pathsFromTiles.ContainsKey(promisingNeighbor))
            {
                pathsFromTiles[promisingNeighbor] = GetAllDirectPaths(promisingNeighbor, target);
            }
            pathsFromNeighbor = pathsFromTiles[promisingNeighbor];

            foreach (List<MapTile> path in pathsFromNeighbor)
            {
                List<MapTile> newPath = new List<MapTile>(path);
                newPath.Insert(0, start);
                paths.Add(newPath);
            }
        }
        return paths;
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
