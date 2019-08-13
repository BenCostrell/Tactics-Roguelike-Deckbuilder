using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image unitImage;
    public Image healthBarFront;
    public TextMeshProUGUI healthText;
    public GameObject healthBarUI;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = "";
        healthText.text = "";
        unitImage.enabled = false;
        healthBarUI.SetActive(false);
        Services.EventManager.Register<InputHover>(OnInputHover);
    }

    public void OnInputHover(InputHover e)
    {
        if (e.hoveredTile != null && e.hoveredTile.tile.containedObjects.Count > 0)
        {
            GridObject gridObject = e.hoveredTile.tile.containedObjects[0];
            unitImage.enabled = true;
            unitImage.sprite = gridObject.data.sprite;
            healthBarUI.SetActive(true);
            healthText.text = gridObject.currentHealth + "/" + gridObject.maxHealth;
            healthBarFront.transform.localScale = new Vector3(
                (float)gridObject.currentHealth / gridObject.maxHealth, 1, 1);
            nameText.text = gridObject.data.gridObjectName;
        }
        else
        {
            nameText.text = "";
            healthText.text = "";
            unitImage.enabled = false;
            healthBarUI.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
