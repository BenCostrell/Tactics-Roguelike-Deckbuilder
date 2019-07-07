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
        MapTile targetTile = null;
        MapTile playerTile = Services.LevelManager.player.currentTile;
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile, 
            gridObject.moveSpeed + gridObject.data.attackRange, gridObject);
        List<MapTile> allAvailableTiles = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
            int.MaxValue, gridObject);
        bool playerInRange = false;
        bool plantInRange = false;
        GridObject closestPlant = null;
        int closestPlantDistance = int.MaxValue;
        int playerDistance = AStarSearch.ShortestPath(gridObject.currentTile, playerTile, gridObject).Count;
        // check tiles in range
        foreach (MapTile tile in tilesInRange)
        {
            if (tile == Services.LevelManager.player.currentTile)
            {
                playerInRange = true;
            }
            foreach(GridObject gridObj in tile.containedObjects)
            {
                if(gridObj.data.phylum == GridObjectData.Phylum.PLANT)
                {
                    int distance = AStarSearch.ShortestPath(gridObject.currentTile, 
                        tile, gridObject).Count;
                    if(distance < closestPlantDistance)
                    {
                        closestPlant = gridObj;
                        closestPlantDistance = distance;
                        plantInRange = true;
                    }
                }
            }
        }
        // check farther tiles
        if(closestPlant == null)
        {
            foreach(MapTile tile in allAvailableTiles)
            {
                foreach (GridObject gridObj in tile.containedObjects)
                {
                    if (gridObj.data.phylum == GridObjectData.Phylum.PLANT)
                    {
                        int distance = AStarSearch.ShortestPath(gridObject.currentTile,
                            tile, gridObject).Count;
                        if (distance < closestPlantDistance)
                        {
                            closestPlant = gridObj;
                            closestPlantDistance = distance;
                        }
                    }
                }
            }
        }

        // decide target
        switch (gridObject.data.targetPriority)
        {
            case GridObjectData.TargetPriority.ONLY_PLAYER:
                targetTile = Services.LevelManager.player.currentTile;
                break;
            case GridObjectData.TargetPriority.ONLY_PLANT:
                targetTile = closestPlant.currentTile;
                break;
            case GridObjectData.TargetPriority.PLAYER_PLANT:
                if (playerInRange || playerDistance <= closestPlantDistance)
                {
                    targetTile = Services.LevelManager.player.currentTile;
                }
                else if (closestPlant != null)
                {
                    targetTile = closestPlant.currentTile;
                }
                break;
            case GridObjectData.TargetPriority.PLANT_PLAYER:
                if (plantInRange || closestPlantDistance <= playerDistance)
                {
                    targetTile = closestPlant.currentTile;
                }
                else 
                {
                    targetTile = playerTile;
                }
                break;
            case GridObjectData.TargetPriority.NEAREST:
                if(playerDistance <= closestPlantDistance)
                {
                    targetTile = playerTile;
                }
                else
                {
                    targetTile = closestPlant.currentTile;
                }
                break;
            case GridObjectData.TargetPriority.NONE:
                break;
            default:
                break;
        }

        if (targetTile == null) return;
        List<MapTile> pathToTarget = AStarSearch.ShortestPath(gridObject.currentTile,
            targetTile, gridObject);
        // trim the target tile itself
        if (pathToTarget.Count > 0)
        {
            MapTile lastTile = pathToTarget[pathToTarget.Count - 1];
            if (lastTile == targetTile)
            {
                pathToTarget.Remove(targetTile);
            }
        }
        List<MapTile> pathToTake = new List<MapTile>();
        for (int i = 0; i < Mathf.Min(gridObject.moveSpeed, pathToTarget.Count); i++)
        {
            pathToTake.Add(pathToTarget[i]);
        }
        if (pathToTake.Count > 0)
        {
            gridObject.MoveToTile(pathToTake);
        }
    }
}
