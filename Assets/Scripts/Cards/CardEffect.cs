using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CardEffect
{

    public virtual void Execute(MapTile target)
    {

    }


    // expecting strings of the form: "..;EFFECT_TYPE1,param1a,param2b;EFFECT_TYPE2,param2a;..."
    public static CardEffect ParseEffectString(string cardEffectString)
    {
        string[] splitEffectString = cardEffectString.Split(',');
        string baseEffectString = splitEffectString[0].ToUpper();
        switch (baseEffectString)
        {
            case "ATTACK":
                return new CardAttack(
                    int.Parse(splitEffectString[1]),
                    int.Parse(splitEffectString[2]));
            default:
                return null;
        }
    }
}
