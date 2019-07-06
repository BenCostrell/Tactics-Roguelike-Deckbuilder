using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectRenderer : MonoBehaviour
{
    private SpriteRenderer sr;
    public int id { get; private set; }
    private Coroutine movementCoroutine;
    private const float moveTimePerTile = 0.2f;
    public bool animating { get; private set; }

    public void Initialize(GridObject gridObject, MapTile mapTile, Transform mapHolder)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = gridObject.data.sprite;
        sr.sortingLayerName = "Objects";
        id = gridObject.id;
        gameObject.name = gridObject.data.gridObjectName;
        transform.parent = mapHolder;
        transform.localPosition = new Vector2(mapTile.coord.x, mapTile.coord.y);
    }

    public void MoveToTile(GridObjectMoved e)
    {
        movementCoroutine = StartCoroutine(MovementCoroutine(e));
    }

    IEnumerator MovementCoroutine(GridObjectMoved e)
    {
        float timeElapsed = 0;
        Coord prevCoord = e.originalTile.coord;
        Coord nextCoord = e.path[0].coord;
        int pathIndex = 0;
        bool done = false;
        animating = true;
        while(!done)
        {
            timeElapsed += Time.deltaTime;
            transform.localPosition = Vector2.Lerp(
                new Vector2(prevCoord.x, prevCoord.y),
                new Vector2(nextCoord.x, nextCoord.y),
                timeElapsed / moveTimePerTile);
            if(timeElapsed >= moveTimePerTile)
            {
                pathIndex += 1;
                if(pathIndex >= e.path.Count)
                {
                    done = true;
                }
                else
                {
                    prevCoord = nextCoord;
                    nextCoord = e.path[pathIndex].coord;
                    timeElapsed = 0;
                }
            }
            yield return null;
        }
        animating = false;
    }
}
