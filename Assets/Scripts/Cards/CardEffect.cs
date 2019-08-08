using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardEffect
{
    public readonly bool targeted;
    public readonly int minRange;
    public readonly int maxRange;

    public CardEffect(bool targeted_, int minRange_, int maxRange_)
    {
        targeted = targeted_;
        minRange = minRange_;
        maxRange = maxRange_;
    }

    public virtual void Execute(MapTile target)
    {

    }

    public virtual bool IsTargetLegal(MapTile target)
    {
        if (!targeted) return true;
        if (targeted && target == null) return false;
        int dist = Coord.Distance(target.coord, Services.LevelManager.player.currentTile.coord);
        if (dist > maxRange || dist < minRange) return false;
        return true;
    }

    // for the full list, expecting strings of the form: 
    // "..;EFFECT_TYPE1,param1a,param2b;EFFECT_TYPE2,param2a;..."
    // each individual effect being the substring "EFFECT_TYPE1,param1a,param2b"
    // which is what this function is expecting
    public static CardEffect ParseEffectString(string cardEffectString)
    {
        string[] splitEffectString = cardEffectString.Split(',');
        string baseEffectString = splitEffectString[0].ToUpper();
        switch (baseEffectString)
        {
            case "ATTACK":
                return new CardAttack(
                    int.Parse(splitEffectString[1]), // damage
                    int.Parse(splitEffectString[2]), // minRange
                    int.Parse(splitEffectString[3])); // maxRange
            default:
                return null;
        }
    }
}
