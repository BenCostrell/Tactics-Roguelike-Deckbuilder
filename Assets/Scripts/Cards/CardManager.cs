using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardManager
{
    private List<Card> fullDeck;
    private List<Card> currentDeck;
    private List<Card> discardPile;
    private List<Card> hand;

    private CardDisplayer cardDisplayer;

    public int handCount { get { return hand.Count; } }

    private const int marginalCardsPerTurn = 3;
    private const int maxStartingCards = 6;

    public CardManager()
    {
        fullDeck = new List<Card>();
        currentDeck = new List<Card>();
        discardPile = new List<Card>();
        hand = new List<Card>();

        cardDisplayer = new CardDisplayer();

        Services.EventManager.Register<CardCast>(OnCardCast);
        Services.EventManager.Register<PlayerTurnEnded>(OnPlayerTurnEnded);
        // for testing, creating starting deck
        CardData whackData = Services.CardDataManager.GetData("Whack");
        for (int i = 0; i < 10; i++)
        {
            Card whackCard = new Card(whackData);
            fullDeck.Add(whackCard);
        }
        //
    }

    public void Update()
    {
        cardDisplayer.Update();
    }

    public void OnLevelStart()
    {
        currentDeck = new List<Card>(fullDeck);
        DrawNewHand();
    }

    public void AcquireCard(Card card)
    {
        DiscardCard(card);
    }

    public void DrawCard()
    {
        if (currentDeck.Count == 0)
        {
            currentDeck = discardPile;
            Services.EventManager.Fire(new DeckReshuffled());
        }
        if (currentDeck.Count == 0)
        {
            return;
        }
        Card card = currentDeck[Random.Range(0, currentDeck.Count)];
        currentDeck.Remove(card);
        hand.Add(card);
        Services.EventManager.Fire(new CardDrawn(card));
    }

    public void OnCardCast(CardCast e)
    {
        hand.Remove(e.card); 
        discardPile.Add(e.card);
    }

    public void OnPlayerTurnEnded(PlayerTurnEnded e)
    {
        DrawNewHand();
    }

    public void DrawNewHand()
    {
        int newHandSize = Mathf.Min(maxStartingCards, hand.Count + marginalCardsPerTurn);
        foreach (Card card in hand)
        {
            DiscardCard(card);
        }
        hand.Clear();
        for (int i = 0; i < newHandSize; i++)
        {
            DrawCard();
            //Debug.Log("drawing "+ (i + 1) + "th card");
        }
    }

    public int GetHandIndex(int id)
    {
        for (int i = 0; i < hand.Count; i++)
        {
            Card card = hand[i];
            if(card.id == id)
            {
                return i;
            }
        }
        return -1;
    }

    private void DiscardCard(Card card)
    {
        discardPile.Add(card);
        Services.EventManager.Fire(new CardDiscarded(card));
    }
}

public class CardDrawn : GameEvent
{
    public readonly Card card;

    public CardDrawn(Card card_)
    {
        card = card_;
    }
}

public class DeckReshuffled : GameEvent { }

public class CardDiscarded : GameEvent
{
    public readonly Card card;
    public CardDiscarded(Card card_)
    {
        card = card_;
    }
}
