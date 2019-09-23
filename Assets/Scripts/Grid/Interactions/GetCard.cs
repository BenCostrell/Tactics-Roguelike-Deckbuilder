using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetCard : ObjectInteraction
{
    private readonly CardData.Rarity rarity;
    private const int numCardOptions = 3;

    public GetCard(CardData.Rarity rarity_) : base()
    {
        rarity = rarity_;
    }

    public override void OnInteract(GridObject gridObject)
    {
        if (gridObject.used)
        {
            return;
        }
        gridObject.used = true;
        // get sum cardz
        List<CardData> possibleCards = Services.CardDataManager.GetCardsOfRarity(rarity);
        List<CardData> offeredCards = new List<CardData>();
        for (int i = 0; i < numCardOptions; i++)
        {
            CardData card = possibleCards[Random.Range(0, possibleCards.Count)];
            possibleCards.Remove(card);
            offeredCards.Add(card);
        }
        Services.EventManager.Fire(new ChestOpened(offeredCards));
        //foreach (CardData card in offeredCards)
        //{
        //    Debug.Log("offering " + card.name);
        //}
    }
}

public class ChestOpened : GameEvent
{
    public readonly List<CardData> offeredCards;
    public ChestOpened(List<CardData> offeredCards_)
    {
        offeredCards = offeredCards_;
    }
}
