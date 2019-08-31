using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CardRenderer hoveredCard;
    private TileRenderer hoveredTile;
    private SpriteButton hoveredButton;
    private bool cardSelected;

    // Start is called before the first frame update
    void Start()
    {
        Services.EventManager.Register<CardSelectionStatusChange>(OnCardSelectionStatusChange);
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        hoveredCard = null;
        hoveredTile = null;
        hoveredButton = null;
        if (hit.collider != null)
        {
            CardRenderer cardRendererHit = hit.transform.GetComponentInParent<CardRenderer>();
            if (cardRendererHit != null)
            {
                hoveredCard = cardRendererHit;
            }
            TileRenderer tileRendererHit = hit.transform.GetComponent<TileRenderer>();
            if (tileRendererHit != null)
            {
                hoveredTile = tileRendererHit;
            }
            SpriteButton spriteButtonHit = hit.transform.GetComponent<SpriteButton>();
            if (spriteButtonHit != null)
            {
                hoveredButton = spriteButtonHit;
            }
        }
        Services.EventManager.Fire(new InputHover(mouseWorldPos, hoveredCard, hoveredTile, hoveredButton,
            cardSelected));
        if (Input.GetMouseButtonUp(0))
        {
            Services.EventManager.Fire(new InputUp());
        }
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                Services.EventManager.Fire(new InputDown(i, hoveredCard, hoveredTile,
                    hoveredButton, cardSelected));
            }
        }
    }
    
    private void OnCardSelectionStatusChange(CardSelectionStatusChange e)
    {
        cardSelected = e.cardSelected;
    }
}

public class InputHover : GameEvent
{
    public readonly Vector3 worldPos;
    public readonly CardRenderer hoveredCard;
    public readonly TileRenderer hoveredTile;
    public readonly SpriteButton hoveredButton;
    public readonly bool cardSelected;

    public InputHover(Vector3 worldPos_, CardRenderer hoveredCard_, TileRenderer hoveredTile_, 
        SpriteButton hoveredButton_, bool cardSelected_)
    {
        worldPos = worldPos_;
        hoveredCard = hoveredCard_;
        hoveredTile = hoveredTile_;
        hoveredButton = hoveredButton_;
        cardSelected = cardSelected_;
    }
}

public class InputDown : GameEvent
{
    public readonly CardRenderer hoveredCard;
    public readonly TileRenderer hoveredTile;
    public readonly SpriteButton hoveredButton;
    public readonly int buttonNum;
    public readonly bool cardSelected;

    public InputDown(int buttonNum_, CardRenderer hoveredCard_, TileRenderer hoveredTile_, 
        SpriteButton hoveredButton_, bool cardSelected_)
    {
        buttonNum = buttonNum_;
        hoveredCard = hoveredCard_;
        hoveredTile = hoveredTile_;
        cardSelected = cardSelected_;
        hoveredButton = hoveredButton_;
    }
}

public class InputUp : GameEvent
{
    public InputUp()
    {
        
    }
}