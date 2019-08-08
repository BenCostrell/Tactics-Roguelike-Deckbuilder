using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;

public class CardDataManager
{
    private Dictionary<string, CardData> cardDataDict;
    private SpriteAtlas atlas;

    public CardDataManager()
    {
        cardDataDict = new Dictionary<string, CardData>();
        atlas = Resources.Load<SpriteAtlas>("SpriteData/CardSprites");
        //temporary, will ultimately load in from spreadsheet
        AddData("Whack", 1, new List<string>() { "ATTACK,1,1,1" }, 
            "Deal 1 damage to a target within 1 range.");
    }

    private void AddData(string name, int cost, List<string> cardEffectStrings, string text)
    {
        List<CardEffect> effects = new List<CardEffect>();
        foreach(string effectString in cardEffectStrings)
        {
            CardEffect effect = CardEffect.ParseEffectString(effectString);
            if (effect != null) effects.Add(effect);
        }
        CardData data = new CardData(name, cost, effects, text, atlas.GetSprite(name.ToLower()));
        cardDataDict[name] = data;
    }

    public CardData GetData(string name)
    {
        return cardDataDict[name];
    }
}
