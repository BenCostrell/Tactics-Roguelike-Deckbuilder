using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileRenderer : MonoBehaviour
{
    private SpriteRenderer sr;
    public MapTile tile { get; private set; }
    private BoxCollider2D col;
    public enum RangeLevel { NONE, MOVE, ATTACK }
    private static Dictionary<RangeLevel, Color> rangeColorDict = new Dictionary<RangeLevel, Color>()
    {
        { RangeLevel.NONE, Color.white },
        { RangeLevel.MOVE, new Color(100f/255, 149f/255, 237f/255) },
        { RangeLevel.ATTACK, Color.red }
    };

    public void Init(MapTile tile_, Transform mapHolder)
    {
        tile = tile_;
        gameObject.name = "Tile: " + tile.coord.x + ", " + tile.coord.y;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = tile.terrain.sprite;
        sr.sortingLayerName = "Map";
        transform.parent = mapHolder;
        transform.localPosition = new Vector3(tile.coord.x, tile.coord.y, 0);
        col = gameObject.AddComponent<BoxCollider2D>();
        //Services.EventManager.Register<InputHover>(CheckHover);
    }

    //public void CheckHover(InputHover e)
    //{
    //    if (sr.bounds.Contains(e.worldPos))
    //    {
    //        OnHover();
    //    }
    //}

    public void OnHover(bool cardSelected)
    {
        Services.EventManager.Fire(new TileHovered(tile, cardSelected));
    }

    public void OnSelected(int cardSelectedId)
    {
        Services.EventManager.Fire(new MapTileSelected(tile, cardSelectedId));
    }

    public void SetRangeColor(RangeLevel rangeLevel)
    {
        sr.color = rangeColorDict[rangeLevel];
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class TileHovered : GameEvent
{
    public readonly MapTile tile;
    public readonly bool cardSelected;

    public TileHovered(MapTile tile_, bool cardSelected_)
    {
        tile = tile_;
        cardSelected = cardSelected_;
    }
}
