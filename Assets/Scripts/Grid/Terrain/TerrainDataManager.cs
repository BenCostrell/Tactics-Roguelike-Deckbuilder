using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDataManager
{
    public Dictionary<TerrainData.TerrainType, TerrainData> terrainDataDict;

    public TerrainDataManager()
    {
        terrainDataDict = new Dictionary<TerrainData.TerrainType, TerrainData>();
        //temporary, will ultimately load in from spreadsheet
        AddData(TerrainData.TerrainType.GRASS, Resources.LoadAll<Sprite>("Sprites/overworld_tileset_grass")[0]);
    }

    public void AddData(TerrainData.TerrainType type, Sprite sprite)
    {
        TerrainData terrainData = new TerrainData(type, sprite);
        terrainDataDict[terrainData.terrainType] = terrainData;
    }
}
