using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhylumTargetOnly : CardEffect
{
    private readonly List<GridObjectData.Phylum> allowedPhyla;

    public PhylumTargetOnly(List<GridObjectData.Phylum> allowedPhyla_) : base(true, 0,int.MaxValue)
    {
        allowedPhyla = allowedPhyla_;
    }

    public override bool IsTargetLegal(MapTile target)
    {
        if (!base.IsTargetLegal(target)) return false;
        foreach (GridObject gridObject in target.containedObjects)
        {
            if (allowedPhyla.Contains(gridObject.data.phylum)) return true;
        }
        return false;
    }
}
