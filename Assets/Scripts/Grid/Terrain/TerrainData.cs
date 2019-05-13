using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainData
{
    public readonly string terrainName;
    public readonly Sprite sprite;

    public TerrainData(string terrainName_, Sprite sprite_)
    {
        terrainName = terrainName_;
        sprite = sprite_;
    }
}
