using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyTurnManager
{
    private List<GridObject> _enemies;
    private TaskManager _tm;

    public EnemyTurnManager()
    {
        _enemies = new List<GridObject>();
        Services.EventManager.Register<GridObjectSpawned>(RegisterEnemy);
    }

    public void RegisterEnemy(GridObjectSpawned e)
    {
        if(e.gridObject.data.enemy)
        {
            _enemies.Add(e.gridObject);
        }
    }

    public void ExecuteEnemyTurn(PlayerTurnEnded e)
    {

    }

    public void Update()
    {
        _tm.Update();
    }
}
