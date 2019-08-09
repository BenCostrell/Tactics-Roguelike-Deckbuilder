using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    public LineRenderer lr;
    public SpriteRenderer arrow;
    private MapTile lastTileHovered;
    private const float arrowOffset = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        lr.enabled = false;
        arrow.enabled = false;
        Services.EventManager.Register<TileHovered>(OnTileHovered);
    }

    public void OnTileHovered(TileHovered e)
    {
        if (e.tile == lastTileHovered) return;
        if (e.cardSelected) return;
        lastTileHovered = e.tile;
        Player player = Services.LevelManager.player;
        bool cantReach = false;
        if (e.tile != null)
        {
            List<MapTile> playerAvailableGoals = AStarSearch.FindAllAvailableGoals(player.currentTile,
                player.currentEnergy, player);
            if(playerAvailableGoals.Contains(e.tile))
            {
                lr.enabled = true;
                arrow.enabled = true;
                // set positions
                List<MapTile> path = AStarSearch.ShortestPath(player.currentTile, e.tile, player);
                path.Insert(0, player.currentTile);
                lr.positionCount = path.Count;
                Vector3[] positions = new Vector3[path.Count];
                for (int i = 0; i < positions.Length; i++)
                {
                    Coord coord = path[i].coord;
                    positions[i] = new Vector3(coord.x, coord.y, 0);
                }
                lr.SetPositions(positions);
                Vector3 diff = positions[positions.Length - 1] - positions[positions.Length - 2];
                arrow.transform.localPosition = positions[positions.Length - 1] + (arrowOffset*diff);
                arrow.transform.localEulerAngles = new Vector3(0, 0,
                    -90 + Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x));
            }
            else
            {
                cantReach = true;
            }
        }
        else
        {
            cantReach = true;
        }
        if (cantReach)
        {
            lr.enabled = false;
            arrow.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
