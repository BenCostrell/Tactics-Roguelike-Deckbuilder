using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplayer
{
    private TileRenderer[,] tileRenderers;
    private Dictionary<int, GridObjectRenderer> gridObjectRenderers;
    public Transform mapHolder { get; private set; }
    private readonly Vector2 centerOffset = new Vector2(0.5f, 2f);
    private List<GridObject> currentlyMovingObjects = new List<GridObject>();
    private Reticle reticle;
    private Queue<GameEvent> animationQueue;
    private bool wasAnimating;
    private GridObjectRenderer gridObjectRendererPrefab;

    public void InitializeMapDisplay(MapTile[,] map)
    {
        gridObjectRendererPrefab = Resources.Load<GridObjectRenderer>("Prefabs/GridObjectRenderer");
        mapHolder = new GameObject("MapHolder").transform;
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        tileRenderers = new TileRenderer[width, height];
        gridObjectRenderers = new Dictionary<int, GridObjectRenderer>();
        foreach (MapTile tile in map)
        {
            GameObject gameObject = new GameObject();
            TileRenderer tileRenderer = gameObject.AddComponent<TileRenderer>();
            tileRenderer.Init(tile, mapHolder);
            tileRenderers[tile.coord.x, tile.coord.y] = tileRenderer;
        }
        mapHolder.transform.position = new Vector2(-width / 2, -height / 2) + centerOffset;
        Services.EventManager.Register<GridObjectSpawned>(OnGridObjectSpawned);
        Services.EventManager.Register<GridObjectMoved>(OnGridObjectMoved);
        Services.EventManager.Register<ObjectAttacked>(OnObjectAttacked);
        foreach (MapTile tile in map)
        {
            foreach(GridObject gridObject in tile.containedObjects)
            {
                Services.EventManager.Fire(new GridObjectSpawned(gridObject, tile));
            }
        }
        GameObject reticleObj = new GameObject();
        reticle = reticleObj.AddComponent<Reticle>();
        reticle.Init(mapHolder);
        animationQueue = new Queue<GameEvent>();
    }

    public void OnObjectAttacked(ObjectAttacked e)
    {
        animationQueue.Enqueue(e);
    }

    public void OnGridObjectMoved(GridObjectMoved e)
    {
        animationQueue.Enqueue(e);
    }

    public void OnGridObjectSpawned(GridObjectSpawned e)
    {
        GridObjectRenderer gridObjectRenderer = GameObject.Instantiate(gridObjectRendererPrefab, 
            mapHolder).GetComponent<GridObjectRenderer>();
        gridObjectRenderer.Initialize(e.gridObject, e.mapTile);
        gridObjectRenderers[e.gridObject.id] = gridObjectRenderer;
    }

    private void ProcessAnimationQueue()
    {
        foreach (GridObjectRenderer renderer in gridObjectRenderers.Values)
        {
            if (renderer.animating) return;
        }
        if (animationQueue.Count == 0)
        {
            if (wasAnimating)
            {
                wasAnimating = false;
                Services.EventManager.Fire(new AllQueuedAnimationsComplete());
            }
            return;
        }
        GameEvent animationEvent = animationQueue.Dequeue();
        Type eventType = animationEvent.GetType();
        if(eventType == typeof(GridObjectMoved))
        {
            GridObjectMoved movement = animationEvent as GridObjectMoved;
            gridObjectRenderers[movement.gridObject.id].MoveToTile(movement);
        }
        else if (eventType == typeof(ObjectAttacked))
        {
            ObjectAttacked attack = animationEvent as ObjectAttacked;
            gridObjectRenderers[attack.attacker.id].Attack(attack);
        }
        
        wasAnimating = true;
    }

    public void Update()
    {
        ProcessAnimationQueue();
    }
}

public class AllQueuedAnimationsComplete : GameEvent { }
