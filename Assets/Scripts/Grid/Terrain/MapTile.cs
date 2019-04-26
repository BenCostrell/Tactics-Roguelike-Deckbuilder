using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    public List<GridObject> containedObjects { get; private set; }
    public List<MapTile> neighbors { get; private set; }
    public readonly Coord coord;
    public readonly TerrainData terrain;

    public MapTile(int x, int y, TerrainData terrain_)
    {
        coord = new Coord(x, y);
        terrain = terrain_;
        containedObjects = new List<GridObject>();
        neighbors = new List<MapTile>();
    }

    public void SetNeigbors(MapTile[,] map)
    {
        foreach(Coord direction in Coord.Directions)
        {
            Coord neighborCoord = coord.Add(direction);
            if(neighborCoord.ContainedInMap(map.GetLength(0), map.GetLength(1))){
                neighbors.Add(map[neighborCoord.x, neighborCoord.y]);
            }
        }
    }

    public void OnObjectEnter(GridObject gridObject)
    {
        containedObjects.Add(gridObject);
    }

    public void OnObjectExit(GridObject gridObject)
    {
        containedObjects.Remove(gridObject);
    }
}
