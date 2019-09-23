using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PlayerTooltip : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Image image;
    public Image healthBarFront;
    public TextMeshProUGUI healthText;
    private float targetHealthFillProportion;
    private float previousHealthFillProportion;
    private float fillLerpTimeRemaining;
    private const float fillLerpDuration = 0.3f;

    // Use this for initialization
    void Start()
    {
        Player player = Services.LevelManager.player;
        image.sprite = player.data.sprite;
        healthText.text = "<color=\"red\">HP</color> " + player.currentHealth + "/" + player.maxHealth;
        healthBarFront.fillAmount = (float)player.currentHealth / player.maxHealth;
        Services.EventManager.Register<HealthChange>(OnHealthChange);
    }

    // Update is called once per frame
    void Update()
    {
        if(fillLerpTimeRemaining < 0)
        {
            fillLerpTimeRemaining -= Time.deltaTime;
            healthBarFront.fillAmount = Mathf.Lerp(previousHealthFillProportion, targetHealthFillProportion,
                EasingEquations.Easing.QuadEaseOut(1 - (fillLerpTimeRemaining / fillLerpDuration)));
            if(fillLerpTimeRemaining <= 0)
            {
                previousHealthFillProportion = targetHealthFillProportion;
            }
        }
    }

   
    private void OnHealthChange(HealthChange e)
    {
        if (e.gridObject != Services.LevelManager.player) return;
        targetHealthFillProportion = (float)e.gridObject.currentHealth / e.gridObject.maxHealth;
        healthText.text = "<color=\"red\">HP</color> " + e.gridObject.currentHealth + "/" + e.gridObject.maxHealth;
        fillLerpTimeRemaining = fillLerpDuration;
    }
}
