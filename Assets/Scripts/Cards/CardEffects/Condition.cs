using UnityEngine;
using System.Collections;

public abstract class Condition
{
    public abstract bool Evaluate(MapTile target);
}
