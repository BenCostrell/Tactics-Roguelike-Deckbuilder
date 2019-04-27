using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private MapManager _mapManager;

    // set to constants for now, will probably pull values from somewhere eventually
    private const int width = 8;
    private const int height = 6;

    private void Start()
    {
        Services.EventManager = new GameEventsManager();
        _mapManager = new MapManager();
        _mapManager.InitializeMap(width, height);
    }

    private void Update()
    {
    }
}
