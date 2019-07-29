using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardDisplayer 
{
    private Transform cardHolder;
    private List<CardRenderer> cardRenderers;
    private CardRenderer cardRendererPrefab;
    private Queue<CardEvent> animationQueue;
    private bool animating;
    private float pauseTimeRemaining;

    public CardDisplayer()
    {
        cardHolder = new GameObject("Card Holder").transform;
        cardRenderers = new List<CardRenderer>();
        cardRendererPrefab = Resources.Load<CardRenderer>("Prefabs/CardRenderer");
        animationQueue = new Queue<CardEvent>();
        Services.EventManager.Register<CardCreated>(OnCardCreated);
        Services.EventManager.Register<CardEventQueued>(QueueCardEvent);
        Services.EventManager.Register<CardAnimationComplete>(OnCardAnimationComplete);
        // temp
        Services.EventManager.Register<CardAnimationPause>(ResolvePause);
    }

    public void ResolvePause(CardAnimationPause e)
    {
        pauseTimeRemaining = e.pauseTime;
    }

    public void OnCardAnimationComplete(CardAnimationComplete e)
    {
        animating = false;
    }

    private void QueueCardEvent(CardEventQueued e)
    {
        animationQueue.Enqueue(e.cardEvent);
    }

    public void Update()
    {
        ProcessAnimationQueue();
    }

    public void OnCardCreated(CardCreated e)
    {
        CardRenderer cardRenderer = GameObject.Instantiate(cardRendererPrefab, cardHolder).
            GetComponent<CardRenderer>();
        cardRenderers.Add(cardRenderer);
        cardRenderer.Init(e.card, cardHolder);
    }

    private void ProcessAnimationQueue()
    {
        if(pauseTimeRemaining > 0)
        {
            pauseTimeRemaining -= Time.deltaTime;
            if(pauseTimeRemaining <= 0)
            {
                Services.EventManager.Fire(new CardAnimationComplete());
            }
            return;
        }
        if (animationQueue.Count > 0 && !animating)
        {
            CardEvent cardEvent = animationQueue.Dequeue();
            animating = true;
            Services.EventManager.Fire(cardEvent);
            //Debug.Log("firing " + cardEvent.GetType());
        }
    }
}

public class CardAnimationComplete : GameEvent
{
}

public class CardAnimationPause : CardEvent {
    public readonly float pauseTime;
    public CardAnimationPause(float pauseTime_)
    {
        pauseTime = pauseTime_;
    }
}