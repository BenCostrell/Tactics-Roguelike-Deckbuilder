﻿using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardRenderer : MonoBehaviour
{
    public SpriteRenderer cardImage;
    public SpriteRenderer cardFrame;
    public TextMeshPro cardName;
    public TextMeshPro cardText;
    public GameObject displayHolder;
    public int id { get; private set; }
    private StateMachine<CardRenderer> stateMachine;
    private static Vector3 handBasePos = new Vector3(0, -4f,0);
    private static Vector3 handSpacing = new Vector3(1.25f, -0.1f, 0.1f);
    private const float handSpreadAngle = 2.5f;
    public SortingGroup sortingGroup;
    private Vector3 baseScale;
    private const float hoverScaleFactor = 1.2f;
    private const float hoverHeight = 0.3f;
    public bool animating
    {
        get
        {
            return stateMachine.currentState is Animating;
        }
    }


    public void Init(Card card, Transform cardHolder)
    {
        id = card.id;
        cardImage.sprite = card.data.sprite;
        cardName.text = card.data.name;
        cardText.text = card.data.text;
        stateMachine = new StateMachine<CardRenderer>(this);
        stateMachine.InitializeState<Inactive>();
        transform.parent = cardHolder;
        baseScale = transform.localScale;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public Vector3 GetHandPos()
    {
        float middleOffset = GetMiddleOffset();
        return handBasePos
            + (middleOffset * handSpacing.x * Vector3.right)
            + (Mathf.Abs(middleOffset) * handSpacing.y * Vector3.up)
            + (middleOffset * handSpacing.z * Vector3.forward);
    }

    public Quaternion GetHandRot()
    {
        Quaternion rotation =  Quaternion.Euler(0, 0, -handSpreadAngle * GetMiddleOffset());
        return rotation;
    }

    private float GetMiddleOffset()
    {
        int handIndex = Services.CardManager.GetHandIndex(id);
        int handCount = Services.CardManager.handCount;
        int middleIndex = handCount / 2;
        float middleOffset = handIndex - middleIndex;
        if (handCount % 2 == 0)
        {
            middleOffset += 0.5f;
        }
        return middleOffset;
    }

    public void SetHandPos()
    {
        transform.localPosition = GetHandPos();
        sortingGroup.sortingOrder = Services.CardManager.GetHandIndex(id);
        transform.localRotation = GetHandRot();
    }

    public void SetHoverStatus(bool hovered)
    {
        if (hovered)
        {
            transform.localScale = baseScale * hoverScaleFactor;
            sortingGroup.sortingOrder = 10;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = new Vector3(transform.localPosition.x, handBasePos.y + hoverHeight, 
                transform.localPosition.z);
        }
        else
        {
            transform.localScale = baseScale;
            SetHandPos();
        }
    }
}

public abstract class CardState : StateMachine<CardRenderer>.State { }

public class Inactive : WaitingToAnimate
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.displayHolder.SetActive(false);
    }

    public override void OnAnimationStart(StartCardAnimation e)
    {
        base.OnAnimationStart(e);
        if (e.id != Context.id) return;
        TransitionTo<BeingDrawn>();
        Debug.Log("card " + e.id + " being drawn");
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.displayHolder.SetActive(true);
    }
}

public abstract class Animating : CardState { }

public abstract class WaitingToAnimate : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<StartCardAnimation>(OnAnimationStart);
        //Debug.Log("waiting to animate");
    }

    public virtual void OnAnimationStart(StartCardAnimation e)
    {
        if (e.id != Context.id) return;
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<StartCardAnimation>(OnAnimationStart);
    }
}

public class BeingDrawn : Animating
{
    private const float drawAnimationDuration = 0.3f;
    private const float staggerTime = 0.1f;
    private float timeElapsed;
    private Vector3 targetScale;
    private Vector3 targetPos;
    private Quaternion targetRot;
    private Vector3 startPos;
    private bool staggerFired;

    public override void OnEnter()
    {
        base.OnEnter();
        targetScale = Context.transform.localScale;
        targetRot = Context.GetHandRot();
        targetPos = Context.GetHandPos();
        Context.transform.position = DeckCountUI.frameTransform.position;
        startPos = Context.transform.localPosition;
        Context.transform.localScale = Vector3.one;
        timeElapsed = 0;
        staggerFired = false;
    }

