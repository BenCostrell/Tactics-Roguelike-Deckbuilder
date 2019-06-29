using UnityEngine;
using System.Collections;
using UnityEngine.U2D;

public class Reticle : MonoBehaviour
{

    // Use this for initialization
    public void Init(Transform mapHolder)
    {
        transform.parent = mapHolder;
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<SpriteAtlas>("SpriteData/UiAtlas").GetSprite("reticle1");
        sr.sortingLayerName = "UI";
        gameObject.name = "Reticle";
        Services.EventManager.Register<TileHovered>(OnTileHovered);
    }

    public void OnTileHovered(TileHovered e)
    {
        transform.localPosition = new Vector3(e.tile.coord.x, e.tile.coord.y, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
