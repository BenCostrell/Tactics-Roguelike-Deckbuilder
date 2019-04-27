using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectData 
{
    public enum GridObjectType { PLAYER }
    public readonly GridObjectType gridObjectType;
    public readonly Sprite sprite;

    public GridObjectData(GridObjectType gridObjectType_, Sprite sprite_)
    {
        gridObjectType = gridObjectType_;
        sprite = sprite_;
    }
}
