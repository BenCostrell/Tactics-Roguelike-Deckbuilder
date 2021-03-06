﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public MapTile[,] map { get; private set; }
    private Dictionary<int, GridObject> _gridObjects;
    public List<GridObject> gridObjectList
    {
        get
        {
            return new List<GridObject>(_gridObjects.Values);
        }
    }
    private MapDisplayer _mapDisplayer;
    private EnemyTurnManager _enemyTurnManager;

    public MapManager()
    {
        InitializeMap(SaveData.currentlyLoadedData.currentLevel.size.x, 
            SaveData.currentlyLoadedData.currentLevel.size.y);
        _enemyTurnManager = new EnemyTurnManager();
        Services.EventManager.Register<PlayerTurnEnded>(OnPlayerTurnEnd);
        Services.EventManager.Register<GridObjectDeath>(OnGridObjectDeath);
    }

    public void Update()
    {
        _mapDisplayer.Update();
        _enemyTurnManager.Update();
    }

    public void InitializeMap(int width, int height)
    {
        map = CreateMap(width, height);
        _gridObjects = new Dictionary<int, GridObject>();
        CreateGridObject(0, 0, new Player());

        bool farCornerOccupied = false;
        foreach (GridObjectSpawnData rewardSpawn in SaveData.currentlyLoadedData.currentLevel.rewardSpawns)
        {
            int dist = ((map.GetLength(0) + map.GetLength(1)) / 2) - 1;
            //for now always spawn chest
            for (int i = 0; i < rewardSpawn.numToSpawn; i++)
            {
                //SpawnChest(dist, dist);

                // just put em in the corners?
                if (!farCornerOccupied)
                {
                    farCornerOccupied = true;
                    CreateGridObject(width - 1, 0, rewardSpawn.gridObjectData.gridObjectName);
                }
                else
                {
                    CreateGridObject(0, height - 1, rewardSpawn.gridObjectData.gridObjectName);
                }
            }
            
        }
        foreach (GridObjectSpawnData enemySpawn in SaveData.currentlyLoadedData.currentLevel.enemySpawns)
        {
            SpawnRandomObjects(enemySpawn.gridObjectData.gridObjectName, enemySpawn.numToSpawn, 2);
        }
        foreach (GridObjectSpawnData vegetationSpawn in SaveData.currentlyLoadedData.currentLevel.vegetationSpawns)
        {
            SpawnRandomObjects(vegetationSpawn.gridObjectData.gridObjectName, vegetationSpawn.numToSpawn);
        }
        //Services.EventManager.Register<InputDown>(OnInputDown);
        _mapDisplayer = new MapDisplayer();
        _mapDisplayer.InitializeMapDisplay(map);
    }

    private MapTile[,] CreateMap(int width, int height) {
        MapTile[,] map = new MapTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TerrainData terrainType;
                if (x == width - 1 && y == height - 1)
                {
                    terrainType = Services.TerrainDataManager.terrainDataDict["DOOR"];
                }
                else
                {
                    terrainType = Services.TerrainDataManager.terrainDataDict["GRASS"];
                }
                MapTile tile = new MapTile(x, y, terrainType);
                map[x, y] = tile;
            }
        }
        foreach(MapTile tile in map)
        {
            tile.SetNeigbors(map);
        }
        return map;
    }

    private void CreateGridObject(int x, int y, string name)
    {
        GridObject gridObject = new GridObject(Services.GridObjectDataManager.GetData(name));
        CreateGridObject(x, y, gridObject);
    }

    private void CreateGridObject(int x, int y, GridObject gridObject)
    {
        _gridObjects[gridObject.id] = gridObject;
        gridObject.SpawnOnTile(map[x, y]);
    }

    private void SpawnRandomObjects(string objectName, int num, int minDistFromEnemies = 1)
    {
        while (num > 0)
        {
            int x = Random.Range(0, map.GetLength(0));
            int y = Random.Range(0, map.GetLength(1));
            MapTile mapTile = map[x, y];
            if (mapTile.containedObject == null && mapTile.terrain.terrainName != "DOOR")
            {
                bool tooClose = false;
                foreach (GridObject gridObject in gridObjectList)
                {
                    if(gridObject.data.phylum == GridObjectData.Phylum.ENEMY &&
                        Coord.Distance(gridObject.currentTile.coord, new Coord(x,y)) < minDistFromEnemies)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (!tooClose)
                {
                    CreateGridObject(x, y, objectName);
                    num -= 1;
                }
            }
        }
    }

    private void SpawnChest(int minDistFromPlayer, int minDistFromExit)
    {
        int maxTries = 1000;
        for (int i = 0; i < maxTries; i++)
        {
            int x = Random.Range(0, map.GetLength(0));
            int y = Random.Range(0, map.GetLength(1));
            MapTile mapTile = map[x, y];
            if(mapTile.containedObject == null && mapTile.coord.Distance(new Coord(0,0)) >= minDistFromPlayer 
                && mapTile.coord.Distance(new Coord(map.GetLength(0)-1, map.GetLength(1)-1)) >= minDistFromExit)
            {
                CreateGridObject(x, y, "CHEST");
                return;
            }
        }
    }

    private bool IsCoordInMap(Coord coord)
    {
        return coord.x >= 0 && coord.x < map.GetLength(0) && coord.y >= 0 && coord.y < map.GetLength(1);
    }

    public void OnPlayerTurnEnd(PlayerTurnEnded e)
    {
        _enemyTurnManager.ExecuteEnemyTurn(gridObjectList);
    }

    public void OnGridObjectDeath(GridObjectDeath e)
    {
        _gridObjects.Remove(e.id);
    }
}

public class MapTileSelected : GameEvent
{
    public readonly MapTile mapTile;
    public readonly int selectedCardId;

    public MapTileSelected(MapTile mapTile_, int selectedCardId_)
    {
        mapTile = mapTile_;
        selectedCardId = selectedCardId_;
    }
}
