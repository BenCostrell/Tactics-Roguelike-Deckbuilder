using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDataManager
{
    public Dictionary<TerrainData.TerrainType, TerrainData> terrainDataDict;

    public TerrainDataManager()
    {
        terrainDataDict = new Dictionary<TerrainData.TerrainType, TerrainData>();
    }

    public void AddData(TerrainData terrainData)
    {
        terrainDataDict[terrainData.terrainType] = terrainData;
    }
}
