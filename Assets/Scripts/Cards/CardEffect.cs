using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardEffect
{
    public readonly bool targeted;
    public readonly int minRange;
    public readonly int maxRange;
    protected Player player { get { return Services.LevelManager.player; } }

    public CardEffect(bool targeted_, int minRange_ = 0, int maxRange_ = 0)
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
        int dist = Coord.Distance(target.coord, player.currentTile.coord);
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
            case "MANA":
                return new Mana(int.Parse(splitEffectString[1]));
            case "DRAW":
                return new Draw(int.Parse(splitEffectString[1]));
            // need to figure out syntax to allow for nesting card effects inside conditionals, or alternative solution
            //case "COND":
            //    return new Conditional()
            case "TARGET_PHYLA":
                List<GridObjectData.Phylum> allowedPhyla = new List<GridObjectData.Phylum>();
                for (int i = 1; i < splitEffectString.Length; i++)
                {
                    allowedPhyla.Add(GridObjectData.StringToPhylum(splitEffectString[i]));
                }
                return new PhylumTargetOnly(allowedPhyla);
            default:
                return null;
        }
    }

}
