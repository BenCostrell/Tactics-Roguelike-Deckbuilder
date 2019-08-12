using UnityEngine;
using System.Collections;

public class Draw : CardEffect
{
    private readonly int numCards;

    public Draw(int numCards_) : base(false)
    {
        numCards = numCards_;
    }

    public override void Execute(MapTile target)
    {
        base.Execute(target);
        for (int i = 0; i < numCards; i++)
        {
            Services.CardManager.DrawCard();
        }
    }
}
