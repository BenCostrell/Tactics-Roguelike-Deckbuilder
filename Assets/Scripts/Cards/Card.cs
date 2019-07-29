using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card 
{
    public int cost { get { return data.cost + costModifier; } }
    public readonly CardData data;
    private int costModifier;
    private List<CardEffect> effects
    {
        get
        {
            List<CardEffect> allEffects = new List<CardEffect>(data.effects);
            allEffects.AddRange(bonusEffects);
            return allEffects;
        }
    }
    private readonly List<CardEffect> bonusEffects;
    public int id { get; private set; }
    private static int nextId
    {
        get
        {
            nextId_ += 1;
            return nextId_;
        }
    }
    private static int nextId_;
    

    public Card(CardData data_)
    {
        data = data_;
        bonusEffects = new List<CardEffect>();
        id = nextId;
        Services.EventManager.Fire(new CardCreated(this));
    }

    public void Cast(MapTile target)
    {
        Services.EventManager.Fire(new CardCast(this, target));
        foreach(CardEffect effect in effects)
        {
            effect.Execute(target);
        }
    }
}

public class CardCast : CardEvent
{
    public readonly Card card;
    public readonly MapTile target;

    public CardCast(Card card_, MapTile target_)
    {
        card = card_;
        target = target_;
    }
}

public class CardCreated : GameEvent
{
    public readonly Card card;
    public CardCreated(Card card_)
    {
        card = card_;
    }
}
