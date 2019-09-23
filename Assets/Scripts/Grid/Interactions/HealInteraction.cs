using UnityEngine;
using System.Collections;

public class HealInteraction : ObjectInteraction
{
    private readonly int healingAmount;

    public HealInteraction(int healingAmount_)
    {
        healingAmount = healingAmount_;
    }

    public override void OnInteract(GridObject gridObject)
    {
        if (gridObject.used)
        {
            return;
        }
        gridObject.used = true;
        Services.LevelManager.player.ChangeHealth(healingAmount);
    }
}
