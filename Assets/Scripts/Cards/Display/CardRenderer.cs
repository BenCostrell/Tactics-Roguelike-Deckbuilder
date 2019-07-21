using TMPro;
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
    private static Vector3 handSpacing = new Vector3(1.25f, -0.2f, 0.1f);
    private const float handSpreadAngle = 5;
    public SortingGroup sortingGroup;
    private Vector3 baseScale;
    private const float hoverScaleFactor = 1.2f;
    private const float hoverHeight = 0.3f;


    public void Init(Card card, Transform cardHolder)
    {
        id = card.id;
        cardImage.sprite = card.data.sprite;
        cardName.text = card.data.name;
        cardText.text = card.data.text;
        stateMachine = new StateMachine<CardRenderer>(this);
        stateMachine.InitializeState<Inactive>();
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

    private Vector3 GetHandPos()
    {
        float middleOffset = GetMiddleOffset();
        return handBasePos
            + (middleOffset * handSpacing.x * Vector3.right)
            + (Mathf.Abs(middleOffset) * handSpacing.y * Vector3.up)
            + (middleOffset * handSpacing.z * Vector3.forward);
    }

    private Quaternion GetHandRot()
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

public class CardState : StateMachine<CardRenderer>.State { }

public class Inactive : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.displayHolder.SetActive(false);
        Services.EventManager.Register<CardDrawn>(OnCardDrawn);
    }

    public void OnCardDrawn(CardDrawn e)
    {
        if (e.card.id != Context.id) return;
        TransitionTo<BeingDrawn>();
    }

    public override void OnExit()
    {
        base.OnExit();
        Context.displayHolder.SetActive(true);
        Services.EventManager.Unregister<CardDrawn>(OnCardDrawn);
    }
}

public class BeingDrawn : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        // for now just put it straight in place
        Context.SetHandPos();
        TransitionTo<InHand>();
    }
}

public class InHand : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.SetHoverStatus(false);
        Services.EventManager.Register<CardRendererHover>(OnCardRendererHover);
    }

    public void OnCardRendererHover(CardRendererHover e)
    {
        if (e.id != Context.id) return;
        TransitionTo<Hovered>();
    }

    public override void OnExit()
    {
        base.OnExit();

        Services.EventManager.Unregister<CardRendererHover>(OnCardRendererHover);
    }
}

public class Hovered : CardState
{
    public override void OnEnter()
    {
        base.OnEnter();
        Context.SetHoverStatus(true);
        Services.EventManager.Register<CardRendererHover>(OnCardRendererHover);
    }

    public void OnCardRendererHover(CardRendererHover e)
    {
        if (e.id != Context.id)
        {
            TransitionTo<InHand>();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        Services.EventManager.Unregister<CardRendererHover>(OnCardRendererHover);
    }
}
