using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Services.EventManager.Fire(new InputHover(mousePos));
        if (Input.GetMouseButtonDown(0))
        {
            Services.EventManager.Fire(new InputDown(mousePos));
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        int id = -1;
        if(hit.collider != null)
        {
            id = hit.transform.GetComponentInParent<CardRenderer>().id;
        }
        Services.EventManager.Fire(new CardRendererHover(id));
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

public class CardRendererHover : GameEvent
{
    public readonly int id;
    public CardRendererHover(int id_)
    {
        id = id_;
    }
}
