using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CardData 
{
    public readonly string name;
    public readonly int cost;
    public readonly List<CardEffect> effects;
    public readonly string rawText;
    public string text
    {
        get
        {
            return rawText; //TODO: make this parse the raw text
        }
    }
    public readonly Sprite sprite;
    public bool targeted
    {
        get
        {
            foreach (CardEffect cardEffect in effects)
            {
                if (cardEffect.targeted) return true;
            }
            return false;
        }
    }


    public CardData(string name_, int cost_, List<CardEffect> effects_, string text_, Sprite sprite_)
    {
        name = name_;
        cost = cost_;
        effects = effects_;
        rawText = text_;
        sprite = sprite_;
    }
}
