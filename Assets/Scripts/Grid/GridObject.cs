﻿using System.Collections;
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

    public int moveSpeed { get { return data.moveSpeed + _bonusMoveSpeed; } }
    private int _bonusMoveSpeed;
    public int maxHealth {  get { return data.maxHealth + _bonusMaxHealth; } }
    private int _bonusMaxHealth;
    public int currentHealth { get; private set; }
    public int attackDamage {  get { return data.attackDamage + _bonusAttackDamage; } }
    private int _bonusAttackDamage;

    public GridObject(GridObjectData data_)
    {
        id = nextId;
        data = data_;
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
        ExitTile(originalTile);
        EnterTile(path[path.Count-1]);
        Services.EventManager.Fire(new GridObjectMoved(this, originalTile, path));
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

    public virtual bool IsTilePassable(MapTile mapTile)
    {
        return mapTile.containedObjects.Count == 0 || mapTile == currentTile;
    }

    protected bool IsTileReachable(int moves, List<MapTile> path)
    {
        return moves >= path.Count && path.Count != 0;
    }

    protected virtual void OnSpawn()
    {

    }

    public void TakeDamage(int damage)
    {
        int prevHealth = currentHealth;
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        int damageTaken = prevHealth - currentHealth;
        Services.EventManager.Fire(new DamageTaken(this, damageTaken));
    }

    public void Die()
    {

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

public class DamageTaken : GameEvent
{
    public readonly GridObject gridObject;
    public readonly int damage;

    public DamageTaken(GridObject gridObject_, int damage_)
    {
        gridObject = gridObject_;
        damage = damage_;
    }
}

