using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridObjectData 
{
    public readonly string gridObjectName;
    public readonly Sprite sprite;
    public readonly int maxHealth;
    public readonly List<EnemyTurnBehavior> enemyTurnBehaviors;
    public readonly List<ObjectInteraction> interactions;
    public enum Phylum { NONE, ENEMY, PLANT, PLAYER }
    public readonly Phylum phylum;
    public readonly int targetPriority;

    public GridObjectData(string gridObjectName_, Sprite sprite_, int maxHealth_,
        List<EnemyTurnBehavior> enemyTurnBehaviors_, List<ObjectInteraction> interactions_, 
        Phylum phylum_, int targetPriority_)
    {
        gridObjectName = gridObjectName_;
        sprite = sprite_;
        maxHealth = maxHealth_;
        enemyTurnBehaviors = enemyTurnBehaviors_;
        interactions = interactions_;
        phylum = phylum_;
        targetPriority = targetPriority_;
    }

    public static Phylum StringToPhylum(string str)
    {
        switch (str.ToUpper())
        {
            case "ENEMY":
                return Phylum.ENEMY;
            case "PLANT":
                return Phylum.PLANT;
            case "PLAYER":
                return Phylum.PLAYER;
            default:
                return Phylum.NONE;
        }
    }
}
