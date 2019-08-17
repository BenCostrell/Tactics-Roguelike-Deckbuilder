using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveData
{
    public static SaveData currentlyLoadedData
    {
        get
        {
            if (_currentlyLoadedData == null)
            {
                _currentlyLoadedData = new SaveData();
            }
            return _currentlyLoadedData;
        }
    }

    private static SaveData _currentlyLoadedData;

    public int currentHealth;
    public int maxHealth;
    public int maxEnergy;
    public List<Card> deck;
    public int lastCompletedLevelNum;
    private const int BASE_MAX_ENERGY = 3;

    public SaveData()
    {
        deck = new List<Card>();
        maxEnergy = BASE_MAX_ENERGY;
        maxHealth = Services.GridObjectDataManager.GetData("PLAYER").maxHealth;
        currentHealth = maxHealth;
        lastCompletedLevelNum = 0;
        Debug.Log("creating new save data");
    }

    public void OnLevelLoad()
    {
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
        Services.EventManager.Register<DamageTaken>(OnDamageTaken);
        Services.EventManager.Register<LevelCompleted>(OnLevelCompleted);
        Services.EventManager.Register<CardAcquired>(OnCardAcquired);
    }

    private void OnEnergyChanged(EnergyChanged e)
    {
        if (e.currentMaxEnergy != e.prevMaxEnergy)
        {
            maxEnergy = e.currentMaxEnergy;
        }
    }

    private void OnDamageTaken(DamageTaken e)
    {
        if (e.gridObject == Services.LevelManager.player)
        {
            currentHealth = e.gridObject.currentHealth;
            Debug.Log("changing current health to " + currentHealth);
        }
    }

    private void OnLevelCompleted(LevelCompleted e)
    {
        lastCompletedLevelNum = e.levelNum;
    }

    private void OnCardAcquired(CardAcquired e)
    {
        deck.Add(e.card);
    }
}
