using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    private List<GridObject> _containedObjects;
    public List<MapTile> _neighbors;
    public readonly Coord coord;
    public readonly TerrainData terrain;

    public MapTile(int x, int y, TerrainData terrain_)
    {
        coord = new Coord(x, y);
        terrain = terrain_;
        _containedObjects = new List<GridObject>();
        _neighbors = new List<MapTile>();
    }

    public void SetNeigbors(MapTile[,] map)
    {
        foreach(Coord direction in Coord.Directions)
        {
            Coord neighborCoord = coord.Add(direction);
            if(neighborCoord.ContainedInMap(map.GetLength(0), map.GetLength(1))){
                _neighbors.Add(map[neighborCoord.x, neighborCoord.y]);
            }
        }
    }
}
