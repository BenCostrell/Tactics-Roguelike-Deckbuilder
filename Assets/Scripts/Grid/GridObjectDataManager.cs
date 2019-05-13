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
        AddData("PLAYER", false, 3);
        AddData("GOBLIN", true, 1);
    }

    private void AddData(string name, bool enemy, int maxHealth)
    {
        GridObjectData gridObjectData = new GridObjectData(name, atlas.GetSprite(name.ToLower()), enemy, maxHealth);
        gridObjectDataDict[gridObjectData.gridObjectName] = gridObjectData;
    }

    public GridObjectData GetData(string name)
    {
        return gridObjectDataDict[name];
    }
}
