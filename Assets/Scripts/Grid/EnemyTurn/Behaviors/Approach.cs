using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Approach : EnemyTurnBehavior
{
    public Approach() : base(1)
    {

    }

    public override void OnEnemyTurn(GridObject gridObject)
    {
        MapTile playerTile = Services.LevelManager.player.currentTile;

        List<MapTile> pathToPlayer = AStarSearch.ShortestPath(gridObject.currentTile,
            playerTile, gridObject);
        // trim the player tile itself
        if (pathToPlayer.Count > 0)
        {
            MapTile lastTile = pathToPlayer[pathToPlayer.Count - 1];
            if (lastTile == playerTile)
            {
                pathToPlayer.Remove(playerTile);
            }
        }
        List<MapTile> pathToTake = new List<MapTile>();
        for (int i = 0; i < Mathf.Min(gridObject.moveSpeed, pathToPlayer.Count); i++)
        {
            pathToTake.Add(pathToPlayer[i]);
        }
        if (pathToTake.Count > 0)
        {
            gridObject.MoveToTile(pathToTake);
        }
    }
}
