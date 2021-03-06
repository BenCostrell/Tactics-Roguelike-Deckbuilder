﻿using System.Collections;
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
                fields[3].Split(';'), // behaviors
                fields[4].Split(';'), // interactions
                int.Parse(fields[5]) // target priority
                );
        }
        //temporary, will ultimately load in from spreadsheet
        //AddData("PLAYER", 3, new List<string>(), GridObjectData.Phylum.PLAYER);
        //AddData("GOBLIN", 1, new List<string>() { "ATTACK,2,1,1,PLAYER_PLANT" },
        //    GridObjectData.Phylum.ENEMY);
    }

    private void AddData(string name, GridObjectData.Phylum phylum,
        int maxHealth, string[] behaviorStrings, string[] interactionStrings, int targetPriority)
    {
        List<EnemyTurnBehavior> behaviors = new List<EnemyTurnBehavior>();
        foreach(string behaviorString in behaviorStrings)
        {
            EnemyTurnBehavior behavior = EnemyTurnBehavior.ParseBehaviorString(behaviorString);
            if (behavior != null) behaviors.Add(behavior);
        }
        List<ObjectInteraction> interactions = new List<ObjectInteraction>();
        foreach (string interactionString in interactionStrings)
        {
            ObjectInteraction interaction = ObjectInteraction.ParseInteractionString(interactionString);
            if (interaction != null) interactions.Add(interaction);
        }
        GridObjectData gridObjectData = new GridObjectData(name.ToUpper(),
            atlas.GetSprite(name.ToLower()), maxHealth, behaviors, interactions, phylum, targetPriority);
        gridObjectDataDict[gridObjectData.gridObjectName] = gridObjectData;
    }

    public GridObjectData GetData(string name)
    {
        return gridObjectDataDict[name.ToUpper()];
    }

    public List<GridObjectData> GetEnemies()
    {
        List<GridObjectData> enemies = new List<GridObjectData>();
        foreach(GridObjectData gridObjectData in gridObjectDataDict.Values)
        {
            if(gridObjectData.phylum == GridObjectData.Phylum.ENEMY)
            {
                enemies.Add(gridObjectData);
            }
        }

        return enemies;
    }
}
