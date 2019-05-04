using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private Image _image;
    private bool _enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _enabled = true;
        Services.EventManager.Register<PlayerTurnEnded>(OnPlayerTurnEnded);
        Services.EventManager.Register<PlayerTurnStarted>(OnPlayerTurnStarted);
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
    }

    public void EndTurn()
    {
        if (!_enabled) return;
        Services.EventManager.Fire(new PlayerTurnEnded());
    }

    public void OnPlayerTurnEnded(PlayerTurnEnded e)
    {
        _enabled = false;
        _image.color = Color.gray;
    }

    public void OnPlayerTurnStarted(PlayerTurnStarted e)
    {
        _enabled = true;
        _image.color = Color.yellow;
    }

    public void OnEnergyChanged(EnergyChanged e)
    {
        if (e.currentEnergy == 0 && _enabled)
        {
            _image.color = Color.green;
        }
    }
}
