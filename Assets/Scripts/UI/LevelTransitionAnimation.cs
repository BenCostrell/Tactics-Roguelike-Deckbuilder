using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTransitionAnimation : MonoBehaviour
{
    private RectTransform rect;
    private const float transitionDuration = 1f;
    private const float leadOutDuration = 1f;
    private float startSize;
    private float transitionTimeRemaining;

    
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        startSize = rect.sizeDelta.x;
        Services.EventManager.Register<StartLevelTransitionAnimation>(StartAnimation);
    }

    private void StartAnimation(StartLevelTransitionAnimation e)
    {
        transitionTimeRemaining = transitionDuration + leadOutDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (transitionTimeRemaining > 0)
        {
            transitionTimeRemaining -= Time.deltaTime;
            if (transitionTimeRemaining >= leadOutDuration)
            {
                rect.sizeDelta = Vector2.Lerp(startSize * Vector2.one, Vector2.zero,
                    EasingEquations.Easing.QuadEaseOut(
                        1 - ((transitionTimeRemaining - leadOutDuration) / transitionDuration)));
            }

            if (transitionTimeRemaining <= 0)
            {
                Services.EventManager.Fire(new TransitionAnimationComplete());
            }
        }
    }
}

public class TransitionAnimationComplete: GameEvent { }
