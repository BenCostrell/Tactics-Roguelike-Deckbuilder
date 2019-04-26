using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public MapTile[,] map { get; private set; }
    private TerrainDataManager terrainDataManager;

    public void InitializeMap(int width, int height)
    {
        InitializeTerrainData();
        map = CreateMap(width, height);
    }

    private void InitializeTerrainData()
    {
        terrainDataManager = new TerrainDataManager();
        //temporary, will ultimately load in from spreadsheet
        terrainDataManager.terrainDataDict.Add(TerrainData.TerrainType.GRASS,
            new TerrainData(TerrainData.TerrainType.GRASS,
            Resources.LoadAll<Sprite>("Sprites/overworld_tileset_grass")[0]));
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
}
