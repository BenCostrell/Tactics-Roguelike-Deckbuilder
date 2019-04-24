﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayer
{
    private SpriteRenderer[,] tileRenderers;
    private List<SpriteRenderer> gridObjectRenderers;
    private Transform mapHolder;
    private readonly Vector2 centerOffset = new Vector2(0.5f, 1f);

    public void InitializeMapDisplay(MapTile[,] map)
    {
        mapHolder = new GameObject("MapHolder").transform;
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        tileRenderers = new SpriteRenderer[width, height];
        foreach (MapTile tile in map)
        {
            GameObject tileObject = new GameObject("Tile: " + tile.coord.x  + ", " + tile.coord.y);
            SpriteRenderer tileRenderer = tileObject.AddComponent<SpriteRenderer>();
            tileRenderers[tile.coord.x, tile.coord.y] = tileRenderer;
            tileRenderer.sprite = tile.terrain.sprite;
            tileObject.transform.parent = mapHolder;
            tileObject.transform.localPosition = new Vector3(tile.coord.x, tile.coord.y, 0);
        }
        mapHolder.transform.position = new Vector2(-width / 2, -height / 2) + centerOffset;
    }
}
