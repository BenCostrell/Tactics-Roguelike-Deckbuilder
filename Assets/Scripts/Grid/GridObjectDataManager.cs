using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectDataManager
{
    private Dictionary<GridObjectData.GridObjectType, GridObjectData> gridObjectDataDict;

    public GridObjectDataManager()
    {
        gridObjectDataDict = new Dictionary<GridObjectData.GridObjectType, GridObjectData>();
        //temporary, will ultimately load in from spreadsheet
        AddData(GridObjectData.GridObjectType.PLAYER, Resources.Load<Sprite>("Sprites/helmetDude"));
    }

    private void AddData(GridObjectData.GridObjectType type, Sprite sprite)
    {
        GridObjectData gridObjectData = new GridObjectData(type, sprite);
        gridObjectDataDict[gridObjectData.gridObjectType] = gridObjectData;
    }

    public GridObjectData GetData(GridObjectData.GridObjectType type)
    {
        return gridObjectDataDict[type];
    }
}
