using UnityEngine;
using System.Collections;
using TMPro;

public class OvermapTile : MonoBehaviour
{
    public LevelData levelData;
    public SpriteRenderer terrainSr;
    public SpriteRenderer[] enemyTypeSrs;
    public SpriteRenderer[] rewardSrs;

    public void Init(LevelData levelData_, Coord coord)
    {
        levelData = levelData_;
        //for now all terrain is grass
        terrainSr.sprite = Services.TerrainDataManager.terrainDataDict["GRASS"].sprite;
        for (int i = 0; i < enemyTypeSrs.Length; i++)
        {
            if (i < levelData.enemySpawns.Count)
            {
                enemyTypeSrs[i].sprite = levelData.enemySpawns[i].gridObjectData.sprite;
                enemyTypeSrs[i].GetComponentInChildren<TextMeshPro>().text = levelData.enemySpawns[i].numToSpawn.ToString();
            }
            else
            {
                enemyTypeSrs[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < rewardSrs.Length; i++)
        {
            if (i < levelData.rewardSpawns.Count)
            {
                rewardSrs[i].sprite = levelData.rewardSpawns[i].gridObjectData.sprite;
                rewardSrs[i].GetComponentInChildren<TextMeshPro>().text = levelData.rewardSpawns[i].numToSpawn.ToString();
            }
            else
            {
                rewardSrs[i].gameObject.SetActive(false);
            }
        }
        transform.localPosition = new Vector3(coord.x, coord.y);
    }

}
