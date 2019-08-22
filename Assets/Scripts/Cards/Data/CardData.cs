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
    public readonly int startingDeckCount;
    public enum Rarity {  Basic, Common, Uncommon, Rare }
    public readonly Rarity rarity;
    public static Dictionary<string, Rarity> rarityStringDict = new Dictionary<string, Rarity>()
    {
        {"BASIC", Rarity.Basic },
        { "COMMON", Rarity.Common },
        { "UNCOMMON", Rarity.Uncommon },
        { "RARE", Rarity.Rare}
    };

    public CardData(string name_, int cost_, List<CardEffect> effects_, string text_, Sprite sprite_, Rarity rarity_, int startingDeckCount_)
    {
        name = name_;
        cost = cost_;
        effects = effects_;
        rawText = text_;
        sprite = sprite_;
        rarity = rarity_;
        startingDeckCount = startingDeckCount_;
    }
}
