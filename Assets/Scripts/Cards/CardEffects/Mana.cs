using UnityEngine;
using System.Collections;

public class Mana : CardEffect
{
    private readonly int mana;

    public Mana (int mana_) : base(false)
    {
        mana = mana_;
    }

    public override void Execute(MapTile target)
    {
        base.Execute(target);
        player.GainEnergy(mana);
    }
}
