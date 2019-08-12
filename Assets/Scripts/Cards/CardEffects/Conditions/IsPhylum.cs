using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class IsPhylum : Condition
{
    private readonly List<GridObjectData.Phylum> allowedPhyla;

    public IsPhylum(List<GridObjectData.Phylum> allowedPhyla_)
    {
        allowedPhyla = allowedPhyla_;
    }

    public override bool Evaluate(MapTile target)
    {
        foreach(GridObject gridObject in target.containedObjects)
        {
            if (allowedPhyla.Contains(gridObject.data.phylum)) return true;
        }
        return false;
    }
}
