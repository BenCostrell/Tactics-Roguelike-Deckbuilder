using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private MapManager _mapManager;
    [HideInInspector]
    public Player player;
    public LevelTransitionAnimation levelTransition;


    private void Awake()
    {
        Services.LevelManager = this;
        Services.EventManager = new GameEventsManager();
        if (Services.GridObjectDataManager == null) Services.GridObjectDataManager = new GridObjectDataManager();
        if (Services.CardDataManager == null) Services.CardDataManager = new CardDataManager();
        if (Services.TerrainDataManager == null) Services.TerrainDataManager = new TerrainDataManager();
        SaveData.currentlyLoadedData.OnLevelLoad();
        _mapManager = new MapManager();
        if (Services.CardManager == null) Services.CardManager = new CardManager();
        Services.EventManager.Register<TransitionAnimationComplete>(OnTransitionComplete);
    }

    private void Start()
    {
        Services.CardManager.OnLevelStart();
    }

    private void Update()
    {
        _mapManager.Update();
        Services.CardManager.Update();
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadLevel();
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Services.CardManager.OnLevelStart();
        //}
    }

    //temp for testing
    public void RestartPlayerTurn()
    {
        Services.EventManager.Fire(new PlayerTurnStarted());
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTransitionComplete(TransitionAnimationComplete e)
    {
        //ReloadLevel();
        SceneManager.LoadScene(1);
    }
}

public class PlayerTurnEnded : GameEvent
{

}

public class PlayerTurnStarted : GameEvent
{

}

public class LevelCompleted : GameEvent
{
    public readonly int levelNum;
    public LevelCompleted(int levelNum_)
    {
        levelNum = levelNum_;
    }
}
