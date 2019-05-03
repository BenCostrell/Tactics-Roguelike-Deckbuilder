using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergyUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    
    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        Services.EventManager.Register<EnergyChanged>(OnEnergyChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnergyChanged(EnergyChanged e)
    {
        textComponent.text = e.currentEnergy + "/" + e.currentMaxEnergy;
    }
}
