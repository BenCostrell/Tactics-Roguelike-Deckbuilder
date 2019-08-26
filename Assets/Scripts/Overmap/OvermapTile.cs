using UnityEngine;
using System.Collections;

public class OvermapTile : MonoBehaviour
{
    public LevelData levelData;
    public SpriteRenderer terrainSr;
    public SpriteRenderer[] enemyTypeSrs;
    public SpriteRenderer[] rewardSrs;

    public void Init(LevelData levelData_)
    {
        levelData = levelData_;
        //for now all terrain is grass
        terrainSr.sprite = Services.TerrainDataManager.terrainDataDict["GRASS"].sprite;
        for (int i = 0; i < enemyTypeSrs.Length; i++)
        {
            if (i < levelData.enemySpawns.Count)
            {
                enemyTypeSrs[i].sprite = levelData.enemySpawns[i].gridObjectData.sprite;
            }
            else
            {
                enemyTypeSrs[i].enabled = false;
            }
        }
        for (int i = 0; i < rewardSrs.Length; i++)
        {
            if (i < levelData.rewardSpawns.Count)
            {
                rewardSrs[i].sprite = levelData.rewardSpawns[i].gridObjectData.sprite;
            }
            else
            {
                rewardSrs[i].enabled = false;
            }
        }
    }

}
