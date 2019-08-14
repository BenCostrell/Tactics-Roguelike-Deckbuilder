using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : EnemyTurnBehavior
{
    public readonly int moveSpeed;
    public readonly int damage;
    public readonly int range;
    public enum TargetPriority { ONLY_PLAYER, ONLY_PLANT, PLAYER_PLANT, PLANT_PLAYER, NEAREST, NONE }
    public readonly TargetPriority targetPriority;

    public Attack(int moveSpeed_, int damage_, int range_, TargetPriority targetPriority_) : base(2)
    {
        moveSpeed = moveSpeed_;
        damage = damage_;
        range = range_;
        targetPriority = targetPriority_;
    }

    public override void OnEnemyTurn(GridObject gridObject)
    {
        Approach(gridObject);
        PerformAttack(gridObject);
    }


    private void Approach(GridObject gridObject)
    {
        MapTile targetTile = null;
        MapTile playerTile = Services.LevelManager.player.currentTile;
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
            moveSpeed + range, gridObject);
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
                        plantInRange = true;
                    }
                }
            }
        }
        // check farther tiles
        if (closestPlant == null)
        {
            foreach (MapTile tile in allAvailableTiles)
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
        switch (targetPriority)
        {
            case TargetPriority.ONLY_PLAYER:
                targetTile = Services.LevelManager.player.currentTile;
                break;
            case TargetPriority.ONLY_PLANT:
                targetTile = closestPlant.currentTile;
                break;
            case TargetPriority.PLAYER_PLANT:
                if (playerInRange || playerDistance <= closestPlantDistance)
                {
                    targetTile = Services.LevelManager.player.currentTile;
                }
                else if (closestPlant != null)
                {
                    targetTile = closestPlant.currentTile;
                }
                break;
            case TargetPriority.PLANT_PLAYER:
                if (plantInRange || closestPlantDistance <= playerDistance)
                {
                    targetTile = closestPlant.currentTile;
                }
                else
                {
                    targetTile = playerTile;
                }
                break;
            case TargetPriority.NEAREST:
                if (playerDistance <= closestPlantDistance)
                {
                    targetTile = playerTile;
                }
                else
                {
                    targetTile = closestPlant.currentTile;
                }
                break;
            case TargetPriority.NONE:
                break;
            default:
                break;
        }
        if (targetTile == null)
        {
            return;
        }
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
        for (int i = 0; i < Mathf.Min(moveSpeed, pathToTarget.Count); i++)
        {
            pathToTake.Add(pathToTarget[i]);
        }
        if (pathToTake.Count > 0)
        {
            gridObject.MoveToTile(pathToTake);
        }
    }


    private void PerformAttack(GridObject gridObject)
    {
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
            range, gridObject, true);
        bool playerInRange = false;
        GridObject closestPlant = null;
        int closestPlantDistance = int.MaxValue;
        foreach (MapTile tile in tilesInRange)
        {
            if (tile == Services.LevelManager.player.currentTile)
            {
                playerInRange = true;
            }
            foreach (GridObject gridObj in tile.containedObjects)
            {
                if (gridObj.data.phylum == GridObjectData.Phylum.PLANT)
                {
                    int distance = Coord.Distance(gridObject.currentTile.coord, tile.coord);
                    if (distance < closestPlantDistance)
                    {
                        closestPlant = gridObj;
                        closestPlantDistance = distance;

                    }
                }
            }
        }
        GridObject target = null;

        switch (targetPriority)
        {
            case TargetPriority.ONLY_PLAYER:
                if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                break;
            case TargetPriority.ONLY_PLANT:
                if (closestPlant != null)
                {
                    target = closestPlant;
                }
                break;
            case TargetPriority.PLAYER_PLANT:
                if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                else if (closestPlant != null)
                {
                    target = closestPlant;
                }
                break;
            case TargetPriority.PLANT_PLAYER:
                if (closestPlant != null)
                {
                    target = closestPlant;
                }
                else if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                break;
            case TargetPriority.NEAREST:
                if (playerInRange && Coord.Distance(Services.LevelManager.player.currentTile.coord,
                    gridObject.currentTile.coord) < closestPlantDistance)
                {
                    target = Services.LevelManager.player;
                }
                else if (closestPlant != null)
                {
                    target = closestPlant;
                }
                break;
            case TargetPriority.NONE:
                break;
            default:
                break;
        }

        if (target == null) return;

        target.TakeDamage(damage);
        Services.EventManager.Fire(new ObjectAttacked(gridObject, target, damage));

    }

    public static TargetPriority GetTargetPriorityFromString(string priorityString)
    {
        priorityString = priorityString.Trim();
        switch (priorityString)
        {
            case "ONLY_PLAYER":
                return TargetPriority.ONLY_PLAYER;
            case "ONLY_PLANT":
                return TargetPriority.ONLY_PLANT;
            case "PLAYER_PLANT":
                return TargetPriority.PLAYER_PLANT;
            case "PLANT_PLAYER":
                return TargetPriority.PLANT_PLAYER;
            case "NEAREST":
                return TargetPriority.NEAREST;
            default:
                return TargetPriority.NONE;
        }
    }
}

public class ObjectAttacked : GameEvent
{
    public readonly GridObject attacker;
    public readonly GridObject target;
    public readonly int damage;

    public ObjectAttacked(GridObject attacker_, GridObject target_, int damage_)
    {
        attacker = attacker_;
        target = target_;
        damage = damage_;
    }
}