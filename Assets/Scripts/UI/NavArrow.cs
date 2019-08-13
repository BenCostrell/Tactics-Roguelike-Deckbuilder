using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    public LineRenderer lr;
    public SpriteRenderer arrow;
    private TileRenderer lastTileHovered;
    private const float arrowOffset = 0.1f;
    private int lastPlayerEnergy;

    // Start is called before the first frame update
    void Start()
    {
        lr.enabled = false;
        arrow.enabled = false;
        Services.EventManager.Register<InputHover>(OnInputHover);
    }

    public void OnInputHover(InputHover e)
    {
        Player player = Services.LevelManager.player;
        if (e.hoveredTile == lastTileHovered && player.currentEnergy == lastPlayerEnergy) return;
        if (e.cardSelected) return;
        lastPlayerEnergy = player.currentEnergy;
        lastTileHovered = e.hoveredTile;
        bool cantReach = false;
        if (e.hoveredTile != null)
        {
            List<MapTile> playerAvailableGoals = AStarSearch.FindAllAvailableGoals(player.currentTile,
                player.currentEnergy, player);
            if(playerAvailableGoals.Contains(e.hoveredTile.tile))
            {
                lr.enabled = true;
                arrow.enabled = true;
                // set positions
                List<MapTile> path = AStarSearch.ShortestPath(player.currentTile, e.hoveredTile.tile, player);
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
