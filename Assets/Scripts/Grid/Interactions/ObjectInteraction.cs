using UnityEngine;
using System.Collections;

public abstract class ObjectInteraction 
{
    public abstract void OnInteract(GridObject gridObject);

    public static ObjectInteraction ParseInteractionString(string interactionString)
    {
        string[] splitBehaviorString = interactionString.Split(',');
        string baseBehaviorString = splitBehaviorString[0].ToUpper();
        switch (baseBehaviorString)
        {
            case "GET_CARD":
                return new GetCard(
                    CardData.rarityStringDict[splitBehaviorString[1].ToUpper().Trim()]
                    );
            default:
                return null;
        }
    }
}
