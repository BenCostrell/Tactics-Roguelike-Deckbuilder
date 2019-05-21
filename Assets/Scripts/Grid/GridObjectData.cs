using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridObjectData 
{
    public readonly string gridObjectName;
    public readonly Sprite sprite;
    public readonly int maxHealth;
    public readonly int moveSpeed;
    public readonly List<EnemyTurnBehavior> enemyTurnBehaviors;

    public GridObjectData(string gridObjectName_, Sprite sprite_, int maxHealth_,
        int moveSpeed_, List<EnemyTurnBehavior> enemyTurnBehaviors_)
    {
        gridObjectName = gridObjectName_;
        sprite = sprite_;
        maxHealth = maxHealth_;
        moveSpeed = moveSpeed_;
        enemyTurnBehaviors = enemyTurnBehaviors_;
    }
}
