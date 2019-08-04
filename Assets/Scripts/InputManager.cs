using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        selectedRendererID = -1;
    }

    private int selectedRendererID;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Services.EventManager.Fire(new InputHover(mousePos));
        if (Input.GetMouseButtonDown(0))
        {
            Services.EventManager.Fire(new InputDown(mousePos));
        }
        if (!Input.GetMouseButton(0))
        {
            selectedRendererID = -1;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        int id = -1;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
        {
            Services.EventManager.Fire(new InputUp(mouseWorldPos));
        }
        if (hit.collider != null)
        {
            id = hit.transform.GetComponentInParent<CardRenderer>().id;
            if (Input.GetMouseButtonDown(0))
            {
                selectedRendererID = id;
                Services.EventManager.Fire(new CardRendererSelected(id, mouseWorldPos));
            }
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

    public InputDown(Vector2 worldPos_)
    {
        worldPos = worldPos_;
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
    public InputUp(Vector2 worldPos_)
    {
        worldPos = worldPos_;
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
