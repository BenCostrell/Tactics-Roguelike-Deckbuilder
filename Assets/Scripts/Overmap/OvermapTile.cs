using UnityEngine;
using System.Collections;
using TMPro;

public class OvermapTile : MonoBehaviour
{
    public LevelData levelData;
    public SpriteRenderer terrainSr;
    public SpriteRenderer[] enemyTypeSrs;
    public SpriteRenderer[] rewardSrs;
    public SpriteRenderer optionReticle;
    private bool availableOption;
    private const float optionReticleLerpPeriod = 0.5f;
    private float lerpTimeRemaining;
    private bool lerpUp;
    private const float optionMaxAlpha = 0.7f;
    private Color currentOptionColor;
    private readonly Color unhighlightedOptionColor = new Color(1, 1, 1, optionMaxAlpha);
    private readonly Color highlightedOptionColor = new Color(0, 0, 1, optionMaxAlpha);

    public void Init(LevelData levelData_, Coord coord, bool availableOption_)
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
        availableOption = availableOption_;
        if (!availableOption) optionReticle.gameObject.SetActive(false);
        else
        {
            currentOptionColor = unhighlightedOptionColor;
            optionReticle.color = currentOptionColor;
            Services.EventManager.Register<LevelSelected>(OnLevelSelected);
        }
    }

    private void Update()
    {
        if (availableOption)
        {
            LerpColor(CheckHover());   
        }
    }


    private void LerpColor(bool highlighted)
    {
        lerpTimeRemaining -= Time.deltaTime;
        float alphaTarget = lerpUp ? optionMaxAlpha : 0;
        if (!highlighted)
        {
            optionReticle.color = new Color(currentOptionColor.r, currentOptionColor.g, currentOptionColor.b,
                Mathf.Lerp(optionMaxAlpha - alphaTarget, alphaTarget, EasingEquations.Easing.QuadEaseOut(
                    (optionReticleLerpPeriod - lerpTimeRemaining) / optionReticleLerpPeriod)));
        }
        if (lerpTimeRemaining <= 0)
        {
            lerpTimeRemaining = optionReticleLerpPeriod;
            lerpUp = !lerpUp;
        }
    }

    private bool CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        bool highlighted = false;
        if (hit.collider != null)
        {
            if (hit.collider.gameObject == gameObject)
            {
                highlighted = true;
            }
        }
        if (highlighted)
        {
            optionReticle.color = highlightedOptionColor;
            if (Input.GetMouseButtonDown(0))
            {
                Services.EventManager.Fire(new LevelSelected(levelData, transform.localPosition));
            }
        }
        return highlighted;
    }

    private void OnLevelSelected(LevelSelected e)
    {
        availableOption = false;
        optionReticle.gameObject.SetActive(false);
    }
}

public class LevelSelected : GameEvent
{
    public readonly LevelData levelData;
    public readonly Vector3 pos;
    public LevelSelected(LevelData levelData_, Vector3 pos_)
    {
        levelData = levelData_;
        pos = pos_;
    }
}
