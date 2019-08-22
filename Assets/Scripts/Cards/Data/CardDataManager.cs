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
        TextAsset cardDataDoc = Resources.Load<TextAsset>("Data/Card_Database");
        string[] cardDataEntries = cardDataDoc.text.Split('\n');
        for (int i = 1; i < cardDataEntries.Length; i++)
        {
            string[] fields = cardDataEntries[i].Split('\t');
            AddData(
                fields[0], // name
                int.Parse(fields[1]), // cost
                fields[2].Split(';'), // effects
                fields[3],// text
                CardData.rarityStringDict[fields[4].ToUpper().Trim()], // rarity
                int.Parse(fields[5]) // starting deck counts
                ); 
        }
        //temporary, will ultimately load in from spreadsheet
        //AddData("Whack", 1, new string[] { "ATTACK,1,1,1" }, 
        //    "Deal 1 damage to a target within 1 range.");
    }

    private void AddData(string name, int cost, string[] cardEffectStrings, string text, CardData.Rarity rarity, int startingDeckCount)
    {
        List<CardEffect> effects = new List<CardEffect>();
        foreach(string effectString in cardEffectStrings)
        {
            CardEffect effect = CardEffect.ParseEffectString(effectString);
            if (effect != null) effects.Add(effect);
        }
        CardData data = new CardData(name, cost, effects, text, atlas.GetSprite(name.ToLower()), rarity, startingDeckCount);
        cardDataDict[name] = data;
    }

    public CardData GetData(string name)
    {
        return cardDataDict[name];
    }

    public List<CardData> GetStartingDeckData()
    {
        List<CardData> startingDeckData = new List<CardData>();
        foreach(CardData cardData in cardDataDict.Values)
        {
            if(cardData.startingDeckCount > 0)
            {
                startingDeckData.Add(cardData);
            }
        }
        return startingDeckData;
    }

    public List<CardData> GetCardsOfRarity(CardData.Rarity rarity)
    {
        List<CardData> cards = new List<CardData>();
        foreach(CardData cardData in cardDataDict.Values)
        {
            if (cardData.rarity == rarity) cards.Add(cardData);
        }
        return cards;
    }
}
