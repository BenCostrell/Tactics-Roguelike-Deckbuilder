﻿using UnityEngine;
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
        Debug.Log("drawing from deck of size " + currentDeck.Count);
        if (currentDeck.Count == 0)
        {
            currentDeck = new List<Card>(discardPile);
            DeckReshuffled deckReshuffledEvent = new DeckReshuffled(currentDeck.Count);
            Services.EventManager.Fire(new CardEventQueued(deckReshuffledEvent, -1));
            discardPile.Clear();
            Debug.Log("deck count: " + currentDeck.Count);
            Debug.Log("discard count: " + discardPile.Count);
        }
        if (currentDeck.Count == 0)
        {
            return;
        }
        Card card = currentDeck[Random.Range(0, currentDeck.Count)];
        currentDeck.Remove(card);
        hand.Add(card);
        CardDrawn cardDrawnEvent = new CardDrawn(card, currentDeck.Count);
        Services.EventManager.Fire(new CardEventQueued(cardDrawnEvent, card.id));
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
        CardDiscarded cardDiscardedEvent = new CardDiscarded(card, discardPile.Count);
        Services.EventManager.Fire(new CardEventQueued(cardDiscardedEvent, card.id));
    }
}

public class CardEventQueued : GameEvent
{
    public readonly CardEvent cardEvent;
    public readonly int id;

    public CardEventQueued(CardEvent cardEvent_, int id_ = -1)
    {
        cardEvent = cardEvent_;
        id = id_;
    }
}

public abstract class CardEvent : GameEvent { }

public class CardDrawn : CardEvent
{
    public readonly Card card;
    public readonly int deckCount;

    public CardDrawn(Card card_, int deckCount_)
    {
        card = card_;
        deckCount = deckCount_;
    }
}

public class DeckReshuffled : CardEvent
{
    public readonly int deckCount;

    public DeckReshuffled(int deckCount_)
    {
        deckCount = deckCount_;
    }
}

public class CardDiscarded : CardEvent
{
    public readonly Card card;
    public readonly int discardPileCount;

    public CardDiscarded(Card card_, int discardPileCount_)
    {
        card = card_;
        discardPileCount = discardPileCount_;
    }
}