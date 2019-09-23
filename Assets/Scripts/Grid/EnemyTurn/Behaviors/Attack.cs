using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Attack : EnemyTurnBehavior
{
    public readonly int moveSpeed;
    public readonly int damage;
    public readonly int range;
    private const int priorityThreshold = 40;

    public Attack(int moveSpeed_, int damage_, int range_) : base(2)
    {
        moveSpeed = moveSpeed_;
        damage = damage_;
        range = range_;
    }

    public override void OnEnemyTurn(GridObject gridObject)
    {
        PlannedAttack plannedAttack = GetCurrentTarget(gridObject);
        if (plannedAttack.plannedPath.Count > 0)
        {
            gridObject.MoveToTile(plannedAttack.plannedPath);
        }
        if(plannedAttack.attackTarget != null)
        {
            plannedAttack.attackTarget.ChangeHealth(-damage);
            Services.EventManager.Fire(new ObjectAttacked(gridObject, plannedAttack.attackTarget, damage));
        }
    }

    public PlannedAttack GetCurrentTarget(GridObject gridObject)
    {
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
            moveSpeed, gridObject);
        List<GridObject> priorityTargets = new List<GridObject>();
        foreach(GridObject gridObj in Services.LevelManager.mapManager.gridObjectList)
        {
            if(gridObj != gridObject && gridObj.data.targetPriority >= priorityThreshold)
            {
                priorityTargets.Add(gridObj);
            }
        }
        // if there are any high priority targets within attack range, go for the highest amongst them
        GridObject highestPriorityTargetInRange = null;
        int highestPriority = 0;
        List<MapTile> plannedPath = new List<MapTile>();
        foreach (MapTile tile in tilesInRange)
        {
            foreach (GridObject priorityTarget in priorityTargets)
            {
                int dist = priorityTarget.currentTile.coord.Distance(tile.coord);
                if (dist > 0 && dist <= range && priorityTarget.data.targetPriority >= highestPriority)
                {
                    if (highestPriorityTargetInRange != priorityTarget)
                    {
                        highestPriority = priorityTarget.data.targetPriority;
                        highestPriorityTargetInRange = priorityTarget;
                        plannedPath = AStarSearch.ShortestPath(gridObject.currentTile, tile,
                            gridObject);
                    }
                    else
                    {
                        List<MapTile> path = AStarSearch.ShortestPath(gridObject.currentTile, tile, 
                            gridObject);
                        if (path.Count < plannedPath.Count)
                        {
                            plannedPath = path;
                        }
                    }
                }
            }
        }
        if (highestPriorityTargetInRange != null)
        {
            //Debug.Log(gridObject.data.gridObjectName + " at " + gridObject.currentTile.coord +
            //    " going for " + plannedPath[plannedPath.Count - 1].coord + " as high priority" +
            //    " target in range");
            return new PlannedAttack(highestPriorityTargetInRange, plannedPath);
        }
        // otherwise, go towards the closest high priority target
        GridObject closestPriorityTarget = null;
        int minDistToClosestPriorityTarget = int.MaxValue;
        foreach(GridObject priorityTarget in priorityTargets)
        {
            int dist = priorityTarget.currentTile.coord.Distance(gridObject.currentTile.coord);
            //Debug.Log("for " + gridObject.data.gridObjectName + " " + priorityTarget.data.gridObjectName
            //    + " is " + dist + " away");
            if (dist < minDistToClosestPriorityTarget || 
                (dist == minDistToClosestPriorityTarget
                && priorityTarget.data.targetPriority > closestPriorityTarget.data.targetPriority))
            {
                closestPriorityTarget = priorityTarget;
                minDistToClosestPriorityTarget = dist;
            }
        }
        //Debug.Log("closest target is " + closestPriorityTarget.data.gridObjectName);
        // blaze a path if necessary
        List<List<MapTile>> possibleDirectPaths = AStarSearch.GetAllDirectPaths(gridObject.currentTile,
            closestPriorityTarget.currentTile);
        foreach(List<MapTile> path in possibleDirectPaths)
        {
            path.RemoveAt(0);
        }
        // sort by how many obstacles, try the clearer paths first
        possibleDirectPaths = new List<List<MapTile>>(
            possibleDirectPaths.OrderBy(l => PathObstacleCount(l)));
        List<MapTile> bestPath = new List<MapTile>();
        List<MapTile> provisionalPath = new List<MapTile>();
        MapTile nextTile = null;
        //Debug.Log("found " + possibleDirectPaths.Count + " possible direct paths");
        for (int i = 0; i < possibleDirectPaths.Count; i++)
        {
            List<MapTile> path = possibleDirectPaths[i];
            provisionalPath.Clear();
            for (int j = 0; j < Mathf.Min(path.Count,moveSpeed); j++)
            {
                MapTile tile = path[j];
                // stop if we would hit a non enemy object
                if (tile.containedObject != null &&
                    tile.containedObject.data.phylum != GridObjectData.Phylum.ENEMY)
                {
                    break;
                }
                else
                {
                    provisionalPath.Add(tile);
                }
            }
            MapTile provisionalNextTile = path[provisionalPath.Count];
            if (provisionalPath.Count > 0)
            {
                MapTile lastTile = provisionalPath[provisionalPath.Count - 1];
                if (lastTile.containedObject != null
                    && lastTile.containedObject.data.phylum == GridObjectData.Phylum.ENEMY)
                {
                    provisionalPath.Remove(lastTile);
                    provisionalNextTile = null;
                }
            }
            if (provisionalPath.Count > bestPath.Count || (bestPath.Count == 0 && nextTile == null))
            {
                bestPath = new List<MapTile>(provisionalPath);
                nextTile = provisionalNextTile;
            }
        }
        GridObject target = null;
        //Debug.Log("considering next tile at " + nextTile.coord);
        if(nextTile != null && nextTile.containedObject != null)
        {
            target = nextTile.containedObject;
        }
        //Debug.Log(gridObject.data.gridObjectName + " at " + gridObject.currentTile.coord +
        //       " going towards " 
        //       + closestPriorityTarget.data.gridObjectName + 
        //       " target outside of range");
        return new PlannedAttack(target, bestPath);
    }

    private int PathObstacleCount(List<MapTile> path)
    {
        int obstacleCount = 0;
        foreach(MapTile tile in path)
        {
            if(tile.containedObject != null && tile.containedObject.data.phylum != GridObjectData.Phylum.ENEMY)
            {
                obstacleCount += 1;
            }
        }
        return obstacleCount;
    }

    //private void Approach(GridObject gridObject)
    //{
    //    MapTile targetTile = null;
    //    MapTile playerTile = Services.LevelManager.player.currentTile;
    //    List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
    //        moveSpeed + range, gridObject);
    //    List<MapTile> allAvailableTiles = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
    //        int.MaxValue, gridObject);
    //    bool playerInRange = false;
    //    bool plantInRange = false;
    //    GridObject closestPlant = null;
    //    int closestPlantDistance = int.MaxValue;
    //    int playerDistance = AStarSearch.ShortestPath(gridObject.currentTile, playerTile, gridObject).Count;
    //    // check tiles in range
    //    foreach (MapTile tile in tilesInRange)
    //    {
    //        if (tile == Services.LevelManager.player.currentTile)
    //        {
    //            playerInRange = true;
    //        }
    //        foreach (GridObject gridObj in tile.containedObject)
    //        {
    //            if (gridObj.data.phylum == GridObjectData.Phylum.PLANT)
    //            {
    //                int distance = AStarSearch.ShortestPath(gridObject.currentTile,
    //                    tile, gridObject).Count;
    //                if (distance < closestPlantDistance)
    //                {
    //                    closestPlant = gridObj;
    //                    closestPlantDistance = distance;
    //                    plantInRange = true;
    //                }
    //            }
    //        }
    //    }
    //    // check farther tiles
    //    if (closestPlant == null)
    //    {
    //        foreach (MapTile tile in allAvailableTiles)
    //        {
    //            foreach (GridObject gridObj in tile.containedObject)
    //            {
    //                if (gridObj.data.phylum == GridObjectData.Phylum.PLANT)
    //                {
    //                    int distance = AStarSearch.ShortestPath(gridObject.currentTile,
    //                        tile, gridObject).Count;
    //                    if (distance < closestPlantDistance)
    //                    {
    //                        closestPlant = gridObj;
    //                        closestPlantDistance = distance;
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    // decide target
    //    switch (targetPriority)
    //    {
    //        case TargetPriority.ONLY_PLAYER:
    //            targetTile = Services.LevelManager.player.currentTile;
    //            break;
    //        case TargetPriority.ONLY_PLANT:
    //            targetTile = closestPlant.currentTile;
    //            break;
    //        case TargetPriority.PLAYER_PLANT:
    //            if (playerInRange || playerDistance <= closestPlantDistance)
    //            {
    //                targetTile = Services.LevelManager.player.currentTile;
    //            }
    //            else if (closestPlant != null)
    //            {
    //                targetTile = closestPlant.currentTile;
    //            }
    //            break;
    //        case TargetPriority.PLANT_PLAYER:
    //            if (plantInRange || closestPlantDistance <= playerDistance)
    //            {
    //                targetTile = closestPlant.currentTile;
    //            }
    //            else
    //            {
    //                targetTile = playerTile;
    //            }
    //            break;
    //        case TargetPriority.NEAREST:
    //            if (playerDistance <= closestPlantDistance)
    //            {
    //                targetTile = playerTile;
    //            }
    //            else
    //            {
    //                targetTile = closestPlant.currentTile;
    //            }
    //            break;
    //        case TargetPriority.NONE:
    //            break;
    //        default:
    //            break;
    //    }
    //    if (targetTile == null)
    //    {
    //        return;
    //    }
    //    List<MapTile> pathToTarget = AStarSearch.ShortestPath(gridObject.currentTile,
    //        targetTile, gridObject);
    //    // trim the target tile itself
    //    if (pathToTarget.Count > 0)
    //    {
    //        MapTile lastTile = pathToTarget[pathToTarget.Count - 1];
    //        if (lastTile == targetTile)
    //        {
    //            pathToTarget.Remove(targetTile);
    //        }
    //    }
    //    List<MapTile> pathToTake = new List<MapTile>();
    //    for (int i = 0; i < Mathf.Min(moveSpeed, pathToTarget.Count); i++)
    //    {
    //        pathToTake.Add(pathToTarget[i]);
    //    }
    //    if (pathToTake.Count > 0)
    //    {
    //        gridObject.MoveToTile(pathToTake);
    //    }
    //}

    //private void PerformAttack(GridObject gridObject)
    //{
    //    List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
    //        range, gridObject, true);
    //    bool playerInRange = false;
    //    GridObject closestPlant = null;
    //    int closestPlantDistance = int.MaxValue;
    //    foreach (MapTile tile in tilesInRange)
    //    {
    //        if (tile == Services.LevelManager.player.currentTile)
    //        {
    //            playerInRange = true;
    //        }
    //        foreach (GridObject gridObj in tile.containedObject)
    //        {
    //            if (gridObj.data.phylum == GridObjectData.Phylum.PLANT)
    //            {
    //                int distance = Coord.Distance(gridObject.currentTile.coord, tile.coord);
    //                if (distance < closestPlantDistance)
    //                {
    //                    closestPlant = gridObj;
    //                    closestPlantDistance = distance;

    //                }
    //            }
    //        }
    //    }
    //    GridObject target = null;

    //    switch (targetPriority)
    //    {
    //        case TargetPriority.ONLY_PLAYER:
    //            if (playerInRange)
    //            {
    //                target = Services.LevelManager.player;
    //            }
    //            break;
    //        case TargetPriority.ONLY_PLANT:
    //            if (closestPlant != null)
    //            {
    //                target = closestPlant;
    //            }
    //            break;
    //        case TargetPriority.PLAYER_PLANT:
    //            if (playerInRange)
    //            {
    //                target = Services.LevelManager.player;
    //            }
    //            else if (closestPlant != null)
    //            {
    //                target = closestPlant;
    //            }
    //            break;
    //        case TargetPriority.PLANT_PLAYER:
    //            if (closestPlant != null)
    //            {
    //                target = closestPlant;
    //            }
    //            else if (playerInRange)
    //            {
    //                target = Services.LevelManager.player;
    //            }
    //            break;
    //        case TargetPriority.NEAREST:
    //            if (playerInRange && Coord.Distance(Services.LevelManager.player.currentTile.coord,
    //                gridObject.currentTile.coord) < closestPlantDistance)
    //            {
    //                target = Services.LevelManager.player;
    //            }
    //            else if (closestPlant != null)
    //            {
    //                target = closestPlant;
    //            }
    //            break;
    //        case TargetPriority.NONE:
    //            break;
    //        default:
    //            break;
    //    }

    //    if (target == null) return;

    //    target.TakeDamage(damage);
    //    Services.EventManager.Fire(new ObjectAttacked(gridObject, target, damage));

    //}

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

public class PlannedAttack
{
    public readonly GridObject attackTarget;
    public readonly List<MapTile> plannedPath;
    public PlannedAttack(GridObject attackTarget_, List<MapTile> plannedPath_)
    {
        attackTarget = attackTarget_;
        plannedPath = plannedPath_;
    }
}
