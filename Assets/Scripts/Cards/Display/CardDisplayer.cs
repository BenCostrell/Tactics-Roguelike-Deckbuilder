using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CardDisplayer 
{
    private Transform cardHolder;
    private List<CardRenderer> cardRenderers;
    private CardRenderer cardRendererPrefab;
    private Queue<CardEventQueued> animationQueue;
    private bool waitingForAnimation;
    private bool framePause;

    public CardDisplayer()
    {
        cardHolder = new GameObject("Card Holder").transform;
        cardRenderers = new List<CardRenderer>();
        cardRendererPrefab = Resources.Load<CardRenderer>("Prefabs/CardRenderer");
        animationQueue = new Queue<CardEventQueued>();
        Services.EventManager.Register<CardCreated>(OnCardCreated);
        Services.EventManager.Register<CardEventQueued>(QueueCardEvent);
        Services.EventManager.Register<CardAnimationComplete>(OnCardAnimationComplete);
        waitingForAnimation = false;
    }

    public void OnCardAnimationComplete(CardAnimationComplete e)
    {
        waitingForAnimation = false;
    }

    private void QueueCardEvent(CardEventQueued e)
    {
        animationQueue.Enqueue(e);
        //Debug.Log("queuing " + e.cardEvent.GetType());
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
        if(animationQueue.Count > 0 && !waitingForAnimation)
        {
            // this bullshit is because the card renderer's statemachine doesn't actually change 
            // until the next frame, so it's not registered for the event soon enough
            // probably should think of a smarter solution.
            if (framePause)
            {
                framePause = false;
            }
            else
            {
                CardEventQueued queuedCardEvent = animationQueue.Dequeue();
                waitingForAnimation = true;
                framePause = true;
                Services.EventManager.Fire(new StartCardAnimation(queuedCardEvent.cardEvent,
                    queuedCardEvent.id));
                Services.EventManager.Fire(queuedCardEvent.cardEvent);
                //Debug.Log("firing " + queuedCardEvent.cardEvent.GetType() + " at time " +Time.time);
            }
        }
    }
}

public class CardAnimationComplete : GameEvent
{
}