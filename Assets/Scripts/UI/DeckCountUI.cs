using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckCountUI : MonoBehaviour
{
    public TextMeshPro countText;
    public Transform frame;
    public const float deckReshuffledCountUpTime = 0.5f;
    private float deckReshuffledTimeRemaining;
    private int count;
    private int targetCount;
    public static Transform frameTransform;

    // Start is called before the first frame update
    void Start()
    {
        Services.EventManager.Register<CardDrawn>(OnCardDrawn);
        Services.EventManager.Register<DeckReshuffled>(OnDeckReshuffled);
        frameTransform = frame;
    }

    // Update is called once per frame
    void Update()
    {
        ReshuffleCountUp();
    }

    public void OnDeckReshuffled(DeckReshuffled e)
    {
        deckReshuffledTimeRemaining = deckReshuffledCountUpTime;
        targetCount = e.deckCount;
        count = 0;
    }

    private void ReshuffleCountUp()
    {
        if (deckReshuffledTimeRemaining > 0)
        {
            deckReshuffledTimeRemaining -= Time.deltaTime;
            count = Mathf.RoundToInt(Mathf.Lerp(targetCount, 0,
                EasingEquations.Easing.Linear(deckReshuffledTimeRemaining / deckReshuffledCountUpTime)));
            countText.text = count.ToString();
            if (deckReshuffledTimeRemaining <= 0)
            {
                Services.EventManager.Fire(new CardAnimationComplete());
            }
        }
    }

    public void OnCardDrawn(CardDrawn e)
    {
        countText.text = e.deckCount.ToString();
    }
}
