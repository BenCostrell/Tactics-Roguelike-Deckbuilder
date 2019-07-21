using UnityEngine;
using System.Collections;

public class CardAttack : CardEffect
{
    private readonly int damage;
    private readonly int range;

    public CardAttack(int damage_, int range_)
    {
        damage = damage_;
        range = range_;
    }

    public override void Execute(MapTile target)
    {
        base.Execute(target);
        foreach(GridObject gridObject in target.containedObjects)
        {
            gridObject.TakeDamage(damage);
        }
    }
}
