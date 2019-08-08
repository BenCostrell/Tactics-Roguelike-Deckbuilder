using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const float cardClickWindow = 0.1f;
    private float clickWindowCountdown;
    private int selectedRendererID;
    private bool clickSelected;
    private TileRenderer hoveredTile;

    // Start is called before the first frame update
    void Start()
    {
        selectedRendererID = -1;
        clickSelected = false;
        clickWindowCountdown = 0;
        hoveredTile = null;
    }


    // Update is called once per frame
    void Update()
    {
        if (clickWindowCountdown > 0)
        {
            clickWindowCountdown -= Time.deltaTime;
        }
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Services.EventManager.Fire(new InputHover(mousePos));
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                Services.EventManager.Fire(new InputDown(mousePos, i, selectedRendererID));
                clickSelected = false;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        int id = -1;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
        {
            if (selectedRendererID != -1 && clickWindowCountdown > 0)
            {
                clickSelected = true;
            }
            Services.EventManager.Fire(new InputUp(mouseWorldPos, clickWindowCountdown > 0));
        }

        if (hit.collider != null)
        {
            CardRenderer cardRendererHit = hit.transform.GetComponentInParent<CardRenderer>();
            if (cardRendererHit != null)
            {
                id = hit.transform.GetComponentInParent<CardRenderer>().id;
                if (Input.GetMouseButtonDown(0))
                {
                    selectedRendererID = id;
                    Services.EventManager.Fire(new CardRendererSelected(id, mouseWorldPos));
                    clickWindowCountdown = cardClickWindow;
                }
            }
            TileRenderer tileRendererHit = hit.transform.GetComponent<TileRenderer>();
            if(tileRendererHit != null)
            {
                tileRendererHit.OnHover();
                hoveredTile = tileRendererHit;
                if (Input.GetMouseButtonDown(0))
                {
                    hoveredTile.OnSelected(selectedRendererID);
                }
                if (Input.GetMouseButtonUp(0) && selectedRendererID != -1)
                {
                    hoveredTile.OnSelected(selectedRendererID);
                }
            }
            else if (hit.transform.gameObject.CompareTag("Background"))
            {
                Services.EventManager.Fire(new TileHovered(null));
            }
        }
        if (!Input.GetMouseButton(0) && !clickSelected)
        {
            selectedRendererID = -1;
        }

        if (selectedRendererID == -1)
        {
            Services.EventManager.Fire(new CardRendererHover(id));
        }
        else
        {
            Services.EventManager.Fire(new CardRendererDrag(selectedRendererID, mouseWorldPos));
        }
    }
}

public class InputHover : GameEvent
{
    public readonly Vector2 worldPos;

    public InputHover(Vector2 worldPos_)
    {
        worldPos = worldPos_;
    }
}

public class InputDown : GameEvent
{
    public readonly Vector2 worldPos;
    public readonly int buttonNum;
    public readonly int selectedCardId;

    public InputDown(Vector2 worldPos_, int buttonNum_, int selectedCardId_)
    {
        worldPos = worldPos_;
        buttonNum = buttonNum_;
        selectedCardId = selectedCardId_;
    }
}

public class CardRendererSelected : GameEvent
{
    public readonly int id;
    public readonly Vector2 worldPos;
    public CardRendererSelected(int id_, Vector2 worldPos_)
    {
        id = id_;
        worldPos = worldPos_;
    }
}

public class CardRendererDrag : GameEvent
{
    public readonly int id;
    public readonly Vector2 worldPos;
    public CardRendererDrag(int id_, Vector2 worldPos_)
    {
        id = id_;
        worldPos = worldPos_;
    }
}

public class InputUp : GameEvent
{
    public readonly Vector2 worldPos;
    public readonly bool withinClickWindow;
    public InputUp(Vector2 worldPos_, bool withinClickWindow_)
    {
        worldPos = worldPos_;
        withinClickWindow = withinClickWindow_;
    }
}

public class CardRendererHover : GameEvent
{
    public readonly int id;
    public CardRendererHover(int id_)
    {
        id = id_;
    }
}
