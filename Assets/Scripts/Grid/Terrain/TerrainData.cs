using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TerrainData
{
    public readonly string terrainName;
    public readonly Sprite sprite;
    public readonly string description;

    public TerrainData(string terrainName_, Sprite sprite_, string description_)
    {
        terrainName = terrainName_;
        sprite = sprite_;
        description = description_;
    }
}
