using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class EnemyTurnManager
{
    private TaskManager _tm;

    public EnemyTurnManager()
    {
        _tm = new TaskManager();
    }

    public void ExecuteEnemyTurn(List<GridObject> gridObjects)
    {
        TaskQueue enemyTurnTasks = new TaskQueue();
        foreach(GridObject gridObject in gridObjects)
        {
            List<EnemyTurnBehavior> behaviors = new List<EnemyTurnBehavior>(
                gridObject.data.enemyTurnBehaviors.OrderBy(b => b.priority));
            foreach(EnemyTurnBehavior behavior in behaviors)
            {
                enemyTurnTasks.Then(behavior.OnEnemyTurn(gridObject));
            }
        }

        enemyTurnTasks.Add(new ParameterizedActionTask<PlayerTurnStarted>(
            Services.EventManager.Fire, new PlayerTurnStarted()));
        _tm.AddTask(enemyTurnTasks);
    }

    public void Update()
    {
        _tm.Update();
    }
}
