using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public MapTile[,] map { get; private set; }
    private Dictionary<int, GridObject> _gridObjects;
    private TerrainDataManager terrainDataManager;
    private GridObjectDataManager gridObjectDataManager;
    private MapDisplayer _mapDisplayer;

    public void InitializeMap(int width, int height)
    {
        InitializeTerrainData();
        map = CreateMap(width, height);
        InitializeGridObjectData();
        _gridObjects = new Dictionary<int, GridObject>();
        CreateGridObject(0, 0, GridObjectData.GridObjectType.PLAYER);
        Services.EventManager.Register<InputDown>(OnInputDown);
        _mapDisplayer = new MapDisplayer();
        _mapDisplayer.InitializeMapDisplay(map);
    }

    private void InitializeTerrainData()
    {
        terrainDataManager = new TerrainDataManager();
        //temporary, will ultimately load in from spreadsheet
        terrainDataManager.AddData(new TerrainData(
            TerrainData.TerrainType.GRASS,
            Resources.LoadAll<Sprite>("Sprites/overworld_tileset_grass")[0]));
    }

    private void InitializeGridObjectData()
    {
        gridObjectDataManager = new GridObjectDataManager();
        //temporary, will ultimately load in from spreadsheet
        gridObjectDataManager.AddData(new GridObjectData(
            GridObjectData.GridObjectType.PLAYER,
            Resources.Load<Sprite>("Sprites/helmetDude")));
    }

    private MapTile[,] CreateMap(int width, int height) {
        MapTile[,] map = new MapTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MapTile tile = new MapTile(x, y, terrainDataManager.terrainDataDict[TerrainData.TerrainType.GRASS]);
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
        GridObject gridObject = new GridObject(gridObjectDataManager.gridObjectDataDict[type]);
        _gridObjects[gridObject.id] = gridObject;
        gridObject.SpawnOnTile(map[x, y]);
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
