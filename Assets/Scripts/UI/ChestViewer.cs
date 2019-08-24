using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestViewer : MonoBehaviour
{
    public Transform offeredCardHolder;
    private List<Card> offeredCards;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        Services.EventManager.Register<ChestOpened>(OnChestOpened);
        offeredCards = new List<Card>();
    }

    private void OnChestOpened(ChestOpened e)
    {
        gameObject.SetActive(true);
        for (int i = 0; i < e.offeredCards.Count; i++)
        {
            CardData cardData = e.offeredCards[i];
            Card card = new Card(cardData);
            Services.EventManager.Fire(new CardCreated(card, offeredCardHolder));
            offeredCards.Add(card);
            Services.EventManager.Fire(new CardOffered(card, i));
        }
        Services.EventManager.Register<CardOfferSelected>(OnOfferSelected);
    }

    private void OnOfferSelected(CardOfferSelected e)
    {
        foreach(Card card in offeredCards)
        {
            if (card != e.card)
            {
                card.Destroy();
            }
        }
        offeredCards.Clear();
        Services.EventManager.Unregister<CardOfferSelected>(OnOfferSelected);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CardOffered : GameEvent
{
    public readonly Card card;
    public readonly int offerOrder;
    public CardOffered(Card card_, int offerOrder_)
    {
        card = card_;
        offerOrder = offerOrder_;
    }
}
