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
        if (gridObject.opened)
        {
            return;
        }
        gridObject.opened = true;
        // get sum cardz
        List<CardData> possibleCards = Services.CardDataManager.GetCardsOfRarity(rarity);
        List<CardData> offeredCards = new List<CardData>();
        for (int i = 0; i < numCardOptions; i++)
        {
            CardData card = possibleCards[Random.Range(0, possibleCards.Count)];
            possibleCards.Remove(card);
            offeredCards.Add(card);
        }
        foreach(CardData card in offeredCards)
        {
            Debug.Log("offering " + card.name);
        }
    }
}
