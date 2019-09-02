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
        if (target.containedObject != null &&
            allowedPhyla.Contains(target.containedObject.data.phylum)) return true;
        return false;
    }
}
