using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridObjectData 
{
    public readonly string gridObjectName;
    public readonly Sprite sprite;
    public readonly bool enemy;
    public readonly int maxHealth;

    public GridObjectData(string gridObjectName_, Sprite sprite_, bool enemy_, int maxHealth_)
    {
        gridObjectName = gridObjectName_;
        sprite = sprite_;
        enemy = enemy_;
        maxHealth = maxHealth_;
    }
}
