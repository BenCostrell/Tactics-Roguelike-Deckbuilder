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
