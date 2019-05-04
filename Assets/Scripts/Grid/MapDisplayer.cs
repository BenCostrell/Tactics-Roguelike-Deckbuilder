﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayer
{
    private SpriteRenderer[,] tileRenderers;
    private Dictionary<int, GridObjectRenderer> gridObjectRenderers;
    public Transform mapHolder { get; private set; }
    private readonly Vector2 centerOffset = new Vector2(0.5f, 1f);
    private List<GridObject> currentlyMovingObjects = new List<GridObject>();

    public void InitializeMapDisplay(MapTile[,] map)
    {
        mapHolder = new GameObject("MapHolder").transform;
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        tileRenderers = new SpriteRenderer[width, height];
        gridObjectRenderers = new Dictionary<int, GridObjectRenderer>();
        foreach (MapTile tile in map)
        {
            GameObject tileObject = new GameObject("Tile: " + tile.coord.x  + ", " + tile.coord.y);
            SpriteRenderer tileRenderer = tileObject.AddComponent<SpriteRenderer>();
            tileRenderers[tile.coord.x, tile.coord.y] = tileRenderer;
            tileRenderer.sprite = tile.terrain.sprite;
            tileRenderer.sortingLayerName = "Map";
            tileObject.transform.parent = mapHolder;
            tileObject.transform.localPosition = new Vector3(tile.coord.x, tile.coord.y, 0);
        }
        mapHolder.transform.position = new Vector2(-width / 2, -height / 2) + centerOffset;
        Services.EventManager.Register<GridObjectSpawned>(OnGridObjectSpawned);
        Services.EventManager.Register<GridObjectMoved>(OnGridObjectMoved);
        foreach(MapTile tile in map)
        {
            foreach(GridObject gridObject in tile.containedObjects)
            {
                Services.EventManager.Fire(new GridObjectSpawned(gridObject, tile));
            }
        }
    }

    public void OnGridObjectMoved(GridObjectMoved e)
    {
        gridObjectRenderers[e.gridObject.id].MoveToTile(e);
    }

    public void OnGridObjectSpawned(GridObjectSpawned e)
    {
        GridObjectRenderer gridObjectRenderer = new GameObject().AddComponent<GridObjectRenderer>();
        gridObjectRenderer.Initialize(e.gridObject, e.mapTile, mapHolder);
        gridObjectRenderers[e.gridObject.id] = gridObjectRenderer;
    }

    public void Update()
    {

    }
}