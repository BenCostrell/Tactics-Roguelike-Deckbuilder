using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridObject
{
    private int _currentEnergy;
    private int currentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            if (currentEnergy != value)
            {
                int prevEnergy = _currentEnergy;
                _currentEnergy = value;
                Services.EventManager.Fire(
                    new EnergyChanged(prevEnergy, currentEnergy, currentMaxEnergy, currentMaxEnergy));
            }
        }
    }
    private int _currentMaxEnergy;
    private int currentMaxEnergy
    {
        get { return _currentMaxEnergy; }
        set
        {
            if (currentMaxEnergy != value)
            {
                int prevMaxEnergy = _currentMaxEnergy;
                _currentMaxEnergy = value;
                int prevEnergy = currentEnergy;
                currentEnergy = Mathf.Min(currentEnergy, currentMaxEnergy);
                Services.EventManager.Fire(
                    new EnergyChanged(prevEnergy, currentEnergy, prevMaxEnergy, currentMaxEnergy));
            }
        }
    }
    private const int BASEMAXENERGY = 3;

    public Player() : base(Services.GridObjectDataManager.GetData(GridObjectData.GridObjectType.PLAYER))
    {
        Services.EventManager.Register<MapTileSelected>(OnTileSelected);
        currentMaxEnergy = BASEMAXENERGY;
        currentEnergy = currentMaxEnergy;
    }

    private void OnTileSelected(MapTileSelected e)
    {
        MapTile targetTile = e.mapTile;
        if (targetTile != _currentTile)
        {
            List<MapTile> path = AStarSearch.ShortestPath(_currentTile, targetTile, this);
            if (IsTileReachable(_currentEnergy, path))
            {
                currentEnergy -= path.Count;
                MoveToTile(targetTile, path);
            }
        }
    }

    public void Refresh()
    {
        currentEnergy = currentMaxEnergy;
    }
}

public class EnergyChanged : GameEvent
{
    public readonly int prevEnergy;
    public readonly int currentEnergy;
    public readonly int prevMaxEnergy;
    public readonly int currentMaxEnergy;

    public EnergyChanged(int prevEnergy_, int currentEnergy_, int prevMaxEnergy_, int currentMaxEnergy_)
    {
        prevEnergy = prevEnergy_;
        currentEnergy = currentEnergy_;
        prevMaxEnergy = prevMaxEnergy_;
        currentMaxEnergy = currentMaxEnergy_;
    }
}
