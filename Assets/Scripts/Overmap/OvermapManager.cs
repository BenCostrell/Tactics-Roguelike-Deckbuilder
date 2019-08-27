using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvermapManager : MonoBehaviour
{
    private OvermapTile[,] overmap;
    public OvermapTile overmapTilePrefab;
    public Camera cam;
    
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
                    overmapTile.Init(levelData, new Coord(x, y));
                    overmap[x, y] = overmapTile;
                }
            }
        }

        //setting camera
        cam.transform.localPosition = new Vector3(currentLevelCoord.x + 0.5f, currentLevelCoord.y + 0.5f, 
            cam.transform.localPosition.z);

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
            option.Init(optionLevelData, optionCoord);
            SaveData.currentlyLoadedData.levelDatas[optionCoord.x, optionCoord.y] = optionLevelData;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
