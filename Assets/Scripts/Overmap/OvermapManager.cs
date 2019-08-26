using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvermapManager : MonoBehaviour
{
    private OvermapTile[,] overmap;
    
    // Start is called before the first frame update
    void Start()
    {
        overmap = new OvermapTile[SaveData.levelsPerRun, SaveData.levelsPerRun];
        for (int x = 0; x < overmap.Length; x++)
        {
            for (int y = 0; y < overmap.Length; y++)
            {
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
