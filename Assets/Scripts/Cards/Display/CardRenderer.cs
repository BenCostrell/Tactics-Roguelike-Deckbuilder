using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardRenderer : MonoBehaviour
{
    public SpriteRenderer cardImage;
    public SpriteRenderer cardFrame;
    public TextMeshPro cardName;
    public TextMeshPro cardText;
    public TextMeshPro costText;
    public GameObject displayHolder;
    public DottedLine targetLine;
    public SpriteRenderer highlight;
    private BoxCollider2D col;
    public int id { get; private set; }
    public Card card { get; private set; }
    private StateMachine<CardRenderer> stateMachine;
    private static Vector3 handBasePos = new Vector3(0, -4f,0);
    private static Vector3 handSpacing = new Vector3(1.25f, -0.1f, 0.1f);
    public static Vector3 handScale = new Vector3(1.5f, 1.5f, 1.0f);
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
    public MapTile currentTarget;
    public Transform cardHolder { get; private set; }
    public int offerOrder;

    public void Init(Card card_, Transform parent, Transform cardHolder_)
    {
        card = card_;
        id = card.id;
        cardImage.sprite = card.data.sprite;
        cardName.text = card.data.name;
        cardText.text = card.data.text;
        costText.text = card.data.cost.ToString();
        col = GetComponentInChildren<BoxCollider2D>();
        stateMachine = new StateMachine<CardRenderer>(this);
        stateMachine.InitializeState<Inactive>();
        cardHolder = cardHolder_;
        transform.parent = parent;
        baseScale = transform.localScale;
        targetLine.gameObject.SetActive(false);
        highlight.enabled = false;
        Services.EventManager.Register<CardDestroyed>(OnCardDestroyed);
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
        transform.localRotation = GetHandRot();
        SetSortingOrder();
    }

    public void SetSortingOrder()
    {
        sortingGroup.sortingOrder = Services.CardManager.GetHandIndex(id);
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

    public void SetAnimationSortingStatus(bool top)
    {
        sortingGroup.sortingLayerName = top ? "TopUI" : "UI";
    }


    public void SetArrowStatus(bool status, Vector3 start, Vector3 target)
    {
        col.enabled = !status;
        targetLine.gameObject.SetActive(status);
        targetLine.SetTarget(start, target);
        foreach(Transform child in displayHolder.GetComponentInChildren<Transform>())
        {
            if(child.gameObject != displayHolder && child.gameObject != cardImage.gameObject)
            {
                child.gameObject.SetActive(!status);
            }
        }
    }

    public void SetHighlight(bool status, Color color)
    {
        highlight.enabled = status;
        highlight.color = color;
    }

    private void OnCardDestroyed(CardDestroyed e)
    {
        if (e.card == card)
        {
            stateMachine.currentState.OnExit();
            Services.EventManager.Unregister<CardDestroyed>(OnCardDestroyed);
            Destroy(gameObject);
        }
    }
}

public abstract class CardState : StateMachine<CardRenderer>.State {

}

public class Inactive : WaitingToAnimate
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.displayHolder.SetActive(false);
        Services.EventManager.Register<CardOffered>(OnCardOffered);
    }

    private void OnCardOffered(CardOffered e)
    {
        if (e.card != Context.card) return;
        TransitionTo<Offered>();
        Context.offerOrder = e.offerOrder;
    }

    public override void OnAnimationStart(StartCardAnimation e)
    {
        base.OnAnimationStart(e);
        if (e.id != Context.id) return;
        TransitionTo<BeingDrawn>();
        //Debug.Log("card " + e.id + " being drawn");
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.displayHolder.SetActive(true);
        Services.EventManager.Unregister<CardOffered>(OnCardOffered);
    }
}

