using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Approach : EnemyTurnBehavior
{
    public Approach() : base(1)
    {

    }

    public override TaskQueue OnEnemyTurn(GridObject gridObject)
    {
        List<MapTile> pathToPlayer = new List<MapTile>();


        return base.OnEnemyTurn(gridObject);
    }
}
