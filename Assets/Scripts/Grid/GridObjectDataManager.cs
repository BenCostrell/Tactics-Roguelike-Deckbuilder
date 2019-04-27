using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectDataManager
{
    public Dictionary<GridObjectData.GridObjectType, GridObjectData> gridObjectDataDict;

    public GridObjectDataManager()
    {
        gridObjectDataDict = new Dictionary<GridObjectData.GridObjectType, GridObjectData>();
    }

    public void AddData(GridObjectData gridObjectData)
    {
        gridObjectDataDict[gridObjectData.gridObjectType] = gridObjectData;
    }
}
