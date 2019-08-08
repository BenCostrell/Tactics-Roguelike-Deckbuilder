using UnityEngine;
using System.Collections;

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
        foreach(GridObject gridObject in target.containedObjects)
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
