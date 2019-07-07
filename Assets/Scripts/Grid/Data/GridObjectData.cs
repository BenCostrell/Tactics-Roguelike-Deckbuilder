using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridObjectData 
{
    public readonly string gridObjectName;
    public readonly Sprite sprite;
    public readonly int maxHealth;
    public readonly int moveSpeed;
    public readonly int attackRange;
    public readonly int attackDamage;
    public readonly List<EnemyTurnBehavior> enemyTurnBehaviors;
    public enum TargetPriority { ONLY_PLAYER, ONLY_PLANT, PLAYER_PLANT, PLANT_PLAYER, NEAREST, NONE }
    public readonly TargetPriority targetPriority;
    public enum Phylum { ENEMY, PLANT, PLAYER }
    public readonly Phylum phylum;

    public GridObjectData(string gridObjectName_, Sprite sprite_, int maxHealth_,
        int moveSpeed_, int attackRange_, int attackDamage_,
        List<EnemyTurnBehavior> enemyTurnBehaviors_, TargetPriority targetPriority_, 
        Phylum phylum_)
    {
        gridObjectName = gridObjectName_;
        sprite = sprite_;
        maxHealth = maxHealth_;
        moveSpeed = moveSpeed_;
        attackRange = attackRange_;
        attackDamage = attackDamage_;
        enemyTurnBehaviors = enemyTurnBehaviors_;
        targetPriority = targetPriority_;
        phylum = phylum_;
    }
}
