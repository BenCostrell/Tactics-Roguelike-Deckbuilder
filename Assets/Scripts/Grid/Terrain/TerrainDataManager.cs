using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TerrainDataManager
{
    public Dictionary<string, TerrainData> terrainDataDict;
    private SpriteAtlas atlas;

    public TerrainDataManager()
    {
        terrainDataDict = new Dictionary<string, TerrainData>();
        atlas = Resources.Load<SpriteAtlas>("SpriteData/TerrainSprites");
        //temporary, will ultimately load in from spreadsheet
        AddData("GRASS");
    }

    public void AddData(string name)
    {
        TerrainData terrainData = new TerrainData(name, atlas.GetSprite(name.ToLower()));
        terrainDataDict[name] = terrainData;
    }
}
