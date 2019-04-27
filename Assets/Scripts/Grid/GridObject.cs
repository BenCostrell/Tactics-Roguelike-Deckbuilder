using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    protected MapTile _currentTile;
    public readonly int id;
    private static int _nextId;
    public static int nextId
    {
        get
        {
            _nextId += 1;
            return _nextId;
        }
    }
    public GridObjectData data { get; private set; }

    public GridObject(GridObjectData data_)
    {
        id = nextId;
        data = data_;
        if(data.gridObjectType == GridObjectData.GridObjectType.PLAYER)
        {
            Services.EventManager.Register<MapTileSelected>(OnTileSelected);
        }
    }

    public void SpawnOnTile(MapTile mapTile)
    {
        EnterTile(mapTile);
        Services.EventManager.Fire(new GridObjectSpawned(this, mapTile));
    }

    public void MoveToTile(MapTile targetTile, List<MapTile> path)
    {
        MapTile originalTile = _currentTile;
        ExitTile(originalTile);
        EnterTile(targetTile);
        Services.EventManager.Fire(new GridObjectMoved(this, originalTile, targetTile, path));
    }

    private void OnTileSelected(MapTileSelected e)
    {
        MapTile targetTile = e.mapTile;
        if (targetTile != _currentTile)
        {
            List<MapTile> path = AStarSearch.ShortestPath(_currentTile, targetTile, this);
            MoveToTile(targetTile, path);
        }
    }

    private void EnterTile(MapTile mapTile)
    {
        mapTile.OnObjectEnter(this);
        _currentTile = mapTile;
    }

    private void ExitTile(MapTile mapTile)
    {
        mapTile.OnObjectExit(this);
    }

    public virtual bool IsTilePassable(MapTile mapTile)
    {
        return mapTile.containedObjects.Count == 0 || mapTile == _currentTile;
    }
}

public class GridObjectMoved : GameEvent
{
    public readonly GridObject gridObject;
    public readonly MapTile originalTile;
    public readonly MapTile targetTile;
    public readonly List<MapTile> path;

    public GridObjectMoved(GridObject gridObject_, MapTile originalTile_, MapTile targetTile_, List<MapTile> path_)
    {
        gridObject = gridObject_;
        originalTile = originalTile_;
        targetTile = targetTile_;
        path = path_;
    }
}

public class GridObjectSpawned : GameEvent
{
    public readonly GridObject gridObject;
    public readonly MapTile mapTile;

    public GridObjectSpawned(GridObject gridObject_, MapTile mapTile_)
    {
        gridObject = gridObject_;
        mapTile = mapTile_;
    }
}

