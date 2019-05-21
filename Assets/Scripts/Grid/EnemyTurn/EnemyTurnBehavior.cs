﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyTurnBehavior
{
    public static Dictionary<string, EnemyTurnBehavior> behaviors =
        new Dictionary<string, EnemyTurnBehavior>()
        {
            {"APPROACH", new Approach() }
        };

    public readonly int priority;

    public EnemyTurnBehavior(int priority_)
    {
        priority = priority_;
    }

    public virtual TaskQueue OnEnemyTurn(GridObject gridObject)
    {
        return new TaskQueue(new List<Task>() { new EmptyTask() });
    }
}
