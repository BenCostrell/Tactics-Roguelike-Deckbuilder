using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChestSkipButton : SpriteButton
{
    private SpriteRenderer sr;
    private TextMeshPro text;

    protected override void Start()
    {
        base.Start();
        sr = GetComponentInChildren<SpriteRenderer>();
        text = GetComponentInChildren<TextMeshPro>();
    }

    protected override void OnHover(InputHover e)
    {
        base.OnHover(e);
        if(e.hoveredButton != this)
        {
            sr.color = Color.white;
            text.color = Color.white;
        }
        else
        {
            sr.color = Color.green;
            text.color = Color.green;
        }
    }

    protected override void OnClick(InputDown e)
    {
        base.OnClick(e);
        if (e.hoveredButton != this) return;
        Services.EventManager.Fire(new CardOfferSelected(null));
    }
}

public abstract class SpriteButton : MonoBehaviour {

    protected virtual void Start()
    {
        Services.EventManager.Register<InputHover>(OnInputHover);
        Services.EventManager.Register<InputDown>(OnInputDown);
    }

    private void OnInputHover(InputHover e)
    {
        OnHover(e);
    }

    private void OnInputDown(InputDown e)
    {
        OnClick(e);
    }

    protected virtual void OnHover(InputHover e) { }
    protected virtual void OnClick(InputDown e) { }
}
