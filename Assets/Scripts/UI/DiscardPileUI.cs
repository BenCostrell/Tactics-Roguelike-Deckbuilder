using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscardPileUI : MonoBehaviour
{
    public TextMeshPro countText;
    public Transform frame;
    private float deckReshuffledTimeRemaining;
    private int count;
    private int startCount;
    public static Transform frameTransform;

    // Start is called before the first frame update
    void Start()
    {
        Services.EventManager.Register<CardDiscarded>(OnCardDiscarded);
        Services.EventManager.Register<DeckReshuffled>(OnDeckReshuffled);
        frameTransform = frame;
    }

    // Update is called once per frame
    void Update()
    {
        ReshuffleCountDown();
    }


    public void OnDeckReshuffled(DeckReshuffled e)
    {
        deckReshuffledTimeRemaining = DeckCountUI.deckReshuffledCountUpTime;
        startCount = e.deckCount;
    }

    private void ReshuffleCountDown()
    {
        if (deckReshuffledTimeRemaining > 0)
        {
            deckReshuffledTimeRemaining -= Time.deltaTime;
            count = Mathf.RoundToInt(Mathf.Lerp(0, startCount,
                EasingEquations.Easing.Linear(deckReshuffledTimeRemaining / DeckCountUI.deckReshuffledCountUpTime)));
            countText.text = count.ToString();
            if (deckReshuffledTimeRemaining <= 0)
            {
                Services.EventManager.Fire(new CardAnimationComplete());
            }
        }
    }

    public void OnCardDiscarded(CardDiscarded e)
    {
        countText.text = e.discardPileCount.ToString();
    }
}
