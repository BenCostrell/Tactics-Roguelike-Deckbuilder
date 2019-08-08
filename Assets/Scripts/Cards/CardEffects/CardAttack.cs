using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardAttack : CardEffect
{
    private readonly int damage;

    public CardAttack(int damage_, int minRange_, int maxRange_) : base(true, minRange_, maxRange_)
    {
        damage = damage_;
    }

    public override void Execute(MapTile target)
    {
        base.Execute(target);
        List<GridObject> tileObjects = new List<GridObject>(target.containedObjects);
        foreach(GridObject gridObject in tileObjects)
        {
            gridObject.TakeDamage(damage);
            // for now, no animation
            Services.EventManager.Fire(new AttackAnimationComplete(gridObject.id));
        }
    }

    public override bool IsTargetLegal(MapTile target)
    {
        bool baseLegal = base.IsTargetLegal(target);
        if (!baseLegal) return false;
        if (target.containedObjects.Count == 0) return false;
        return true;
    }
}
