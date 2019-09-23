using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    public MapTile currentTile { get; protected set; }
    public readonly int id;
    private static int _nextId;
    private static int nextId
    {
        get
        {
            _nextId += 1;
            return _nextId;
        }
    }
    public GridObjectData data { get; private set; }

    public int maxHealth { get; protected set; }
    public int currentHealth { get; protected set; }

    public bool used;

    public GridObject(GridObjectData data_)
    {
        id = nextId;
        data = data_;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }

    public void SpawnOnTile(MapTile mapTile)
    {
        EnterTile(mapTile);
        OnSpawn();
        Services.EventManager.Fire(new GridObjectSpawned(this, mapTile));
    }

    public void MoveToTile(List<MapTile> path)
    {
        MapTile originalTile = currentTile;
        Services.EventManager.Fire(new GridObjectMoved(this, originalTile, path));
        ExitTile(originalTile);
        EnterTile(path[path.Count - 1]);
    }

    private void EnterTile(MapTile mapTile)
    {
        mapTile.OnObjectEnter(this);
        currentTile = mapTile;
    }

    private void ExitTile(MapTile mapTile)
    {
        mapTile.OnObjectExit(this);
    }

    public virtual bool IsTilePassable(MapTile mapTile, bool raw = false, bool ignoreEnemies = false)
    {
        GridObject effectivelyContainedObject = mapTile.containedObject;
        if (ignoreEnemies)
        {
            if (effectivelyContainedObject != null &&
                effectivelyContainedObject.data.phylum == GridObjectData.Phylum.ENEMY)
            {
                effectivelyContainedObject = null;
            }
        }
        return raw || effectivelyContainedObject == null || mapTile == currentTile;
    }

    protected bool IsTileReachable(int moves, List<MapTile> path)
    {
        return moves >= path.Count && path.Count != 0;
    }

    protected virtual void OnSpawn()
    {
        Services.EventManager.Register<InputDown>(OnInputDown);
    }

    protected virtual void OnInputDown(InputDown e)
    {
        if (e.hoveredTile == null || e.hoveredTile.tile != currentTile || 
            Services.LevelManager.player.currentTile.coord.Distance(currentTile.coord) != 1) return;
        OnInteract();
    }

    public void ChangeHealth(int change)
    {
        int prevHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + change, 0, maxHealth);
        int healthChange = prevHealth - currentHealth;
        Services.EventManager.Fire(new HealthChange(this, healthChange));
        if (currentHealth == 0) Die();
    }

    public void Die()
    {
        currentTile.OnObjectExit(this);
        Services.EventManager.Unregister<InputDown>(OnInputDown);
        Services.EventManager.Fire(new GridObjectDeath(id));
    }

    public void OnInteract()
    {
        foreach (ObjectInteraction interaction in data.interactions)
        {
            interaction.OnInteract(this);
        }
    }
}

public class GridObjectMoved : GameEvent
{
    public readonly GridObject gridObject;
    public readonly MapTile originalTile;
    public readonly List<MapTile> path;

    public GridObjectMoved(GridObject gridObject_, MapTile originalTile_, List<MapTile> path_)
    {
        gridObject = gridObject_;
        originalTile = originalTile_;
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

public class HealthChange : GameEvent
{
    public readonly GridObject gridObject;
    public readonly int change;

    public HealthChange(GridObject gridObject_, int change_)
    {
        gridObject = gridObject_;
        change = change_;
    }
}

public class GridObjectDeath : GameEvent
{
    public readonly int id;
    public GridObjectDeath(int id_)
    {
        id = id_;
    }
}
