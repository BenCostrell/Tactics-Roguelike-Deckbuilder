using UnityEngine;
using System.Collections;

public class Conditional : CardEffect
{
    private readonly CardEffect ifTrue;
    private readonly Condition condition;

    public Conditional(Condition condition_, CardEffect ifTrue_) :base(false)
    {
        condition = condition_;
        ifTrue = ifTrue_;
    }

    public override bool IsTargetLegal(MapTile target)
    {
        return base.IsTargetLegal(target) && ifTrue.IsTargetLegal(target);
    }

    public override void Execute(MapTile target)
    {
        base.Execute(target);
        if (condition.Evaluate(target))
        {
            ifTrue.Execute(target);
        }
    }
}
