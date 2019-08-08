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

    public Player() : base(Services.GridObjectDataManager.GetData("PLAYER"))
    {
        Services.EventManager.Register<MapTileSelected>(OnTileSelected);
        Services.EventManager.Register<PlayerTurnStarted>(Refresh);
        Services.EventManager.Register<PlayerTurnEnded>(OnPlayerTurnEnded);
        currentMaxEnergy = BASEMAXENERGY;
        currentEnergy = currentMaxEnergy;
        Services.LevelManager.player = this;
    }

    private void OnTileSelected(MapTileSelected e)
    {
        if (e.selectedCardId != -1) return;
        MapTile targetTile = e.mapTile;
        if (targetTile != currentTile)
        {
            List<MapTile> path = AStarSearch.ShortestPath(currentTile, targetTile, this);
            if (path.Count == 0) return; // target is in front of me but impassable
            if (path[path.Count - 1] != targetTile) return; // path is as close as it can get but target is impassable
            if (!IsTileReachable(_currentEnergy, path)) return; // not enough energy to follow path
            currentEnergy -= path.Count;
            MoveToTile(path);
        }
    }

    public void Refresh(PlayerTurnStarted e)
    {
        currentEnergy = currentMaxEnergy;
        Services.EventManager.Register<MapTileSelected>(OnTileSelected);
    }

    public void OnPlayerTurnEnded(PlayerTurnEnded e)
    {
        Services.EventManager.Unregister<MapTileSelected>(OnTileSelected);

        // temp until enemy turn is implemented
        //Services.LevelManager.Invoke("RestartPlayerTurn", 5);
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
