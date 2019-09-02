using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyTurnBehavior
{
    public readonly int priority;

    public EnemyTurnBehavior(int priority_)
    {
        priority = priority_;
    }

    public virtual void OnEnemyTurn(GridObject gridObject)
    {

    }

    public static EnemyTurnBehavior ParseBehaviorString(string behaviorString)
    {
        string[] splitBehaviorString = behaviorString.Split(',');
        string baseBehaviorString = splitBehaviorString[0].ToUpper();
        switch (baseBehaviorString)
        {
            case "ATTACK":
                return new Attack(
                    int.Parse(splitBehaviorString[1]), // move speed
                    int.Parse(splitBehaviorString[2]), // damage
                    int.Parse(splitBehaviorString[3]) // range
                    ); 
            default:
                return null;
        }
    }
}
