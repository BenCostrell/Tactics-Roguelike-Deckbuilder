using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainData
{
    public enum TerrainType { GRASS }
    public readonly TerrainType terrainType;
    public readonly Sprite sprite;

    public TerrainData(TerrainType terrainType_, Sprite sprite_)
    {
        terrainType = terrainType_;
        sprite = sprite_;
    }
}