public class Offered : CardState
{
    private readonly float offeredScale = 2;
    private readonly float offerSpacing = 3.2f;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.transform.localScale = offeredScale * Vector3.one;
        Context.SetAnimationSortingStatus(true);
        Context.transform.localPosition = (Context.offerOrder - 1) * offerSpacing * Vector3.right;
        Services.EventManager.Register<InputHover>(OnInputHover);
        Services.EventManager.Register<InputDown>(OnInputDown);
    }

    private void OnInputHover(InputHover e)
    {
        if (e.hoveredCard == Context)
        {
            Context.SetHighlight(true, Color.green);
        }
        else
        {
            Context.SetHighlight(false, Color.white);
        }
    }

    private void OnInputDown(InputDown e)
    {
        if (e.hoveredCard != Context) return;
        Context.transform.parent = Context.cardHolder;
        Services.EventManager.Fire(new CardOfferSelected(Context.card));
        Debug.Log(Context.card.data.name + " selected");

        TransitionTo<BeingDiscarded>();
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<InputDown>(OnInputDown);
        Services.EventManager.Unregister<InputHover>(OnInputHover);
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
    private const float drawAnimationDuration = 0.4f;
    private const float staggerTime = 0.15f;
    private float timeElapsed;
    private Vector3 targetScale;
    private Vector3 targetPos;
    private Quaternion targetRot;
    private Vector3 startPos;
    private bool staggerFired;

    public override void OnEnter()
    {
        base.OnEnter();
        targetScale = CardRenderer.handScale;
        targetRot = Context.GetHandRot();
        targetPos = Context.GetHandPos();
        Context.transform.position = DeckCountUI.frameTransform.position;
        startPos = Context.transform.localPosition;
        Context.transform.localScale = Vector3.one;
        timeElapsed = 0;
        staggerFired = false;
        Context.SetAnimationSortingStatus(true);
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
        Context.SetAnimationSortingStatus(false);
        Context.SetHandPos();
    }
}

public abstract class InHand : CardState
{
    private Vector3 targetHandPos;
    private Quaternion targetHandRot;
    private const float adjustDuration = 0.5f;
    private float adjustTimeRemaining;
    private Vector3 prevHandPos;
    private Quaternion prevHandRot;

    public override void OnEnter()
    {
        base.OnEnter();
        Services.EventManager.Register<InputHover>(OnInputHover);
        Services.EventManager.Register<CardEventQueued>(OnCardEventQueued);
        Services.EventManager.Register<CardDiscarded>(OnCardDiscarded);
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
        prevHandRot = Context.transform.localRotation;
        prevHandPos = Context.transform.localPosition;
        adjustTimeRemaining = 0;
        SetEnergyHighlight();
        //Debug.Log("card " + Context.id + " entering an in hand state");
    }

    public override void Update()
    {
        base.Update();
        if (adjustTimeRemaining > 0)
        {
            adjustTimeRemaining -= Time.deltaTime;
            float progress = EasingEquations.Easing
                .QuadEaseOut(1 - (adjustTimeRemaining / adjustDuration));
            Context.transform.localRotation = Quaternion.Lerp(prevHandRot, targetHandRot, progress);
            Context.transform.localPosition = Vector3.Lerp(prevHandPos, targetHandPos, progress);
        }
    }

    public void OnCardDiscarded(CardDiscarded e)
    {
        targetHandPos = Context.GetHandPos();
        targetHandRot = Context.GetHandRot();
        adjustTimeRemaining = adjustDuration;
        Context.SetSortingOrder();
    }

    public void OnCardEventQueued(CardEventQueued e)
    {
        if (e.id != Context.id) return;
        if (e.cardEvent is CardDiscarded)
        {
            TransitionTo<WaitingToBeDiscarded>();
        }
    }

    public virtual void OnInputHover(InputHover e)
    {
        
    }

    private void OnEnergyChanged(EnergyChanged e)
    {
        SetEnergyHighlight();
    }

