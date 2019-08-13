using UnityEngine;
using System.Collections;
using UnityEngine.U2D;

public class Reticle : MonoBehaviour
{
    private SpriteRenderer sr;
    
    // Use this for initialization
    public void Init(Transform mapHolder)
    {
        transform.parent = mapHolder;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<SpriteAtlas>("SpriteData/UiAtlas").GetSprite("reticle1");
        sr.sortingLayerName = "UI";
        gameObject.name = "Reticle";
        Services.EventManager.Register<InputHover>(OnInputHover);
    }

    public void OnInputHover(InputHover e)
    {
        if (e.hoveredTile == null)
        {
            sr.enabled = false;
        }
        else
        {
            sr.enabled = true;
            transform.localPosition = new Vector3(e.hoveredTile.tile.coord.x, e.hoveredTile.tile.coord.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
