using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerrainTooltip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image terrainImage;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = "";
        descriptionText.text = "";
        terrainImage.enabled = false;
        Services.EventManager.Register<InputHover>(OnInputHover);
    }

    public void OnInputHover(InputHover e)
    {
        if (e.hoveredTile != null)
        {
            TerrainData terrainData = e.hoveredTile.tile.terrain;
            terrainImage.enabled = true;
            terrainImage.sprite = terrainData.sprite;
            nameText.text = terrainData.terrainName;
            descriptionText.text = terrainData.description;
        }
        else
        {
            nameText.text = "";
            descriptionText.text = "";
            terrainImage.enabled = false;
        }
    }
}
