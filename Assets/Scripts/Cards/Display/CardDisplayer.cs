using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardDisplayer 
{
    private Transform cardHolder;
    private List<CardRenderer> cardRenderers;
    private CardRenderer cardRendererPrefab;

    public CardDisplayer()
    {
        cardHolder = new GameObject("Card Holder").transform;
        cardRenderers = new List<CardRenderer>();
        cardRendererPrefab = Resources.Load<CardRenderer>("Prefabs/CardRenderer");
        Services.EventManager.Register<CardCreated>(OnCardCreated);
    }

    public void OnCardCreated(CardCreated e)
    {
        CardRenderer cardRenderer = GameObject.Instantiate(cardRendererPrefab, cardHolder).
            GetComponent<CardRenderer>();
        cardRenderers.Add(cardRenderer);
        cardRenderer.Init(e.card, cardHolder);
    }
}
