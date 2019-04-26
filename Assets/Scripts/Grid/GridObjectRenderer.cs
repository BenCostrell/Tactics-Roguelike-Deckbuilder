using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectRenderer : MonoBehaviour
{
    private SpriteRenderer sr;
    public GridObject gridObject { get; private set; }

    public void Initialize(GridObject gridObject_, MapTile mapTile, Transform mapHolder)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        //sr.sprite
        gridObject = gridObject_;
        transform.parent = mapHolder;
        transform.localPosition = new Vector2(mapTile.coord.x, mapTile.coord.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
