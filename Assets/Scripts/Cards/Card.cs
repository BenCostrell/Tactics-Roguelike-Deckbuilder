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
    public int minRange
    {
        get
        {
            foreach (CardEffect cardEffect in effects)
            {
                if (cardEffect.targeted) return cardEffect.minRange;
            }
            return 0;
        }
    }
    public int maxRange
    {
        get
        {
            foreach (CardEffect cardEffect in effects)
            {
                if (cardEffect.targeted) return cardEffect.maxRange;
            }
            return 0;
        }
    }

    public Card(CardData data_)
    {
        data = data_;
        bonusEffects = new List<CardEffect>();
        id = nextId;
        Services.EventManager.Fire(new CardCreated(this));
    }

    public bool IsTargetLegal(MapTile target)
    {
        foreach(CardEffect cardEffect in effects)
        {
            if (!cardEffect.IsTargetLegal(target)) return false;
        }
        return true;
    }

    public void Cast(MapTile target)
    {
        Services.EventManager.Fire(new CardCast(this, target));
        foreach(CardEffect effect in effects)
        {
            effect.Execute(target);
        }
    }

    public bool IsCastable(MapTile target)
    {
        if (cost > Services.LevelManager.player.currentEnergy) return false;
        if (!IsTargetLegal(target)) return false;
        return true;
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