    protected void SetEnergyHighlight()
    {
        if (Services.LevelManager.player.currentEnergy >= Context.card.cost)
        {
            Context.SetHighlight(true, Color.green);
        }
        else
        {
            Context.SetHighlight(false, Color.white);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<CardEventQueued>(OnCardEventQueued);
        Services.EventManager.Unregister<InputHover>(OnInputHover);
        Services.EventManager.Unregister<CardDiscarded>(OnCardDiscarded);
        Services.EventManager.Unregister<EnergyChanged>(OnEnergyChanged);
        Context.SetHighlight(false, Color.white);
        //Debug.Log("card " + Context.id + " exiting an in hand state");
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

    public override void OnInputHover(InputHover e)
    {
        base.OnInputHover(e);
        if (e.hoveredCard != Context) return;
        if (e.cardSelected) return;
        TransitionTo<Hovered>();
    }
}

public class Hovered : InHand
{
    private bool otherCardHovered;

    public override void OnEnter()
    {
        base.OnEnter();
        Context.SetHoverStatus(true);
        Services.EventManager.Register<InputDown>(OnInputDown);
        Services.EventManager.Fire(new CardRendererHoverStart(Context));
        otherCardHovered = false;
    }

    public override void OnInputHover(InputHover e)
    {
        base.OnInputHover(e);
        if (e.hoveredCard != Context)
        {
            TransitionTo<Unhovered>();
        }
        otherCardHovered = e.hoveredCard != null;
    }

    public virtual void OnInputDown(InputDown e)
    {
        TransitionTo<Selected>();
    }

    public override void OnExit()
    {
        base.OnExit();
        Services.EventManager.Unregister<InputDown>(OnInputDown);
        Services.EventManager.Fire(new CardRendererHoverEnd(Context, otherCardHovered));
    }
}

public class Selected : Hovered
{
    private const float maxY = -2f;
    private bool castThreshold;
    private bool targeted;
    private const float clickWindow = 0.1f;
    private float clickWindowCountdown;

    public override void OnEnter()
    {
        base.OnEnter();
        targeted = Context.card.data.targeted;
        Context.currentTarget = null;
        Context.SetAnimationSortingStatus(true);
        Services.EventManager.Register<InputUp>(OnInputUp);
        Services.EventManager.Register<InputDown>(OnInputDown);
        clickWindowCountdown = clickWindow;
        Services.EventManager.Fire(new CardSelectionStatusChange(true));
    }

    public override void Update()
    {
        base.Update();
        if (clickWindowCountdown > 0)
        {
            clickWindowCountdown -= Time.deltaTime;
        }
    }

    public override void OnInputHover(InputHover e)
    {
        if (e.hoveredTile == null)
        {
            Context.currentTarget = null;
        }
        else
        {
            Context.currentTarget = e.hoveredTile.tile;
        }
        float y;
        if (targeted)
        {
            y = Mathf.Min(e.worldPos.y, maxY);
        }
        else
        {
            y = e.worldPos.y;
        }
        castThreshold = e.worldPos.y >= maxY;
        if (!targeted)
        {
            if (castThreshold)
            {
                Context.SetHighlight(true, Color.blue);
            }
            else
            {
                SetEnergyHighlight();
            }
        }
        bool floatStop = castThreshold && targeted;
        float x = floatStop ? Context.transform.position.x : e.worldPos.x;
        Context.transform.position = new Vector3(x, y, 0f);

        if (floatStop)
        {
            Context.SetArrowStatus(true, Context.transform.position, new Vector3(e.worldPos.x, e.worldPos.y, 0));
        }
        else
        {
            Context.SetArrowStatus(false, Vector3.zero, Vector3.zero);
        }
    }

    public void OnInputUp(InputUp e)
    {
        if (clickWindowCountdown > 0) return;
        if ((!targeted && castThreshold && Context.card.IsCastable(null)) ||
            targeted && Context.card.IsCastable(Context.currentTarget))
        {
            TransitionTo<Casting>();
        }
        else
        {
            TransitionTo<Unhovered>();
        }
    }

    public override void OnInputDown(InputDown e)
    {
        if (e.buttonNum == 1)
        {
            TransitionTo<Unhovered>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.SetAnimationSortingStatus(false);
        Context.SetArrowStatus(false, Vector3.zero, Vector3.zero);
        Services.EventManager.Unregister<InputUp>(OnInputUp);
        Services.EventManager.Unregister<InputDown>(OnInputDown);
        Services.EventManager.Fire(new CardSelectionStatusChange(false));
        Context.SetHighlight(false, Color.white);
    }
}

public class Casting : InHand
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.card.Cast(Context.currentTarget);
    }
}

public class BeingDiscarded : Animating
{
    private const float discardAnimationDuration = 0.5f;
    private const float staggerTime = 0.2f;
    private bool staggerFired;
    private float timeElapsed;
    private Vector3 startScale;
    private Vector3 startPos;
    private Quaternion startRot;
    private Transform cardHolder;

    public override void OnEnter()
    {
        base.OnEnter();
        cardHolder = Context.cardHolder;
        Context.transform.parent = DiscardPileUI.frameTransform;
        startScale = Context.transform.localScale;
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

public class CardRendererHoverStart : GameEvent
{
    public readonly CardRenderer cardRenderer;
    public CardRendererHoverStart(CardRenderer cardRenderer_)
    {
        cardRenderer = cardRenderer_;
    }
}

public class CardRendererHoverEnd : GameEvent
{
    public readonly CardRenderer cardRenderer;
    public readonly bool otherCardHovered;
    public CardRendererHoverEnd(CardRenderer cardRenderer_, bool otherCardHovered_)
    {
        cardRenderer = cardRenderer_;
        otherCardHovered = otherCardHovered_;
    }
}

public class CardSelectionStatusChange : GameEvent
{
    public readonly bool cardSelected;
    public CardSelectionStatusChange(bool cardSelected_)
    {
        cardSelected = cardSelected_;
    }
}

public class CardOfferSelected: GameEvent
{
    public readonly Card card;
    public CardOfferSelected(Card card_)
    {
        card = card_;
    }
}

