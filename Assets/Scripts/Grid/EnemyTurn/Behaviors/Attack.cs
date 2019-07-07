using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : EnemyTurnBehavior
{
    public Attack() : base(2)
    {

    }

    public override void OnEnemyTurn(GridObject gridObject)
    {
        List<MapTile> tilesInRange = AStarSearch.FindAllAvailableGoals(gridObject.currentTile,
            gridObject.data.attackRange, gridObject, true);
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

        switch (gridObject.data.targetPriority)
        {
            case GridObjectData.TargetPriority.ONLY_PLAYER:
                if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                break;
            case GridObjectData.TargetPriority.ONLY_PLANT:
                if (closestPlant != null)
                {
                    target = closestPlant;
                }
                break;
            case GridObjectData.TargetPriority.PLAYER_PLANT:
                if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                else if (closestPlant != null)
                {
                    target = closestPlant;
                }
                break;
            case GridObjectData.TargetPriority.PLANT_PLAYER:
                if (closestPlant != null)
                {
                    target = closestPlant;
                }
                else if (playerInRange)
                {
                    target = Services.LevelManager.player;
                }
                break;
            case GridObjectData.TargetPriority.NEAREST:
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
            case GridObjectData.TargetPriority.NONE:
                break;
            default:
                break;
        }

        if (target == null) return;

        target.TakeDamage(gridObject.attackDamage);
        Services.EventManager.Fire(new ObjectAttacked(gridObject, target, gridObject.attackDamage));

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