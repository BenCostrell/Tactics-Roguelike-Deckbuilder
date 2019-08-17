using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    public Image ring;
    public Image potentialEnergyRing;
    private float targetFillProportion;
    private float previousFillProportion;
    private float fillLerpTimeRemaining;
    private const float fillLerpDuration = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
        Services.EventManager.Register<CardRendererHoverStart>(OnCardHoverStart);
        Services.EventManager.Register<CardRendererHoverEnd>(OnCardHoverEnd);
        Services.EventManager.Register<InputHover>(OnInputHover);
        previousFillProportion = 1;
        potentialEnergyRing.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (fillLerpTimeRemaining > 0)
        {
            fillLerpTimeRemaining -= Time.deltaTime;
            ring.fillAmount = Mathf.Lerp(previousFillProportion, targetFillProportion,
                EasingEquations.Easing.QuadEaseOut(1 - (fillLerpTimeRemaining / fillLerpDuration)));
            if (fillLerpTimeRemaining <= 0)
            {
                previousFillProportion = targetFillProportion;
            }
        }
    }

    private void OnEnergyChanged(EnergyChanged e)
    {
        textComponent.text = e.currentEnergy + "/" + e.currentMaxEnergy;
        targetFillProportion = (float)e.currentEnergy / e.currentMaxEnergy;
        fillLerpTimeRemaining = fillLerpDuration;
    }

    private void OnCardHoverStart(CardRendererHoverStart e)
    {
        SetPotentialFill(e.cardRenderer.card.cost);
    }

    private void OnCardHoverEnd(CardRendererHoverEnd e)
    {
        if(!e.otherCardHovered)
        {
            potentialEnergyRing.fillAmount = 0;
        }
    }

    private void SetPotentialFill(int energyCost)
    {
        Player player = Services.LevelManager.player;
        if (energyCost <= player.currentEnergy)
        {
            potentialEnergyRing.fillAmount = (float)energyCost / player.currentMaxEnergy;
            potentialEnergyRing.transform.localEulerAngles = new Vector3(0, 0, 360f * (float)player.currentEnergy / player.currentMaxEnergy);
        }
    }

    private void OnInputHover(InputHover e)
    {
        if (e.cardSelected || e.hoveredCard != null) return;
        if (e.hoveredTile == null)
        {
            potentialEnergyRing.fillAmount = 0;
        }
        else
        {
            Player player = Services.LevelManager.player;
            List<MapTile> playerAvailableGoals = AStarSearch.FindAllAvailableGoals(player.currentTile,
                player.currentEnergy, player);
            if (playerAvailableGoals.Contains(e.hoveredTile.tile))
            {
                List<MapTile> path = AStarSearch.ShortestPath(player.currentTile, e.hoveredTile.tile, player);
                SetPotentialFill(path.Count);
            }
            else
            {
                potentialEnergyRing.fillAmount = 0;
            }
        }
    }
}
