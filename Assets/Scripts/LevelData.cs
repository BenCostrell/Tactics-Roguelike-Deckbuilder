using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelData 
{
    public readonly int difficulty;
    public readonly List<GridObjectSpawnData> enemySpawns;
    public readonly List<GridObjectSpawnData> vegetationSpawns;
    public readonly List<GridObjectSpawnData> rewardSpawns;
    public readonly IntVector2 size;
    public bool completed;
    // include zone type data eventually

    public LevelData(int difficulty_, IntVector2 size_, List<GridObjectSpawnData> enemySpawns_,
        List<GridObjectSpawnData> vegetationSpawns_, List<GridObjectSpawnData> rewardSpawns_)
    {
        difficulty = difficulty_;
        size = size_;
        enemySpawns = enemySpawns_;
        vegetationSpawns = vegetationSpawns_;
        rewardSpawns = rewardSpawns_;
    }
}

public class GridObjectSpawnData
{
    public readonly GridObjectData gridObjectData;
    public readonly int numToSpawn;

    public GridObjectSpawnData(GridObjectData gridObjectData_, int numToSpawn_)
    {
        gridObjectData = gridObjectData_;
        numToSpawn = numToSpawn_;
    }
}