    public override void Update()
    {
        base.Update();
        timeElapsed += Time.deltaTime;
        Context.transform.localScale = Vector3.Lerp(Vector3.one, targetScale,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / drawAnimationDuration));
        Context.transform.localPosition = Vector3.Lerp(startPos, targetPos,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / drawAnimationDuration));
        Context.transform.localRotation = Quaternion.Lerp(Quaternion.identity, targetRot,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / drawAnimationDuration));
        if (timeElapsed > staggerTime && !staggerFired)
        {
            staggerFired = true;
            Services.EventManager.Fire(new CardAnimationComplete());
        }
        if (timeElapsed > drawAnimationDuration)
        {
            TransitionTo<Unhovered>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.SetHandPos();
    }
}

public abstract class InHand : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<CardRendererHover>(OnCardRendererHover);
        Services.EventManager.Register<CardEventQueued>(OnCardEventQueued);
    }

    public void OnCardEventQueued(CardEventQueued e)
    {
        if (e.id != Context.id) return;
        if(e.cardEvent is CardDiscarded)
        {
            TransitionTo<WaitingToBeDiscarded>();
        }
        else if(e.cardEvent is CardCast)
        {
            TransitionTo<Inactive>();
        }
    }

    public virtual void OnCardRendererHover(CardRendererHover e)
    {
        
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<CardEventQueued>(OnCardEventQueued);
        Services.EventManager.Unregister<CardRendererHover>(OnCardRendererHover);
    }
}

public class WaitingToBeDiscarded : WaitingToAnimate
{
    public override void OnAnimationStart(StartCardAnimation e)
    {
        base.OnAnimationStart(e);
        if (e.id != Context.id) return;
        TransitionTo<BeingDiscarded>();
    }
}

public class Unhovered : InHand
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.SetHoverStatus(false);
    }

    public override void OnCardRendererHover(CardRendererHover e)
    {
        base.OnCardRendererHover(e);
        if (e.id != Context.id) return;
        TransitionTo<Hovered>();
    }
}

public class Hovered : InHand
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.SetHoverStatus(true);
    }

    public override void OnCardRendererHover(CardRendererHover e)
    {
        if (e.id != Context.id)
        {
            TransitionTo<Unhovered>();
        }
    }
}

public class BeingDiscarded : Animating
{
    private const float discardAnimationDuration = 0.3f;
    private const float staggerTime = 0.1f;
    private bool staggerFired;
    private float timeElapsed;
    private Vector3 startScale;
    private Vector3 startPos;
    private Quaternion startRot;
    private Transform cardHolder;

    public override void OnEnter()
    {
        base.OnEnter();
        cardHolder = Context.transform.parent;
        Context.transform.parent = DiscardPileUI.frameTransform;
        startPos = Context.transform.localPosition;
        startRot = Context.transform.localRotation;
        timeElapsed = 0;
        staggerFired = false;
    }

    public override void Update()
    {
        base.Update();
        timeElapsed += Time.deltaTime;
        Context.transform.localScale = Vector3.Lerp(startScale, Vector3.one,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / discardAnimationDuration));
        Context.transform.localPosition = Vector3.Lerp(startPos, Vector3.zero,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / discardAnimationDuration));
        Context.transform.localRotation = Quaternion.Lerp(startRot, Quaternion.identity,
            EasingEquations.Easing.QuadEaseOut(timeElapsed / discardAnimationDuration));
        if(timeElapsed > staggerTime && !staggerFired)
        {
            staggerFired = true;
            Services.EventManager.Fire(new CardAnimationComplete());
        }
        if (timeElapsed > discardAnimationDuration)
        {
            TransitionTo<Inactive>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.transform.parent = cardHolder;
    }
}

public class StartCardAnimation : GameEvent
{
    public CardEvent cardEvent;
    public readonly int id;
    public StartCardAnimation(CardEvent cardEvent_, int id_)
    {
        cardEvent = cardEvent_;
        id = id_;
    }
}
