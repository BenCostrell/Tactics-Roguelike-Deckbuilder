using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridObjectData 
{
    public enum GridObjectType { PLAYER, GOBLIN }
    public readonly GridObjectType gridObjectType;
    public readonly Sprite sprite;
    public readonly bool enemy;
    public readonly int maxHealth;

    public GridObjectData(GridObjectType gridObjectType_, Sprite sprite_, bool enemy_, int maxHealth_)
    {
        gridObjectType = gridObjectType_;
        sprite = sprite_;
        enemy = enemy_;
        maxHealth = maxHealth_;
    }
}
