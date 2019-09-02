using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    public GridObject containedObject { get; private set; }
    public List<MapTile> neighbors { get; private set; }
    public readonly Coord coord;
    public readonly TerrainData terrain;

    public MapTile(int x, int y, TerrainData terrain_)
    {
        coord = new Coord(x, y);
        terrain = terrain_;
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
        containedObject = gridObject;
        if (gridObject.data.phylum == GridObjectData.Phylum.PLAYER && terrain.terrainName == "DOOR")
        {
            Services.EventManager.Fire(new LevelCompleted(SaveData.currentlyLoadedData.lastCompletedLevelNum + 1));
        }
    }

    public void OnObjectExit(GridObject gridObject)
    {
        containedObject = null;
    }
}
