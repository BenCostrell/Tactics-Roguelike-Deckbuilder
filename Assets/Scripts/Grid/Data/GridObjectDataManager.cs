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
        TextAsset objectDataDoc = Resources.Load<TextAsset>("Data/Object_Database");
        string[] objectDataEntries = objectDataDoc.text.Split('\n');
        for (int i = 1; i < objectDataEntries.Length; i++)
        {
            string[] fields = objectDataEntries[i].Split('\t');
            AddData(
                fields[0], // name
                GridObjectData.StringToPhylum(fields[1]), // phylum
                int.Parse(fields[2]), // health
                fields[3].Split(';') // behaviors
                );
        }
        //temporary, will ultimately load in from spreadsheet
        //AddData("PLAYER", 3, new List<string>(), GridObjectData.Phylum.PLAYER);
        //AddData("GOBLIN", 1, new List<string>() { "ATTACK,2,1,1,PLAYER_PLANT" },
        //    GridObjectData.Phylum.ENEMY);
    }

    private void AddData(string name, GridObjectData.Phylum phylum,
        int maxHealth, string[] enemyTurnBehaviors)
    {
        List<EnemyTurnBehavior> behaviors = new List<EnemyTurnBehavior>();
        foreach(string behaviorString in enemyTurnBehaviors)
        {
            EnemyTurnBehavior behavior = EnemyTurnBehavior.ParseBehaviorString(behaviorString);
            if (behavior != null) behaviors.Add(behavior);
        }
        GridObjectData gridObjectData = new GridObjectData(name.ToUpper(),
            atlas.GetSprite(name.ToLower()), maxHealth, behaviors, phylum);
        gridObjectDataDict[gridObjectData.gridObjectName] = gridObjectData;
    }

    public GridObjectData GetData(string name)
    {
        return gridObjectDataDict[name.ToUpper()];
    }
}
