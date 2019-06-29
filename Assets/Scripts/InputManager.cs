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
