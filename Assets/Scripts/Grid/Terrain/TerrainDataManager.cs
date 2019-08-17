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
        TextAsset terrainDataDoc = Resources.Load<TextAsset>("Data/Terrain_Database");
        string[] terrainDataEntries = terrainDataDoc.text.Split('\n');
        for (int i = 1; i < terrainDataEntries.Length; i++)
        {
            string[] fields = terrainDataEntries[i].Split('\t');
            AddData(
                fields[0].ToUpper().Trim(), // name
                fields[1] // description
                );
        }
    }

    public void AddData(string name, string description)
    {
        TerrainData terrainData = new TerrainData(name, atlas.GetSprite(name.ToLower()), description);
        terrainDataDict[name] = terrainData;
    }
}
