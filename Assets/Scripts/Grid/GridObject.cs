using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject
{
    protected MapTile _currentTile;

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

