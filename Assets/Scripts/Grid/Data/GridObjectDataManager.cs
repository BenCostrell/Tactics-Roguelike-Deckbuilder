using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GridObjectDataManager
{
    private Dictionary<string, GridObjectData> gridObjectDataDict;
    private SpriteAtlas atlas;

    public GridObjectDataManager()
    {
        gridObjectDataDict = new Dictionary<string, GridObjectData>();
        atlas = Resources.Load<SpriteAtlas>("SpriteData/GridObjectSprites");
        //temporary, will ultimately load in from spreadsheet
        AddData("PLAYER", 3, 0, 1, 0, new List<string>(), GridObjectData.TargetPriority.NONE,
            GridObjectData.Phylum.PLAYER);
        AddData("GOBLIN", 1, 2, 1, 1, new List<string>() { "APPROACH", "ATTACK" },
            GridObjectData.TargetPriority.PLAYER_PLANT, GridObjectData.Phylum.ENEMY);
    }

    private void AddData(string name, int maxHealth, int moveSpeed, int attackRange,
        int attackDamage, List<string> enemyTurnBehaviors, 
        GridObjectData.TargetPriority targetPriority, GridObjectData.Phylum phylum)
    {
        List<EnemyTurnBehavior> behaviors = new List<EnemyTurnBehavior>();
        foreach(string behaviorString in enemyTurnBehaviors)
        {
            if (EnemyTurnBehavior.behaviors.ContainsKey(behaviorString))
            {
                behaviors.Add(EnemyTurnBehavior.behaviors[behaviorString]);
            }
        }
        GridObjectData gridObjectData = new GridObjectData(name, 
            atlas.GetSprite(name.ToLower()), maxHealth, moveSpeed, attackRange, attackDamage,
            behaviors, targetPriority, phylum);
        gridObjectDataDict[gridObjectData.gridObjectName] = gridObjectData;
    }

    public GridObjectData GetData(string name)
    {
        return gridObjectDataDict[name];
    }
}
