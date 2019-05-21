using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private MapManager _mapManager;
    [HideInInspector]
    public Player player;

    public bool waitForAnimation;

    private void Awake()
    {
        Services.LevelManager = this;
        Services.EventManager = new GameEventsManager();
        Services.GridObjectDataManager = new GridObjectDataManager();
        Services.TerrainDataManager = new TerrainDataManager();
        _mapManager = new MapManager();
    }

    private void Update()
    {
        _mapManager.Update();
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }
    }

    //temp for testing
    public void RestartPlayerTurn()
    {
        Services.EventManager.Fire(new PlayerTurnStarted());
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public class PlayerTurnEnded : GameEvent
{

}

public class PlayerTurnStarted : GameEvent
{

}
