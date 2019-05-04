using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public MapTile[,] map { get; private set; }
    private Dictionary<int, GridObject> _gridObjects;
    private MapDisplayer _mapDisplayer;

    public void Update()
    {

    }

    public void InitializeMap(int width, int height)
    {
        map = CreateMap(width, height);
        _gridObjects = new Dictionary<int, GridObject>();
        CreateGridObject(0, 0, new Player());
        SpawnEnemies();
        Services.EventManager.Register<InputDown>(OnInputDown);
        _mapDisplayer = new MapDisplayer();
        _mapDisplayer.InitializeMapDisplay(map);
    }

    private MapTile[,] CreateMap(int width, int height) {
        MapTile[,] map = new MapTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MapTile tile = new MapTile(x, y, 
                    Services.TerrainDataManager.terrainDataDict[TerrainData.TerrainType.GRASS]);
                map[x, y] = tile;
            }
        }
        foreach(MapTile tile in map)
        {
            tile.SetNeigbors(map);
        }
        return map;
    }

    private void CreateGridObject(int x, int y, GridObjectData.GridObjectType type)
    {
        GridObject gridObject = new GridObject(Services.GridObjectDataManager.GetData(type));
        CreateGridObject(x, y, gridObject);
    }

    private void CreateGridObject(int x, int y, GridObject gridObject)
    {
        _gridObjects[gridObject.id] = gridObject;
        gridObject.SpawnOnTile(map[x, y]);
    }

    private void SpawnEnemies()
    {
        int numEnemies = 2;
        while(numEnemies > 0)
        {
            int x = Random.Range(0, map.GetLength(0));
            int y = Random.Range(0, map.GetLength(1));
            if(map[x,y].containedObjects.Count == 0)
            {
                CreateGridObject(x, y, GridObjectData.GridObjectType.GOBLIN);
                numEnemies -= 1;
            }
        }
    }

    private void OnInputDown(InputDown e)
    {
        Vector2 mouseLocalPos = _mapDisplayer.mapHolder.InverseTransformPoint(e.worldPos);
        Coord coord = new Coord(Mathf.RoundToInt(mouseLocalPos.x), Mathf.RoundToInt(mouseLocalPos.y));
        if (IsCoordInMap(coord))
        {
            Services.EventManager.Fire(new MapTileSelected(map[coord.x, coord.y]));
        }
    }

    private bool IsCoordInMap(Coord coord)
    {
        return coord.x >= 0 && coord.x < map.GetLength(0) && coord.y >= 0 && coord.y < map.GetLength(1);
    }
}

public class MapTileSelected : GameEvent
{
    public readonly MapTile mapTile;

    public MapTileSelected(MapTile mapTile_)
    {
        mapTile = mapTile_;
    }
}
