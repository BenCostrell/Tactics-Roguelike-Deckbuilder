using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OvermapManager : MonoBehaviour
{
    private OvermapTile[,] overmap;
    public OvermapTile overmapTilePrefab;
    public Camera cam;
    public Transform playerIcon;
    private const float playerLerpTime = 1f;
    private float playerLerpTimeElapsed;
    private bool transitioning;
    private Vector3 playerStart;
    private Vector3 playerTarget;

    // Start is called before the first frame update
    void Start()
    {
        overmap = new OvermapTile[SaveData.levelsPerRun, SaveData.levelsPerRun];
        LevelData currentLevel = SaveData.currentlyLoadedData.currentLevel;
        Coord currentLevelCoord = new Coord(0, 0);
        for (int x = 0; x < SaveData.levelsPerRun; x++)
        {
            for (int y = 0; y < SaveData.levelsPerRun; y++)
            {
                LevelData levelData = SaveData.currentlyLoadedData.levelDatas[x, y];
                if (levelData == currentLevel)
                {
                    currentLevelCoord = new Coord(x, y);
                }
                if (levelData != null)
                {
                    OvermapTile overmapTile = Instantiate(overmapTilePrefab, transform);
                    overmapTile.Init(levelData, new Coord(x, y), false);
                    overmap[x, y] = overmapTile;
                }
            }
        }

        //setting camera
        cam.transform.localPosition = new Vector3(currentLevelCoord.x + 0.5f, currentLevelCoord.y + 0.5f,
            cam.transform.localPosition.z);
        playerStart = new Vector3(currentLevelCoord.x, currentLevelCoord.y, 0);
        playerIcon.localPosition = playerStart;

        List<GridObjectData> possibleEnemies = Services.GridObjectDataManager.GetEnemies();
        IntVector2 size = currentLevel.difficulty == 1 ? new IntVector2(7, 6) : new IntVector2(8, 6);
        for (int i = 0; i < 2; i++)
        {
            OvermapTile option = Instantiate(overmapTilePrefab, transform);
            List<GridObjectSpawnData> enemySpawns = new List<GridObjectSpawnData>(currentLevel.enemySpawns);
            GridObjectData addedEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
            possibleEnemies.Remove(addedEnemy);
            GridObjectSpawnData newSpawnData;
            int numToSpawn = 1;
            foreach (GridObjectSpawnData gridObjectSpawnData in enemySpawns)
            {
                if (gridObjectSpawnData.gridObjectData.gridObjectName == addedEnemy.gridObjectName)
                {
                    numToSpawn = gridObjectSpawnData.numToSpawn + 1;
                    enemySpawns.Remove(gridObjectSpawnData);
                    break;
                }
            }
            newSpawnData = new GridObjectSpawnData(addedEnemy, numToSpawn);
            enemySpawns.Add(newSpawnData);
            //this part is jank right now
            LevelData optionLevelData = new LevelData(currentLevel.difficulty + 1, size, enemySpawns,
                new List<GridObjectSpawnData>(){
                                    new GridObjectSpawnData( Services.GridObjectDataManager.GetData("BRUSH"),
                                    (size.x*2)-2) },
                new List<GridObjectSpawnData>() {
                                    new GridObjectSpawnData(Services.GridObjectDataManager.GetData("CHEST"), 1)});
            Coord optionCoord = new Coord(currentLevelCoord.x + (1 - i), currentLevelCoord.y + i);
            option.Init(optionLevelData, optionCoord, true);
            SaveData.currentlyLoadedData.levelDatas[optionCoord.x, optionCoord.y] = optionLevelData;
        }
        Services.EventManager.Register<LevelSelected>(OnLevelSelected);
    }

    // Update is called once per frame
    void Update()
    {
        if (transitioning)
        {
            playerLerpTimeElapsed += Time.deltaTime;
            playerIcon.transform.localPosition = Vector3.Lerp(playerStart, playerTarget,
                EasingEquations.Easing.QuadEaseOut(playerLerpTimeElapsed / playerLerpTime));
            if (playerLerpTimeElapsed >= playerLerpTime)
            {
                transitioning = false;
                Services.EventManager.Fire(new StartLevelTransitionAnimation());
            }
        }
    }

    private void OnLevelSelected(LevelSelected e)
    {
        SaveData.currentlyLoadedData.currentLevel = e.levelData;
        Services.EventManager.Register<TransitionAnimationComplete>(OnTransitionComplete);
        Services.EventManager.Unregister<LevelSelected>(OnLevelSelected);
        transitioning = true;
        playerLerpTimeElapsed = 0;
        playerTarget = e.pos;
    }

    private void OnTransitionComplete(TransitionAnimationComplete e)
    {
        Services.EventManager.Unregister<TransitionAnimationComplete>(OnTransitionComplete);
        SceneManager.LoadScene(0);
    }
}
