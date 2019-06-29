using UnityEngine;
using System.Collections;

public class TileRenderer : MonoBehaviour
{
    private SpriteRenderer sr;
    private MapTile tile;

    public void Init(MapTile tile_, Transform mapHolder)
    {
        tile = tile_;
        gameObject.name = "Tile: " + tile.coord.x + ", " + tile.coord.y;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = tile.terrain.sprite;
        sr.sortingLayerName = "Map";
        transform.parent = mapHolder;
        transform.localPosition = new Vector3(tile.coord.x, tile.coord.y, 0);
        Services.EventManager.Register<InputHover>(CheckHover);
    }

    public void CheckHover(InputHover e)
    {
        if (sr.bounds.Contains(e.worldPos))
        {
            OnHover();
        }
    }

    private void OnHover()
    {
        Services.EventManager.Fire(new TileHovered(tile));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class TileHovered : GameEvent
{
    public readonly MapTile tile;

    public TileHovered(MapTile tile_)
    {
        tile = tile_;
    }
}
